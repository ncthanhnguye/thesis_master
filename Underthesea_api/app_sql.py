from flask import Flask, request, jsonify
from underthesea import word_tokenize
import pyodbc
import re
from collections import Counter

# Khởi tạo Flask app
app = Flask(__name__)

# Kết nối tới SQL Server
def connect_db():
    conn = pyodbc.connect('DRIVER={SQL Server};'
                          'SERVER=Tylerdo;'
                          'DATABASE=THESIS_MASTER;'
                          'UID=sa;'
                          'PWD=123456')
    return conn

# Tạo danh sách stopwords
with open('vietnamese-stopwords.txt', 'r', encoding='utf-8') as file:
    stopwords = [line.strip() for line in file]

# Hàm để xử lý và trích xuất từ khóa
def extract_keywords(text):
    tokens = word_tokenize(text)
    filtered_tokens = [token.lower() for token in tokens if token.lower() not in stopwords]
    return filtered_tokens

# Hàm để chuẩn hóa từ khóa (loại bỏ dấu câu, chuyển về chữ thường)
def normalize_keyword(keyword):
    return re.sub(r'\W+', '', keyword.lower().strip())

# Hàm để lấy toàn bộ dữ liệu từ bảng DATA_CRAWL
def get_all_data():
    conn = connect_db()  # Kết nối đến cơ sở dữ liệu
    cursor = conn.cursor()  # Tạo con trỏ để thực hiện các truy vấn SQL

    query = "SELECT KeyWords, Luat FROM DATA_CRAWL"  # Truy vấn SQL để lấy cột KeyWords và Luat
    
    try:
        cursor.execute(query)  # Thực thi truy vấn
        results = cursor.fetchall()  # Lấy tất cả các dòng kết quả

        # Mỗi dòng dữ liệu sẽ là một tuple chứa KeyWords và Luat
        data = []
        for row in results:
            keywords = eval(row[0]) if isinstance(row[0], str) else row[0]  # Chuyển đổi chuỗi thành danh sách
            luat = row[1]
            data.append((keywords, luat))

    except Exception as e:
        print(f"Error occurred: {e}")
        data = []  # Nếu xảy ra lỗi, trả về danh sách rỗng

    conn.close()  # Đóng kết nối với cơ sở dữ liệu
    
    return data  # Trả về danh sách các tuple (KeyWords, Luat)

# Hàm tính toán tỷ lệ tương đồng giữa hai danh sách từ khóa
def calculate_similarity(list1, list2):
    counter1 = Counter(list1)
    counter2 = Counter(list2)
    common_keywords = sum((counter1 & counter2).values())
    total_keywords = sum(counter1.values())
    similarity = common_keywords / total_keywords if total_keywords > 0 else 0
    return similarity

# Route để xử lý văn bản
@app.route('/process', methods=['POST'])
def process_text_api():
    data = request.get_json()
    text = data.get('text', '')

    if not text:
        return jsonify({'error': 'No text provided'}), 400
    
    # Gọi hàm để xử lý và trích xuất từ khóa
    keywords = extract_keywords(text)
    normalized_keywords = [normalize_keyword(keyword) for keyword in keywords]
    print('xử lý và trích xuất từ khóa:', normalized_keywords)

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
        print('Dòng đầu tiên có tỷ lệ trùng khớp cao nhất:', best_match)
        return jsonify({
            'luat': best_match,
            'input_keywords': keywords,
            'matched_keywords': best_match_keywords,
            'similarity': highest_similarity
        })
    else:
        print('Không tìm thấy dòng nào có tỷ lệ trùng khớp cao')
        return jsonify({'message': 'Không tìm thấy dòng nào có tỷ lệ trùng khớp cao'}), 404

# Chạy Flask server
if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=8089)
