import sys
import os
from pathlib import Path
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM
import torch
import random

MODEL_DIR = "models/vit5-response-model"

tokenizer = AutoTokenizer.from_pretrained(MODEL_DIR, local_files_only=True)
model = AutoModelForSeq2SeqLM.from_pretrained(MODEL_DIR, local_files_only=True).to("cuda" if torch.cuda.is_available() else "cpu")

def build_prompt(input_text):
    return input_text.strip()

def clean_text(text):
    # Loại bỏ ký tự không phải UTF-8 hợp lệ
    return text.encode("utf-8", errors="ignore").decode("utf-8", errors="ignore")

def generate_response(user_input):
    prompt = build_prompt(user_input)
    inputs = tokenizer(prompt, return_tensors="pt", padding=True, truncation=True, max_length=64).to(model.device)
    output = model.generate(
        **inputs,
        max_new_tokens=80,
        do_sample=True,
        temperature=1.3,
        top_k=50,
        top_p=0.95,
        num_return_sequences=3
    )
    response = tokenizer.decode(random.choice(output), skip_special_tokens=True)
    response = response.replace("br>", "<br>")
    return clean_text(response)  # 👈 đảm bảo UTF-8 sạch

if __name__ == "__main__":
    sys.stdout.reconfigure(encoding='utf-8')
    input_text = " ".join(sys.argv[1:]) if len(sys.argv) > 1 else ""
    print(generate_response(input_text), flush=True)
