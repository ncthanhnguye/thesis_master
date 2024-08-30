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

    query = "SELECT KeyWords, Luat FROM DATA_CRAWL"  # Truy vấn SQL 
    try:
        cursor.execute(query) 
        results = cursor.fetchall()  

        # Mỗi dòng dữ liệu sẽ là chứa KeyWords và Luat
        data = []
        for row in results:
            keywords = eval(row[0]) if isinstance(row[0], str) else row[0]  # Chuyển chuỗi thành danh sách
            luat = row[1]
            data.append((keywords, luat))

    except Exception as e:
        print(f"Error occurred: {e}")
        data = []  
    finally:
        conn.close()  
    
    return data  #Trả về danh sách
