import shutil
from pdf2image import convert_from_path
from src.config import *
import numpy as np
import cv2
from PIL import Image, ImageEnhance
import os
from urllib.parse import urlparse
import requests
import fnmatch
import logging
from datetime import datetime
import numpy as np 
import cv2
from datetime import datetime
from sys import platform
import pytesseract
from flask import jsonify
from skimage.feature import hog
import joblib
from skimage.io import imread
from skimage.transform import resize
from skimage.color import rgb2gray
import pickle

#reader = easyocr.Reader(['vi'])
# create an instance of each transformer
modelHandSignatures = pickle.load(open("Classification_Model.p","rb"))
Categories = ["1","2","3"]

if platform == "win32":
    pytesseract.pytesseract.tesseract_cmd = PATH_TESSERACT_EXE

def Convert_PDF_toImage(pdfPath):
    pages = []
    if platform == "linux" or platform == "linux2":
        pages = convert_from_path(pdfPath, DPI)
    elif platform == "darwin":
        print('MacOSX')
    elif platform == "win32":
        pages = convert_from_path(pdfPath, DPI, poppler_path=PATH_PDF_BIN)
    #print(len(pages))
    for page in pages:
        page.save(pdfPath.replace(".pdf", "") + '_page_' + str(pages.index(page) + 1) +'.jpg', 'JPEG')
    #print("Convert to image successfully !!!")

def Resize_Image_Keep_Ratio(image, width = None, height = None, inter = cv2.INTER_AREA):
    # initialize the dimensions of the image to be resized and
    # grab the image size
    dim = None
    (h, w) = image.shape[:2]
    
    # if both the width and height are None, then return the
    # original image
    if width is None and height is None:
        return image

    # check to see if the width is None
    if width is None:
        # calculate the ratio of the height and construct the
        # dimensions
        r = height / float(h)
        dim = (int(w * r), height)

    # otherwise, the height is None
    else:
        # calculate the ratio of the width and construct the
        # dimensions
        r = width / float(w)
        dim = (width, int(h * r))

    # resize the image
    resized = cv2.resize(image, dim, interpolation = inter)

    # return the resized image
    return resized

def Enhance_Image(img, factor: int):
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    img = Image.fromarray(img)
    enhancer = ImageEnhance.Sharpness(img).enhance(factor)
    #print(gray.std())
    if gray.std() < 30:
        enhancer = ImageEnhance.Contrast(enhancer).enhance(factor)
    return np.array(enhancer)

def Compute_Hist(image):
    hist = np.zeros((256,), np.uint8)
    h, w = image.shape[:2]
    for i in range(h):
        for j in range(w):
            hist[image[i][j]] += 1
    return hist

def Equal_Hist(hist):
    cumulator = np.zeros_like(hist, np.float64)
    for i in range(len(cumulator)):
        cumulator[i] = hist[:i].sum()
    new_hist = (cumulator - cumulator.min())/(cumulator.max() - cumulator.min()) * 255
    new_hist = np.uint8(new_hist)
    return new_hist

def Run_Equal_Hist(img):
    hist = Compute_Hist(img)
    new_hist = Equal_Hist(hist)
    h, w = img.shape[:2]
    for i in range(h):
        for j in range(w):
            img[i,j] = new_hist[img[i,j]]
    return img

def Group_H_Lines(h_lines, thin_thresh):
    new_h_lines = []
    while len(h_lines) > 0:
        thresh = sorted(h_lines, key=lambda x: x[0][1])[0][0]
        lines = [line for line in h_lines if thresh[1] -
                 thin_thresh <= line[0][1] <= thresh[1] + thin_thresh]
        h_lines = [line for line in h_lines if thresh[1] - thin_thresh >
                   line[0][1] or line[0][1] > thresh[1] + thin_thresh]
        x = []
        for line in lines:
            x.append(line[0][0])
            x.append(line[0][2])
        x_min, x_max = min(x) - int(5*thin_thresh), max(x) + int(5*thin_thresh)
        new_h_lines.append([x_min, thresh[1], x_max, thresh[1]])
    return new_h_lines

def Group_V_Lines(v_lines, thin_thresh):
    new_v_lines = []
    while len(v_lines) > 0:
        thresh = sorted(v_lines, key=lambda x: x[0][0])[0][0]
        lines = [line for line in v_lines if thresh[0] -
                 thin_thresh <= line[0][0] <= thresh[0] + thin_thresh]
        v_lines = [line for line in v_lines if thresh[0] - thin_thresh >
                   line[0][0] or line[0][0] > thresh[0] + thin_thresh]
        y = []
        for line in lines:
            y.append(line[0][1])
            y.append(line[0][3])
        y_min, y_max = min(y) - int(4*thin_thresh), max(y) + int(4*thin_thresh)
        new_v_lines.append([thresh[0], y_min, thresh[0], y_max])
    return new_v_lines

