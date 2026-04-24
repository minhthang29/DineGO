const password = document.getElementById('cus_password');
const confirmPassword = document.getElementById('cus_confirm_password');
const message = document.getElementById('password-match-message');
const form = document.querySelector('form[asp-action="AddCustomer"]');

function checkPasswordMatch() {
    if (password.value !== confirmPassword.value) {
        message.textContent = "Mật khẩu xác nhận không khớp!";
        return false;
    } else {
        message.textContent = "";
        return true;
    }
}

password.addEventListener('input', checkPasswordMatch);
confirmPassword.addEventListener('input', checkPasswordMatch);

form.addEventListener('submit', function (e) {
    if (!checkPasswordMatch()) {
        e.preventDefault();
        confirmPassword.focus();
    }
});

document.getElementById('cus_image').addEventListener('change', function (evt) {
    const [file] = this.files;
    if (file) {
        document.getElementById('avatar-preview').src = URL.createObjectURL(file);
    }
});