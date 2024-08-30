from flask import Flask, request, jsonify
from db import get_all_data
from utils import extract_keywords, normalize_keyword, calculate_similarity
from cache import cache

# Khởi tạo Flask app
app = Flask(__name__)
cache.init_app(app)

@app.route('/process', methods=['POST'])
def process_text_api():
    data = request.get_json()
    text = data.get('text', '')

    if not text:
        return jsonify({'error': 'No text provided'}), 400
    
    # Gọi hàm để xử lý và trích xuất từ khóa
    keywords = extract_keywords(text)
    normalized_keywords = [normalize_keyword(keyword) for keyword in keywords]

    # Gọi hàm để lấy toàn bộ dữ liệu từ bảng DATA_CRAWL
    all_data = get_all_data()

    # Tìm dòng đầu tiên có tỷ lệ trùng khớp cao nhất
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
            break  # Dừng lại sau khi tìm thấy dòng đầu tiên

    if best_match:
        return jsonify({
            'luat': best_match,
            'input_keywords': keywords,
            'matched_keywords': best_match_keywords,
            'similarity': highest_similarity
        })
    else:
        return jsonify({'message': 'Không tìm thấy dòng nào có tỷ lệ trùng khớp cao'}), 404

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=8089)
