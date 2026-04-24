// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('.owl-carousel').owlCarousel({
        loop: false,
        margin: 10,
        nav: true,
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            },
            1400: {
                items: 4
            }
        }
    })
})

//Product - Start - Thang

const radios = document.querySelectorAll('input[type="radio"]');
const regionButton = document.querySelector('.region-display');

// Lắng nghe sự kiện thay đổi radio
radios.forEach((radio) => {
    radio.addEventListener('change', () => {
        // Cập nhật nội dung nút dựa trên nhãn của radio được chọn
        regionButton.textContent = document.querySelector(`label[for="${radio.id}"]`).textContent;
    });
});

const rangeInput = document.querySelectorAll(".range-input input"),
    priceInput = document.querySelectorAll(".price-input input"),
    range = document.querySelector(".slider .progress");
let priceGap = 1000;

priceInput.forEach(input => {
    input.addEventListener("input", e => {
        let minPrice = parseInt(priceInput[0].value),
            maxPrice = parseInt(priceInput[1].value);

        if ((maxPrice - minPrice >= priceGap) && maxPrice <= rangeInput[1].max) {
            if (e.target.className === "input-min") {
                rangeInput[0].value = minPrice;
                range.style.left = ((minPrice / rangeInput[0].max) * 100) + "%";
            } else {
                rangeInput[1].value = maxPrice;
                range.style.right = 100 - (maxPrice / rangeInput[1].max) * 100 + "%";
            }
        }
    });
});

rangeInput.forEach(input => {
    input.addEventListener("input", e => {
        let minVal = parseInt(rangeInput[0].value),
            maxVal = parseInt(rangeInput[1].value);

        if ((maxVal - minVal) < priceGap) {
            if (e.target.className === "range-min") {
                rangeInput[0].value = maxVal - priceGap
            } else {
                rangeInput[1].value = minVal + priceGap;
            }
        } else {
            priceInput[0].value = minVal;
            priceInput[1].value = maxVal;
            range.style.left = ((minVal / rangeInput[0].max) * 100) + "%";
            range.style.right = 100 - (maxVal / rangeInput[1].max) * 100 + "%";
        }
    });
});

//Product - End - Thang


//Profile_Header _ Thang _ Start
$(document).ready(function () {
    const profileContainer = document.getElementById("profileContainer");
    const profileMenu = document.getElementById("profileMenu");

    // Only enable hover dropdown on desktop
    function enableProfileHoverIfDesktop() {
        if (!profileContainer || !profileMenu) return;

        // Clean previous handlers by cloning (simple, avoids duplicates)
        const clone = profileContainer.cloneNode(true);
        profileContainer.parentNode.replaceChild(clone, profileContainer);

        const container = document.getElementById("profileContainer");
        const menu = document.getElementById("profileMenu");

        if (window.matchMedia('(min-width: 992px)').matches) {
            let timeout;
            container.addEventListener("mouseenter", function () {
                clearTimeout(timeout);
                menu.style.display = "block";
            });
            container.addEventListener("mouseleave", function () {
                timeout = setTimeout(function () {
                    menu.style.display = "none";
                }, 200);
            });
            menu.addEventListener("mouseenter", function () {
                clearTimeout(timeout);
            });
            menu.addEventListener("mouseleave", function () {
                timeout = setTimeout(function () {
                    menu.style.display = "none";
                }, 200);
            });
        } else {
            // On mobile: ensure hidden
            if (menu) menu.style.display = 'none';
            // On mobile: clicking profile area navigates to Profile page
            container.addEventListener('click', function () {
                var link = document.querySelector('#profileMenu a[href*="/Customer/Profile"], #profileMenu a[href*="Customer/Profile"]');
                if (link && link.getAttribute('href')) {
                    window.location.href = link.getAttribute('href');
                }
            });
        }
    }

    enableProfileHoverIfDesktop();
    window.addEventListener('resize', enableProfileHoverIfDesktop);
});
//Add js _ Phuonghh _ Start
function toggleDropdown() {
    let dropdown = document.getElementById("profileDropdown");
    let arrow = document.querySelector(".dropdown-arrow");
    dropdown.style.display = dropdown.style.display === "block" ? "none" : "block";
    arrow.style.transform = dropdown.style.display === "block" ? "rotate(180deg)" : "rotate(0deg)";
}

// Ẩn dropdown khi click ra ngoài
document.addEventListener("click", function (event) {
  const profileInfo = document.querySelector(".profile-info");
  const dropdown = document.getElementById("profileDropdown");

  // ⚠️ Nếu không tồn tại thì bỏ qua
  if (!profileInfo || !dropdown) return;

  if (!profileInfo.contains(event.target)) {
    dropdown.style.display = "none";
    const arrow = document.querySelector(".dropdown-arrow");
    if (arrow) arrow.style.transform = "rotate(0deg)";
  }
});

//Add js _ Phuonghh _ End
//Profile_Header _ Thang _ End


