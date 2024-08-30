from flask import Flask, request, jsonify
from underthesea import word_tokenize
import pandas as pd

# Khởi tạo Flask app
app = Flask(__name__)

# Tạo danh sách stopwords
with open('vietnamese-stopwords.txt', 'r', encoding='utf-8') as file:
    stopwords = [line.strip() for line in file]

# Hàm để xử lý và trích xuất từ khóa
def extract_keywords(text):
    # Tách từ trong văn bản
    tokens = word_tokenize(text)
    
    # Loại bỏ các từ dừng
    filtered_tokens = [token.lower() for token in tokens if token.lower() not in stopwords]
    
    return filtered_tokens

# Route để xử lý văn bản
@app.route('/process', methods=['POST'])
def process_text_api():
    # Lấy đoạn văn bản từ yêu cầu của client
    data = request.get_json()
    text = data.get('text', '')

    if not text:
        return jsonify({'error': 'No text provided'}), 400
    
    # Gọi hàm để xử lý và trích xuất từ khóa
    keywords = extract_keywords(text)
    
    # Trả về kết quả dưới dạng JSON
    return jsonify({'keywords': keywords})

# Chạy Flask server
if __name__ == '__main__':
    # Chạy Flask server trên cổng 8080 và lắng nghe trên tất cả các địa chỉ IP
    app.run(debug=True, host='0.0.0.0', port=8089)
