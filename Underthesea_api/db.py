from config import connect_db
from cache import cache

@cache.cached(timeout=300, key_prefix='all_data')
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
