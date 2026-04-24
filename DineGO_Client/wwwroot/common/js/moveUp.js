// Hiện nút khi cuộn xuống
window.addEventListener('scroll', function () {
    var btn = document.getElementById('scrollToTopBtn');
    if (window.scrollY > 200) {
        btn.style.display = 'block';
    } else {
        btn.style.display = 'none';
    }
});
// Cuộn lên đầu trang khi nhấn nút
document.getElementById('scrollToTopBtn').onclick = function () {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};