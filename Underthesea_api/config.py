import pyodbc

def connect_db():
    conn = pyodbc.connect('DRIVER={SQL Server};'
                          'SERVER=Tylerdo;'
                          'DATABASE=THESIS_MASTER;'
                          'UID=sa;'
                          'PWD=123456')
    return conn
