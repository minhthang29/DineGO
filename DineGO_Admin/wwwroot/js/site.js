function toggleSubMenu(id, btn) {
    const submenu = document.getElementById(id);
    submenu.classList.toggle('hidden');
    // Xoay icon mũi tên
    const icon = btn.querySelector('.fa-chevron-down');
    icon.classList.toggle('rotate-180');
}