def Seg_Intersect(line1: list, line2: list):
    a1, a2 = line1
    b1, b2 = line2
    da = a2-a1
    db = b2-b1
    dp = a1-b1

    def Perp(a):
        b = np.empty_like(a)
        b[0] = -a[1]
        b[1] = a[0]
        return b

    dap = Perp(da)
    denom = np.dot(dap, db)
    num = np.dot(dap, dp)
    return (num / denom.astype(float))*db + b1

def Get_Bottom_Right(right_points, bottom_points, points):
    for right in right_points:
        for bottom in bottom_points:
            if [right[0], bottom[1]] in points:
                return right[0], bottom[1]
    return None, None

def Download_File(fileName, url):
    with open(fileName, 'wb') as handle:
        response = requests.get(str(url), stream=True)

        if not response.ok:
            return False

        for block in response.iter_content(1024):
            if not block:
                break
            handle.write(block)
    return True

def Get_FileName(urlFile):
    x = urlparse(urlFile)
    fileName = os.path.basename(x.path)
    return fileName

def Proccess_Get_Data(url):
    fileName = Get_FileName(url)
    folderName = fileName.replace(".pdf", "")
    dataFolder = os.path.join(DATA_PATH, folderName)
    makeDir = False
    if not os.path.exists(dataFolder):
        os.makedirs(dataFolder)
        makeDir = True
    else:
        for file in os.listdir(dataFolder):
            filePath = os.path.join(dataFolder, file)
            if os.path.isfile(filePath) or os.path.islink(filePath):
                os.unlink(filePath)
            elif os.path.isdir(filePath):
                shutil.rmtree(filePath)
        makeDir = True
    if makeDir == True:
        pathDowwnload = os.path.join(dataFolder, fileName)
        if Download_File(pathDowwnload, url):
            Convert_PDF_toImage(os.path.join(dataFolder, fileName))
    
    return dataFolder

def Get_All_Images(dataFolder):
    listImageName = [f for f in os.listdir(dataFolder) if fnmatch.fnmatch(f, '*.jpg')]
    #print (listImageName)
    return listImageName

def Save_Log(status, contentInfor):
    now = datetime.now()
    filenameLog = os.path.join(LOG_PATH, now.strftime("%d%m%Y_%H%M%S")) 
    logging.basicConfig(filename = str(filenameLog)+'.log',
                        filemode = 'w',
                        format = '%(asctime)s %(message)s',
                        #datefmt = '%H:%M:%S',
                        level = logging.DEBUG
    )
    logger = logging.getLogger()

    logger.debug(status)
    logger.info(contentInfor)
    return

def Check_IsSigned_EFF(image):
    flat_data = []
    img_resized = resize(image,(150,150,3))
    flat_data.append(img_resized.flatten())
    flat_data = np.array(flat_data)
    y_output = modelHandSignatures.predict(flat_data)
    y_output = Categories[y_output[0]]
    return y_output

def Get_Cols(thresh, ver_kernel):
    cols = 0
    # Find number of columns
    #vertical = cv2.morphologyEx(thresh, cv2.MORPH_RECT, ver_kernel, iterations=11) #chọn Iteration theo số nguyên tố
    # cv2.imshow('vcc', thresh)
    # cv2.waitKey(0)
    cnts = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE) #thay thresh = vertical nếu cần
    cnts = cnts[0] if len(cnts) == 2 else cnts[1]
    cols = len(cnts) - 1
    return cols

def Try_Or(value, key = ''):
        if key == 'stt':
            try:
                return int(value)
            except:
                return value
        if key == 'date':
            try:
                print(value)
                value = datetime.strptime(value, '%d/%m/%Y').date()
                return value
            except Exception as e:
                print(e)
                return value

