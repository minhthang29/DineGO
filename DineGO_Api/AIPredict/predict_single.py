import sys
sys.dont_write_bytecode = True

from predict_unified import predict_tags
import sys, json

text = sys.argv[1]
tags = predict_tags(text)
print(json.dumps([t[0] for t in tags]))  # Chỉ in danh sách tag
