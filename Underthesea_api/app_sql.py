from flask import Flask, request, jsonify
from db import get_all_data
from utils import extract_keywords, normalize_keyword, calculate_similarity
from cache import cache
from config import FLASK_HOST, FLASK_PORT, DEBUG_MODE
from cors_config import init_cors

app = Flask(__name__)
cache.init_app(app)

init_cors(app)

@app.route('/underthesea', methods=['POST'])
def process_text_api():
    data = request.get_json()
    text = data.get('text', '')

    if not text:
        return jsonify({'error': 'Không có dữ liệu văn bản'}), 400

    # Trích xuất 
    keywords = extract_keywords(text)
    normalized_keywords = [normalize_keyword(keyword) for keyword in keywords]

    # dữ liệu từ bảng DATA_CRAWL 
    all_data = get_all_data()

    # Danh sách để lưu trữ 
    matches = []

    # danh sách dictionary 
    for data_entry in all_data:
        keywords_list = data_entry.get('KeyWords', [])  # Lấy danh sách từ khóa KeyWords
        luat = data_entry.get('Luat', '')               # Lấy trường 'Luat' từ dictionary
        
        # Normalize 
        normalized_keywords_list = [normalize_keyword(keyword) for keyword in keywords_list]
        
        # tỷ lệ trùng khớp
        similarity = calculate_similarity(normalized_keywords, normalized_keywords_list)
        
        # Thêm  vào danh sách với tỷ lệ trùng khớp và db
        matches.append({
            'data': data_entry,  # Toàn bộ các trường từ db, dưới dạng dictionary
            'similarity': similarity
        })

    # Sắp xếp các 
    matches_sorted = sorted(matches, key=lambda x: x['similarity'], reverse=True)

    # Lấy 10 tỷ lệ trùng khớp cao nhất
    top_matches = matches_sorted[:10]

    if top_matches:
        # Trả về 10 kết quả p
        return jsonify({
            'results': [
                {
                    'data': match['data'],  # db
                    'similarity': match['similarity'] 
                }
                for match in top_matches
            ]
        })
    else:
        return jsonify({'message': 'Không tìm thấy kết quả phù hợp'})

if __name__ == '__main__':
    app.run(debug=DEBUG_MODE, host=FLASK_HOST, port=FLASK_PORT)