def Proccess_OCR_V2(fileName, typeFile, listText, dataFolder):
    image = os.path.join(dataFolder, fileName)
    table_image = cv2.imread(image)
    table_image = table_image[120:5670, 120:5670] # trái cộng phải trừ, trái bằng phải (trên : dưới, trái : phải)
    
    table_image = Resize_Image_Keep_Ratio(table_image, height=IMG_HEIGHT)
    table_image = cv2.cvtColor(table_image, cv2.COLOR_BGR2GRAY)
    enhance_im = Run_Equal_Hist(table_image)
    gray = enhance_im
    thresh, img_bin = cv2.threshold(
        gray, 128, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)
    img_bin = 255-img_bin
    kernel_len = gray.shape[1]//120
    hor_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (kernel_len, 1))
    image_horizontal = cv2.erode(img_bin, hor_kernel, iterations=7)
    horizontal_lines = cv2.dilate(image_horizontal, hor_kernel, iterations=7)

    h_lines = cv2.HoughLinesP(horizontal_lines, 1, np.pi/180, 100, maxLineGap=250)

    new_horizontal_lines = Group_H_Lines(h_lines, kernel_len)

    kernel_len = gray.shape[1]//120
    ver_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1, kernel_len))
    image_vertical = cv2.erode(img_bin, ver_kernel, iterations=9)
    vertical_lines = cv2.dilate(image_vertical, ver_kernel, iterations=3)
    v_lines = cv2.HoughLinesP(vertical_lines, 1, np.pi/180, 100, maxLineGap=250)
    new_vertical_lines = Group_V_Lines(v_lines, kernel_len)

    points = []
    #cols = Get_Cols(img_bin, ver_kernel)
    cols = Get_Cols(image_vertical, ver_kernel)
    print(cols)
    if((typeFile == '0' and cols != 8) or (typeFile == '1' and cols != 9)):
        return jsonify(
                Status = STATUS_FAIL_READFILE,
                Data = None,
                Message = CONTENT_INFOR_FAIL
                )
    else:
        for hline in new_horizontal_lines:
            x1A, y1A, x2A, y2A = hline
            for vline in new_vertical_lines:
                x1B, y1B, x2B, y2B = vline

                line1 = [np.array([x1A, y1A]), np.array([x2A, y2A])]
                line2 = [np.array([x1B, y1B]), np.array([x2B, y2B])]

                x, y = Seg_Intersect(line1, line2)
                if x1A <= x <= x2A and y1B <= y <= y2B:
                    points.append([int(x), int(y)])

        cells = []
        
        listContent = []
        listContentTmp = []
        for point in points:
            left, top = point
            right_points = [p for p in points if p[0] > left and p[1] == top]
            bottom_points = [p for p in points if p[1] > top and p[0] == left]
            right, bottom = Get_Bottom_Right(
                right_points, bottom_points, points)
            if right and bottom:
                imgRec = cv2.rectangle(table_image, (left, top), (right, bottom), (0, 0, 255), 5)
                
                cropped_image = table_image[top+7:bottom-7, left+7:right-7] # Slicing to crop the image
                cropped_image = Resize_Image_Keep_Ratio(cropped_image, height=128)
                #cropped_image = Run_Equal_Hist(cropped_image)
                #listContent.append(cropped_image)
                # if(len(listContent) == cols):
                #     cv2.imwrite(fileName.replace('.jpg', '') + str([top,bottom,left,right]) + '_crop.jpg',listContent[cols -1])
                #     listContent.clear()
                #cropped_gray = cv2.cvtColor(cropped_image, cv2.COLOR_BGR2GRAY)
                cropped_blur = cv2.GaussianBlur(cropped_image, (3, 3), 0)
                cropped_thresh = cv2.threshold(cropped_blur, 0, 255, cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)[1]

                #print(reader.readtext(cropped_blur, detail = 0))
                # Morph open to remove noise and invert image
                kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (2,2))
                opening = cv2.morphologyEx(cropped_thresh, cv2.MORPH_OPEN, kernel, iterations=1)
                #closing = cv2.morphologyEx(opening, cv2.MORPH_OPEN, kernel, iterations=1)
                invert = 255 - opening
                # listContent.append(pytesseract.image_to_string(invert, lang='vie', config=TESSERACT_CONFIG)
                #                                                         .replace("\n"," ")
                #                                                         .replace("l "," ")
                #                                                         .replace("`", "")
                #                                                         .strip())
                listContent.append(invert)
                if(len(listContent) == cols):
                    #cv2.imwrite(str([top,bottom,left,right]) + '_blur.jpg', cropped_blur)
                    #listContent[cols - 1] = Check_IsSigned_EFF(cropped_blur)
                    statusSig = Check_IsSigned_EFF(cropped_blur)
                    contentCCCD = pytesseract.image_to_string(listContent[4], config=TESSERACT_CONFIG).replace("\n"," ").replace("l "," ").replace("`", "").strip()
                    if(typeFile == '0'): #Danh sach ky nhan DV, NLD
                        receiverItem = {
                            "Code": contentCCCD,
                            "Status": str(statusSig)
                        }
                        listContentTmp.append(receiverItem)
                        listContent.clear()
                    elif(typeFile == '1'): #Danh sach ky nhan Gia dinh
                        receiverItem = {
                            "OrderIdx": Try_Or(listContent[0], key = 'stt'),
                            "Name": listContent[1],
                            "UnitName": listContent[2],
                            "UnitParentName": listContent[3],
                            "Code": listContent[4],
                            "Status": str(listContent[8])
                        }
                        listContentTmp.append(receiverItem)
                        listContent.clear()
        del listContentTmp[0]
        #listText.extend(listContentTmp)
        cv2.imwrite(image.replace('.jpg', '') + '_rec.jpg', imgRec)
        return listContentTmp



# if __name__ == "__main__":
#     url = "http://10.70.105.15:8016/Upload/Tmp/Account/admin/20220805131948467_Danh%20s%C3%A1ch%20%C4%91%C6%B0%E1%BB%A3c%20nh%E1%BA%ADn%20(4).pdf"
#     Convert_PDF_toImage(DATA_PATH + "/DS_NLD/" + "DS_NLD_DAKY_SCAN.pdf")
