import os
os.environ["TRANSFORMERS_NO_TF"] = "1"

from transformers import DistilBertTokenizerFast, DistilBertForSequenceClassification
import torch, json

model_path = os.path.abspath(os.path.join(os.path.dirname(__file__), "models", "intent-food-model"))
model = DistilBertForSequenceClassification.from_pretrained(model_path)
tokenizer = DistilBertTokenizerFast.from_pretrained(model_path)

with open(os.path.join(model_path, "labels.json")) as f:
    label2id = json.load(f)
id2label = {v: k for k, v in label2id.items()}

def predict_tags(text, threshold=0.1):
    inputs = tokenizer(text, return_tensors="pt", truncation=True, padding=True)
    with torch.no_grad():
        logits = model(**inputs).logits
        probs = torch.sigmoid(logits).squeeze().tolist()
    return [(id2label[i], round(p, 3)) for i, p in enumerate(probs) if p >= threshold]
