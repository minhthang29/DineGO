// Display common notification
const box = document.querySelector('.notification-box');
if (box) {
    const progress = box.querySelector('.progress-bar');
    if (progress) {
        // Đặt width ban đầu là 0%, animate lên 100% trong 5s (trái sang phải)
        progress.style.width = '0%';
        progress.style.transition = 'width 2s linear, opacity 0.3s linear';
        setTimeout(() => {
            progress.style.width = '100%';
        }, 10);

        // Fade out progress bar ở cuối
        setTimeout(() => {
            progress.style.opacity = '0';
        }, 2800);
    }

    box.addEventListener('click', () => {
        box.classList.add('hide');
        setTimeout(() => box.remove(), 300);
    });

    setTimeout(() => {
        box.classList.add('hide');
        setTimeout(() => box.remove(), 300);
    }, 3000);
}