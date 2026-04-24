document.getElementById('quick-feature-search').addEventListener('input', function () {
    const keyword = this.value.toLowerCase();
    // Lấy tất cả các menu cha (nav > a, nav > div > button)
    const menuItems = document.querySelectorAll('aside nav > *');
    menuItems.forEach(item => {
        // Nếu là link menu chính
        if (item.tagName === 'A') {
            const text = item.textContent.toLowerCase();
            if (keyword === "" || text.includes(keyword)) {
                item.classList.remove('hidden');
            } else {
                item.classList.add('hidden');
            }
        }
        // Nếu là nhóm có submenu (div)
        else if (item.tagName === 'DIV') {
            const button = item.querySelector('button');
            const submenu = item.querySelector('div');
            let hasMatch = false;

            // Kiểm tra button cha
            if (button) {
                const text = button.textContent.toLowerCase();
                if (keyword === "" || text.includes(keyword)) {
                    button.classList.remove('hidden');
                    hasMatch = true;
                } else {
                    button.classList.add('hidden');
                }
            }

            // Kiểm tra các submenu
            if (submenu) {
                const links = submenu.querySelectorAll('a');
                let submenuHasMatch = false;
                links.forEach(link => {
                    const linkText = link.textContent.toLowerCase();
                    if (keyword === "" || linkText.includes(keyword)) {
                        link.classList.remove('hidden');
                        submenuHasMatch = true;
                    } else {
                        link.classList.add('hidden');
                    }
                });
                // Nếu có submenu khớp thì hiện button cha và submenu
                if (submenuHasMatch) {
                    if (button) button.classList.remove('hidden');
                    submenu.classList.remove('hidden');
                    hasMatch = true;
                } else {
                    submenu.classList.add('hidden');
                }
            }

            // Nếu không có gì khớp thì ẩn cả nhóm
            if (!hasMatch) {
                item.classList.add('hidden');
            } else {
                item.classList.remove('hidden');
            }
        }
    });
});

const searchInput = document.getElementById('quick-feature-search');
const clearBtn = document.getElementById('clear-feature-search');

searchInput.addEventListener('input', function () {
    clearBtn.style.display = this.value ? 'block' : 'none';
});

clearBtn.addEventListener('click', function () {
    searchInput.value = '';
    searchInput.dispatchEvent(new Event('input'));
    searchInput.focus();
});