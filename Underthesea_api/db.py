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

	query = "SELECT ID,TenCauHoi,LinhVuc, CauTraLoi ,Luat, KeyWords FROM DATA_CRAWL" # Truy vấn SQL 
	try:
		cursor.execute(query) 
		results = cursor.fetchall() 

		# Mỗi dòng dữ liệu sẽ là chứa KeyWords và Luat
		data = []
		for row in results:
			ID = row[0]
			TenCauHoi = row[1]
			LinhVuc = row[2]
			CauTraLoi = row[3]
			Luat = row[4]
			KeyWords = eval(row[5]) if isinstance(row[5], str) else row[5] # Chuyển chuỗi thành danh sách

			data.append((ID,TenCauHoi,LinhVuc ,CauTraLoi ,Luat , KeyWords))

	except Exception as e:
		print(f"Error occurred: {e}")
		data = [] 
	finally:
		conn.close() 

	return data #Trả về danh sách
