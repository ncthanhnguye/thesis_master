
from flask import Flask, request, jsonify
from db import get_all_data
from utils import extract_keywords, normalize_keyword, calculate_similarity
from cache import cache
from config import FLASK_HOST, FLASK_PORT, DEBUG_MODE

app = Flask(__name__)
cache.init_app(app)

@app.route('/process', methods=['POST'])
def process_text_api():
    data = request.get_json()
    text = data.get('text', '')

    if not text:
        return jsonify({'error': 'No text provided'}), 400
    
    #hàm để xử lý và trích xuất
    keywords = extract_keywords(text)
    normalized_keywords = [normalize_keyword(keyword) for keyword in keywords]

    #hàm để lấy toàn bộ dữ liệu từ DATA_CRAWL
    all_data = get_all_data()

    #dòng đầu tiên có tỷ lệ trùng khớp cao nhất
    best_match = None
    best_match_keywords = None
    highest_similarity = 0

    for keywords_list, luat in all_data:
        normalized_keywords_list = [normalize_keyword(keyword) for keyword in keywords_list]
        similarity = calculate_similarity(normalized_keywords, normalized_keywords_list)
        if similarity > highest_similarity:
            highest_similarity = similarity
            best_match = luat
            best_match_keywords = keywords_list
            break  # Dừng lại 

    if best_match:
        return jsonify({
            'luat': best_match,
            'input_keywords': keywords,
            'matched_keywords': best_match_keywords,
            'similarity': highest_similarity
        })
    else:
        return jsonify({'message': 'Không tìm thấy'}), 404

if __name__ == '__main__':
    app.run(debug=DEBUG_MODE, host=FLASK_HOST, port=FLASK_PORT)
