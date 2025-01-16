from flask_cors import CORS

def init_cors(app):
    CORS(app, supports_credentials=True, resources={r"/*": {
        "origins": "*",  # Thay * bằng danh sách domain cụ thể nếu cần
        "methods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
        "allow_headers": ["Content-Type", "Authorization", "username"],  # Thêm username vào đây
        "expose_headers": ["Content-Type", "Authorization", "username"]
    }})
