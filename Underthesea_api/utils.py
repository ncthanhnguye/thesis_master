import re
from collections import Counter
from underthesea import word_tokenize
import numpy as np

# Đọc stopwords từ file
with open('vietnamese-stopwords.txt', 'r', encoding='utf-8') as file:
    stopwords = [line.strip() for line in file]

def extract_keywords(text):
    tokens = word_tokenize(text)
    filtered_tokens = [token.lower() for token in tokens if token.lower() not in stopwords]
    return filtered_tokens

def normalize_keyword(keyword):
    return re.sub(r'\W+', '', keyword.lower().strip())

def calculate_similarity(list1, list2):
    counter1 = Counter(list1)
    counter2 = Counter(list2)
    common_keywords = sum((counter1 & counter2).values())
    total_keywords = sum(counter1.values())
    return np.divide(common_keywords, total_keywords) if total_keywords > 0 else 0
