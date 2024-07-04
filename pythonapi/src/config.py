import os

PATH = os.getcwd()
DATA_PATH = os.path.join(PATH, 'Data')
LOG_PATH = os.path.join(PATH, 'Log')
IMG_WIDTH = 1378
IMG_HEIGHT = 720
DIM = 3
DPI = 500
PATH_TESSERACT_EXE = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
TESSERACT_CONFIG = r'--oem 3 --psm 6'
PATH_PDF_BIN = r'poppler-22.04.0\Library\bin'
IS_SIGNED = 1
ISNOT_SIGNED = 0
WARNING_SIGNED = 2

STATUS_SUCCESS_API = 1
STATUS_FAIL_API = 2
STATUS_FAIL_READFILE = 3

#Check if exist Dir
IS_EXIST_DIR = 1
IS_NOTEXIST_DIR = 0

CONTENT_INFOR_SUCCESS = 'ALL RECORDS WERE READ SUCCESSFULLY !!!'
CONTENT_INFOR_FAIL = 'Tập tin tải lên không đúng định dạng nội dung. Vui lòng kiểm tra lại!'
CONTENT_INFOR_FAIL_PAGES = 'Vui lòng giữ thẳng giấy khi thực hiện Scan'

NUM_COLORS = 3
IMG_HEIGHT_EFF = 224
IMG_WIDTH_EFF = 224
NUM_CLASS = 2

HOST = '10.70.38.171'
PORT = 8009


