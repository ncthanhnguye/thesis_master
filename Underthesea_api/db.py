import pyodbc
from config import SQL_HOST, SQL_DATABASE, SQL_USER, SQL_PASSWORD

def connect_db():
    # Kết nối SQL Server
    conn = pyodbc.connect(f'DRIVER={{SQL Server}};'
                          f'SERVER={SQL_HOST};'
                          f'DATABASE={SQL_DATABASE};'
                          f'UID={SQL_USER};'
                          f'PWD={SQL_PASSWORD}')
    return conn

def get_all_data():
    conn = connect_db() 
    
    cursor = conn.cursor() 

    # Truy vấn SQL để lấy tất cả các trường
    query = "SELECT * FROM DATA_CRAWL"  # Truy vấn tất cả các cột trong bảng DATA_CRAWL
    try:
        cursor.execute(query)
        results = cursor.fetchall()

        # Lấy danh sách tên các cột từ 
        columns = [column[0] for column in cursor.description]

        data = []
        for row in results:
            # Tạo một dictionary chứa tất cả các cột và giá trị
            row_data = {}
            for i, column in enumerate(columns):
                row_data[column] = row[i]
            
            # Nếu cột KeyWords là chuỗi, chuyển đổi thành danh sách
            if 'KeyWords' in row_data and isinstance(row_data['KeyWords'], str):
                row_data['KeyWords'] = eval(row_data['KeyWords'])

            data.append(row_data)

    except Exception as e:
        print(f"Error occurred: {e}")
        data = []  # Trả về danh sách trống nếu có lỗi
    finally:
        conn.close()
    
    return data  # Trả về danh sách, mỗi dòng là một dictionary
