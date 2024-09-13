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

    # Hàm để xử lý và trích xuất từ khóa
    keywords = extract_keywords(text)
    normalized_keywords = [normalize_keyword(keyword) for keyword in keywords]

    # Hàm để lấy toàn bộ dữ liệu từ DATA_CRAWL
    all_data = get_all_data()

    # Danh sách để lưu trữ các kết quả phù hợp
    matches = []

    for keywords_list, luat in all_data:
        normalized_keywords_list = [normalize_keyword(keyword) for keyword in keywords_list]
        similarity = calculate_similarity(normalized_keywords, normalized_keywords_list)
        matches.append({
            'luat': luat,
            'keywords': keywords_list,
            'similarity': similarity
        })

    # Sắp xếp các kết quả theo tỷ lệ trùng khớp từ cao đến thấp
    matches_sorted = sorted(matches, key=lambda x: x['similarity'], reverse=True)

    # Lấy 10 kết quả có tỷ lệ trùng khớp cao nhất
    top_matches = matches_sorted[:10]

    if top_matches:
        return jsonify({
            'results': [
                {
                    'luat': match['luat'],
                }
                for match in top_matches
            ]
        })
    else:
        return jsonify({'message': 'Không tìm thấy kết quả phù hợp'})

if __name__ == '__main__':
    app.run(debug=DEBUG_MODE, host=FLASK_HOST, port=FLASK_PORT)
