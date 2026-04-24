function openBlockPopup(id) {
    document.getElementById('block-popup').classList.remove('hidden');
    document.getElementById('block-id').value = id;
}
function closeBlockPopup() {
    document.getElementById('block-popup').classList.add('hidden');
    document.getElementById('block-form').reset();
}
function openActivatePopup(id) {
    document.getElementById('activate-id').value = id;
    document.getElementById('activate-popup').classList.remove('hidden');
}
function closeActivatePopup() {
    document.getElementById('activate-popup').classList.add('hidden');
}
// Đóng popup khi bấm ra ngoài
document.getElementById('block-popup').addEventListener('click', function (e) {
    if (e.target === this) closeBlockPopup();
});
function toggleActionMenu(btn) {
    // Đóng tất cả menu khác
    document.querySelectorAll('.action-menu').forEach(m => m.classList.add('hidden'));
    const menu = btn.parentElement.querySelector('.action-menu');
    if (!menu) return;

    // Hiện menu tạm thời để đo vị trí thật
    menu.classList.remove('hidden');
    menu.classList.remove('bottom-full', 'mb-2', 'top-0', 'mt-2');
    const rect = menu.getBoundingClientRect();
    const spaceBottom = window.innerHeight - rect.bottom;
    // console.log('Space below menu:', spaceBottom);

    if (spaceBottom < 160) { // Nếu không đủ chỗ bên dưới (~menu height)
        menu.classList.add('bottom-full', 'mb-2');
        menu.classList.remove('top-0', 'mt-2');
    } else {
        menu.classList.add('top-0', 'mt-2');
        menu.classList.remove('bottom-full', 'mb-2');
    }

    // Nếu menu đã mở thì đóng, chưa mở thì mở
    if (menu.dataset.open === "true") {
        menu.classList.add('hidden');
        menu.dataset.open = "false";
    } else {
        menu.classList.remove('hidden');
        menu.dataset.open = "true";
    }

    // Đóng khi click ngoài
    document.addEventListener('click', function handler(e) {
        if (!btn.parentElement.contains(e.target)) {
            menu.classList.add('hidden');
            menu.dataset.open = "false";
            document.removeEventListener('click', handler);
        }
    });
}