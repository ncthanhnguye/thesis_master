from flask_cors import CORS

def init_cors(app):
    # Cấu hình CORS cho toàn bộ app
    CORS(app, resources={
        r"/*": {
            "origins": "*", 
            "methods": ["GET", "POST", "PUT","DELETE"], 
            "allow_headers": ["Content-Type", "Authorization"], 
            "supports_credentials": True 
        }
    })