//Image_Profile _ Thang _ Start
function previewImage(event) {
    const profilePic = document.querySelector(".profile-pic");
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            profilePic.style.backgroundImage = `url(${e.target.result})`;
            profilePic.textContent = "";
        };
        reader.readAsDataURL(file);
    }
}
//Image_Profile _ Thang _ End

//SearchByLocation _ Thang _ Start
$(document).ready(function () {
    const nameInput = $('.search-name');
    const addressSelect = $('.search-address');
    const resultContainer = $('.search-results');

    // Chỉ chạy nếu các phần tử tồn tại
    if (addressSelect.length && resultContainer.length) {
        let timer;

        addressSelect.on('change', function () {
            clearTimeout(timer);
            timer = setTimeout(function () {
                const name = ''; // tên bỏ trống, chỉ lọc theo khu vực
                const address = addressSelect.val();

                $.ajax({
                    url: '/Restaurant/SearchByLocation',
                    method: 'GET',
                    data: { name: name, address: address },
                    success: function (result) {
                        resultContainer.html(result);
                    },
                    error: function (xhr, status, err) {
                        console.error("AJAX Error:", err);
                    }
                });
            }, 200);
        });
    }
});
//SearchByLocation _ Thang _ End

//ListSearchSuggest _ Thang _ Start
document.addEventListener('DOMContentLoaded', function () {
    const restaurants = JSON.parse(document.getElementById('restaurantData').dataset.restaurants);

    const input = document.querySelector('.search-name');
    const suggestions = document.getElementById('autocompleteSuggestions');

    input.addEventListener('input', function () {
        const query = this.value.trim().toLowerCase();
        suggestions.innerHTML = '';

        if (query === '') {
            suggestions.style.display = 'none';
            return;
        }

        const matched = restaurants.filter(r => r.name.toLowerCase().includes(query));

        const ul = document.createElement('ul');
        ul.classList.add('suggestion-list');

        if (matched.length === 0) {
            // 👉 Nếu không tìm thấy: chỉ hiển thị 1 li "Không tìm thấy"
            const li = document.createElement('li');
            li.textContent = "Không tìm thấy nhà hàng nào phù hợp.";
            li.style.color = '#888';
            li.style.padding = '8px 16px';
            li.style.cursor = 'default';
            ul.appendChild(li);
        } else {
            // 👉 Nếu có: hiển thị list như cũ
            matched.forEach(r => {
                const li = document.createElement('li');
                li.innerHTML = `
                    <a href="/Restaurant/Details/${r.id}" style="display: flex; align-items: center; justify-content: space-between;">
                        <div style="display: flex; align-items: center; gap: 10px;">
                            <img src="${r.image}" alt="Ảnh" style="width: 40px; height: 40px; object-fit: cover; border-radius: 4px;">
                            <span>${r.name}</span>
                        </div>
                        <span style="color: #888;">${r.address}</span>
                    </a>
                `;
                ul.appendChild(li);
            });
        }

        suggestions.appendChild(ul);
        suggestions.style.display = 'block';
    });

    document.addEventListener('click', function (e) {
        if (!input.contains(e.target) && !suggestions.contains(e.target)) {
            suggestions.style.display = 'none';
        }
    });
});
//ListSearchSuggest _ Thang _ End


//Button 'X' clear content in input search_ Thang _ Start
const nameInput = document.querySelector('.search-name');
const clearBtn = document.getElementById('clearInput');

// Hiện/ẩn nút X khi có text
nameInput.addEventListener('input', () => {
    if (nameInput.value.length > 0) {
        clearBtn.style.display = 'block';
    } else {
        clearBtn.style.display = 'none';
    }
});

// Click X để xóa
clearBtn.addEventListener('click', () => {
    nameInput.value = '';
    clearBtn.style.display = 'none';
    // Cũng ẩn autocomplete nếu có
    const suggestions = document.getElementById('autocompleteSuggestions');
    suggestions.innerHTML = '';
    suggestions.style.display = 'none';
    nameInput.focus();
});
//Button 'X' clear content in input search_ Thang _ End


//Comment & Like in Post _ Thang _ Start
function toggleComments(el) {
    const comments = el.closest('.post').querySelector('.comments');
    comments.style.display = comments.style.display === 'none' ? 'none' : 'block';
}

function likePost(el) {
    el.textContent = el.textContent.includes('Liked') ? '👍 Like' : '👍 Liked';
}
//Comment & Like in Post _ Thang _ End

//Active when click in Res & ResOwner Panel _ Thang _ Start
function toggleSubMenu(id) {
        const submenu = document.getElementById(id);
        submenu.style.display = submenu.style.display === 'block' ? 'none' : 'block';
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll('.menu-item').forEach(item => {
            item.addEventListener('click', function () {
                document.querySelectorAll('.menu-item').forEach(i => i.classList.remove('active'));
                this.classList.add('active');
            });
        });
    });
//Active when click in ResOwner Panel _ Thang _ End

//Loading
function showGlobalLoader() {
  document.body.classList.add("loading");
}

function hideGlobalLoader() {
  document.body.classList.remove("loading");
}
