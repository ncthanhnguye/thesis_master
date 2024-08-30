from flask import Flask, request, jsonify
from underthesea import word_tokenize
import pandas as pd
import nltk

# Khởi tạo Flask app
app = Flask(__name__)

# Tải xuống mô hình phân tách câu của nltk
nltk.download('punkt')

# Load Vietnamese stopwords từ file
with open('vietnamese-stopwords.txt', 'r', encoding='utf-8') as file:
    vietnamese_stopwords = [line.strip() for line in file]

# Hàm xử lý văn bản
def process_text(text):
    # Tách đoạn văn bản theo dòng
    sentences = text.splitlines()

    # Danh sách lưu trữ các câu đã xử lý
    processed_sentences = []
    for sentence_line in sentences:
        stripped_sentence_line = sentence_line.strip()
        if stripped_sentence_line:  # Kiểm tra nếu dòng không rỗng
            sentences_after_dot = stripped_sentence_line.split('.')
            for sentence_after_dot in sentences_after_dot:
                if sentence_after_dot:
                    # Tách từ trong câu
                    words = sentence_after_dot.split()  # Giả sử các từ đã được tách bằng dấu cách
                    
                    # Loại bỏ các từ dừng tiếng Việt
                    filtered_words = [word for word in words if word.lower() not in vietnamese_stopwords]
                    
                    # Nối lại câu đã xử lý và thêm vào danh sách
                    processed_sentence = ' '.join(filtered_words)
                    processed_sentences.append(processed_sentence)
    
    # Trả về danh sách các câu đã xử lý
    return processed_sentences

# Route xử lý văn bản
@app.route('/process', methods=['POST'])
def process_text_api():
    # Lấy đoạn văn bản từ request của client
    data = request.get_json()
    text = data.get('text', '')
    
    # Gọi hàm xử lý văn bản
    processed_sentences = process_text(text)
    
    # Trả kết quả dưới dạng JSON
    return jsonify({'processed_sentences': processed_sentences})

# Chạy Flask server
if __name__ == '__main__':
    # Chạy Flask server trên cổng 8080 và lắng nghe trên tất cả các địa chỉ IP
    app.run(debug=True, host='0.0.0.0', port=8089)
    