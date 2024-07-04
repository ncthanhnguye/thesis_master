import flask
from flask import request, jsonify, current_app
from flask_cors import CORS
from src.config import *
from src.utilities_upgrade import Get_All_Images, Process_Get_Data, Get_Data, OCR_Image, Get_Data_Single
from waitress import serve

app = flask.Flask(__name__)
app.config["DEBUG"] = True
app.config['CORS_HEADERS'] = 'Content-Type'
app.config["ENV"] = "production"
cors = CORS(app, resources={r"/*": {"origins": "*"}})


@app.route('/home', methods=['GET'])
def home():
    return '''<h1>API IS RUNNING</h1>
<p>A prototype API for distant reading of science fiction novels.</p>'''

@app.route('/api/v2/getReceived', methods=['GET'])
def getReceived():
    print(current_app.config["ENV"])
    print(request)
    try:
        args = request.args
        typeFile = args.get('Type')
        url = args.get('url')
        resOCR = Get_Data_Single(url, typeFile)
        if(resOCR['results'] == STATUS_FAIL_READFILE):
            return jsonify(
                Status = STATUS_FAIL_READFILE,
                Data = [],
                Message = CONTENT_INFOR_FAIL,
                FailurePage = resOCR["failurePage"]
                )
        else:
            if(len(resOCR["failurePage"]) != 0):
                return jsonify(
                        Status = STATUS_SUCCESS_API,
                        Data = resOCR['results'],
                        Message = CONTENT_INFOR_FAIL_PAGES,
                        FailurePage = resOCR["failurePage"]
                )
            else:
                return jsonify(
                        Status = STATUS_SUCCESS_API,
                        Data = resOCR['results'],
                        Message = CONTENT_INFOR_SUCCESS,
                        FailurePage = resOCR["failurePage"]
                )
    except Exception as e:
        print("Catch:")
        print(e)
        return jsonify(
            Status = STATUS_FAIL_API,
            Data = None,
            Message = e.args
            )
        
@app.route('/home', methods=['GET']) # thay '/home' thành Endpoint cần sử dụng, thay 'GET' thành method cần sử dụng
def ten_ham(tham_so):
    try:
        pass
    except Exception as e:
        print("Catch:")
        print(e)
        return jsonify(
            Status = STATUS_FAIL_API,
            Data = None,
            Message = e.args
            )

if __name__ == "__main__":
    with app.app_context():
        # app.run(host=HOST, port=PORT)
        app.run()
    #getRecieved()


# if __name__ == "__main__":
#     url = os.path.join(DATA_PATH, '20221026154009968_File_scanNLD_160ng', '20221026154009968_File_scanNLD_160ng_page_7.jpg')
#     OCR_Image(url, '0')