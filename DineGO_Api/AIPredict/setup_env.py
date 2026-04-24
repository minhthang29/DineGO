import os
import sys
import subprocess

def run(cmd_list):
    print(f"📦 Running: {' '.join(cmd_list)}")
    subprocess.run(cmd_list, check=True)

if __name__ == "__main__":
    python_exec = sys.executable
    venv_dir = "venv"

    if not os.path.exists(venv_dir):
        run([python_exec, "-m", "venv", venv_dir])
        print("✅ Virtual environment created.")

    # Dù đang trong venv hay không → xác định python của venv
    venv_python = os.path.join(venv_dir, "Scripts", "python.exe") if os.name == "nt" else os.path.join(venv_dir, "bin", "python")

    if not os.path.isfile(venv_python):
        raise FileNotFoundError(f"Không tìm thấy Python trong venv: {venv_python}")

    # Gọi pip qua python -m pip (ổn định nhất)
    run([venv_python, "-m", "pip", "install", "--disable-pip-version-check", "-r", "requirements.txt"])

    print("🎉 Setup hoàn tất! Bạn đã sẵn sàng chạy AI.")
