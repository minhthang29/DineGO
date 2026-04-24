document.addEventListener("DOMContentLoaded", function () {

    // 👉 Hiển thị lỗi tổng quát
    function showError(message) {
        const errorContainer = document.getElementById("cart-errors");
        const errorList = document.getElementById("error-list");
        if (errorContainer && errorList) {
            errorList.innerHTML = `<li>${message}</li>`;
            errorContainer.classList.remove("d-none");
        }
    }

    function clearAllErrors() {
        const errorContainer = document.getElementById("cart-errors");
        if (errorContainer) {
            errorContainer.classList.add("d-none");
        }
    }

    // 👉 Check giờ đóng cửa nhà hàng
    function checkRestaurantClosingTime() {
        const limitsDiv = document.querySelector('.restaurant-limits');
        const closeTime = limitsDiv?.dataset.closeTime;
        const lastOrderMinutes = parseInt(limitsDiv?.dataset.lastOrderMinutes) || 60; // Thời gian đặt món cuối
        
        if (!closeTime) return false; // Không có thông tin giờ đóng cửa
        
        const now = new Date();
        const currentTime = now.getHours() * 60 + now.getMinutes(); // Chuyển về phút
        
        // Parse giờ đóng cửa (format: "HH:mm")
        const [closeHour, closeMinute] = closeTime.split(':').map(Number);
        const closeTimeInMinutes = closeHour * 60 + closeMinute;
        
        // Tính thời gian cuối cùng được đặt món (trước giờ đóng cửa 1 tiếng = 60 phút)
        const lastOrderTime = closeTimeInMinutes - lastOrderMinutes;
        
        return currentTime >= lastOrderTime;
    }
    // 👉 Hàm tính thời gian cuối cùng được đặt món
    function getLastOrderTimeText() {
        const limitsDiv = document.querySelector('.restaurant-limits');
        const closeTime = limitsDiv?.dataset.closeTime;
        const lastOrderMinutes = parseInt(limitsDiv?.dataset.lastOrderMinutes) || 60; // Thời gian đặt món cuối
        
        if (!closeTime) return "";
        
        const [closeHour, closeMinute] = closeTime.split(':').map(Number);
        const closeTimeInMinutes = closeHour * 60 + closeMinute;
        
        // Tính thời gian cuối cùng được đặt món (trước giờ đóng cửa 1 tiếng)
        const lastOrderTime = closeTimeInMinutes - lastOrderMinutes;

        // Chuyển đổi về giờ:phút
        const lastOrderHour = Math.floor(lastOrderTime / 60);
        const lastOrderMinute = lastOrderTime % 60;
        
        return `${lastOrderHour.toString().padStart(2, '0')}:${lastOrderMinute.toString().padStart(2, '0')}`;
    }
    // 👉 Hàm tính tổng cộng và check giới hạn
    function calculateGrandTotal() {
        let grand = 0;
        let hasError = false;
        let hasSelectedItems = false;
        let totalQuantity = 0;
        let selectedQuantity = 0;
        
        // Xóa tất cả lỗi cũ
        clearAllErrors();
        
        // Check giờ đóng cửa trước tiên
        if (checkRestaurantClosingTime()) {
            const limitsDiv = document.querySelector('.restaurant-limits');
            const closeTime = limitsDiv?.dataset.closeTime;
            const lastOrderTime = getLastOrderTimeText();
            showError(`Nhà hàng sẽ đóng cửa lúc ${closeTime}. Không thể đặt món sau ${lastOrderTime}`);
            hasError = true;
        }
        
        // Tính tổng tiền và số lượng từ các món được chọn
        document.querySelectorAll(".cart-checkbox:checked").forEach(cb => {
            hasSelectedItems = true;
            const row = cb.closest("tr");
            const totalCell = row.querySelector(".total-price");
            const quantityInput = row.querySelector(".quantity-input");
            const value = parseInt(totalCell.textContent.replace(/[^\d]/g, ""));
            const quantity = parseInt(quantityInput.value) || 1;
            
            if (!isNaN(value)) {
                grand += value;
                selectedQuantity += quantity;
            }
        });

        // Tính tổng số lượng tất cả món (cho check số lượng tối đa mỗi món)
        document.querySelectorAll(".quantity-input").forEach(input => {
            totalQuantity += parseInt(input.value) || 1;
        });

        // Lấy giới hạn từ restaurant
        const limitsDiv = document.querySelector('.restaurant-limits');
        const maxOrderPrice = parseInt(limitsDiv?.dataset.maxPrice) || 5000000; // Giá tối đa tổng đơn hàng
        const maxQuantityPerItem = parseInt(limitsDiv?.dataset.maxQuantity) || 50; // Số lượng tối đa mỗi món
        const globalMinOrder = parseInt(limitsDiv?.dataset.minPrice) || 200000; // Đơn hàng tối thiểu
        
        // Check các giới hạn (chỉ khi có món được chọn và chưa quá giờ)
        if (hasSelectedItems && !checkRestaurantClosingTime()) {
            // 1. Check đơn hàng tối thiểu
            if (grand < globalMinOrder) {
                showError(`Đơn hàng tối thiểu ${globalMinOrder.toLocaleString()} VND`);
                hasError = true;
            } 
            // 2. Check giá tối đa tổng đơn hàng
            else if (grand > maxOrderPrice) {
                showError(`Tổng giá trị đơn hàng vượt quá ${maxOrderPrice.toLocaleString()} VND`);
                hasError = true;
            }
        }

        // 3. Check số lượng tối đa mỗi món (áp dụng cho tất cả món, không phân biệt có được chọn hay không)
        let hasMaxQuantityError = false;
        document.querySelectorAll(".quantity-input").forEach(input => {
            const quantity = parseInt(input.value) || 1;
            if (quantity > maxQuantityPerItem) {
                hasMaxQuantityError = true;
            }
        });

        if (hasMaxQuantityError) {
            showError(`Số lượng mỗi món không được vượt quá ${maxQuantityPerItem}`);
            hasError = true;
        }

        // Update displays
        const grandTotal = document.getElementById("grandTotal");
        const subTotal = document.getElementById("subTotal");
        const totalItemsSpan = document.getElementById("totalItems");
        
        if (grandTotal) {
            grandTotal.textContent = grand.toLocaleString() + " VND";
        }
        if (subTotal) {
            subTotal.textContent = grand.toLocaleString() + " VND";
        }
        if (totalItemsSpan) {
            totalItemsSpan.textContent = `${selectedQuantity} món được chọn`;
        }

        // Enable/disable checkout button
        const checkoutBtn = document.querySelector("#checkoutForm button[type='submit']");
        if (checkoutBtn) {
            if (!hasSelectedItems || hasError) {
                checkoutBtn.disabled = true;
                checkoutBtn.classList.add("btn-secondary");
                checkoutBtn.classList.remove("btn-danger");
                
                // Thay đổi text theo tình huống
                if (!hasSelectedItems) {
                    checkoutBtn.innerHTML = '<i class="bi bi-cart-x"></i> Chọn món để mua hàng';
                } else if (checkRestaurantClosingTime()) {
                    checkoutBtn.innerHTML = '<i class="bi bi-clock"></i> Quá giờ đặt món';
                } else if (grand < globalMinOrder) {
                    checkoutBtn.innerHTML = `<i class="bi bi-exclamation-triangle"></i> Tối thiểu ${globalMinOrder.toLocaleString()}đ`;
                } else {
                    checkoutBtn.innerHTML = '<i class="bi bi-exclamation-triangle"></i> Vượt quá giới hạn';
                }
            } else {
                checkoutBtn.disabled = false;
                checkoutBtn.classList.add("btn-danger");
                checkoutBtn.classList.remove("btn-secondary");
                checkoutBtn.innerHTML = '<i class="bi bi-cart-check"></i> Mua hàng';
            }
        }
    }

    // 👉 Xử lý +/- với check giới hạn
    document.querySelectorAll('.quantity-form').forEach(form => {
        const minusBtn = form.querySelector('.minus-btn');
        const plusBtn = form.querySelector('.plus-btn');
        const input = form.querySelector('.quantity-input');
        const submitBtn = form.querySelector('.submit-btn');
        const priceCell = form.closest("tr").querySelector(".unit-price");
        const totalCell = form.closest("tr").querySelector(".total-price");
        const price = parseInt(priceCell.dataset.price);

        const updateTotal = () => {
            const quantity = parseInt(input.value) || 1;
            const total = price * quantity;
            totalCell.innerHTML = `<strong class="text-danger">${total.toLocaleString()} VND</strong>`;
            calculateGrandTotal();
        };

        plusBtn.addEventListener("click", () => {
            // Check giờ đóng cửa trước
            if (checkRestaurantClosingTime()) {
                const limitsDiv = document.querySelector('.restaurant-limits');
                const closeTime = limitsDiv?.dataset.closeTime;
                showError(`Nhà hàng sẽ đóng cửa lúc ${closeTime}. Không thể thay đổi số lượng`);
                return;
            }
            
            let value = parseInt(input.value);
            const limitsDiv = document.querySelector('.restaurant-limits');
            const maxQuantityPerItem = parseInt(limitsDiv?.dataset.maxQuantity) || 50;
            
            // Check số lượng tối đa cho món này
            if (value >= maxQuantityPerItem) {
                showError(`Số lượng mỗi món không được vượt quá ${maxQuantityPerItem}`);
                return;
            }
            
            input.value = value + 1;
            updateTotal();
            submitBtn.click();
        });

        minusBtn.addEventListener("click", () => {
            // Check giờ đóng cửa trước
            if (checkRestaurantClosingTime()) {
                const limitsDiv = document.querySelector('.restaurant-limits');
                const closeTime = limitsDiv?.dataset.closeTime;
                showError(`Nhà hàng sẽ đóng cửa lúc ${closeTime}. Không thể thay đổi số lượng`);
                return;
            }
            
            let value = parseInt(input.value);
            if (value > 1) {
                input.value = value - 1;
                updateTotal();
                submitBtn.click();
            }
        });

        // Check khi user nhập trực tiếp
        input.addEventListener('change', () => {
            // Check giờ đóng cửa trước
            if (checkRestaurantClosingTime()) {
                const limitsDiv = document.querySelector('.restaurant-limits');
                const closeTime = limitsDiv?.dataset.closeTime;
                showError(`Nhà hàng sẽ đóng cửa lúc ${closeTime}. Không thể thay đổi số lượng`);
                return;
            }
            
            const limitsDiv = document.querySelector('.restaurant-limits');
            const maxQuantityPerItem = parseInt(limitsDiv?.dataset.maxQuantity) || 50;
            let value = parseInt(input.value) || 1;
            
            if (value < 1) {
                input.value = 1;
            } else if (value > maxQuantityPerItem) {
                input.value = maxQuantityPerItem;
                showError(`Số lượng mỗi món không được vượt quá ${maxQuantityPerItem}`);
            }
            
            updateTotal();
        });
    });

    // 👉 Disable thao tác khi quá giờ
    function disableCartActions() {
        if (checkRestaurantClosingTime()) {
            // Disable tất cả checkbox
            document.querySelectorAll('.cart-checkbox, .selectAllGroup').forEach(cb => {
                cb.disabled = true;
            });
            
            // Disable tất cả quantity buttons
            document.querySelectorAll('.plus-btn, .minus-btn, .quantity-input').forEach(btn => {
                btn.disabled = true;
            });
        }
    }

    // 👉 Xử lý checkbox (giữ nguyên)
    const globalCheck = document.getElementById('selectAllGlobal');
    const groupChecks = document.querySelectorAll('.selectAllGroup');
    const itemChecks = document.querySelectorAll('.cart-checkbox');

    // Checkbox "Chọn tất cả" global
    globalCheck?.addEventListener('change', function () {
        if (checkRestaurantClosingTime()) return;
        
        const checked = this.checked;
        groupChecks.forEach(g => g.checked = checked);
        itemChecks.forEach(i => i.checked = checked);
        calculateGrandTotal();
    });

    // Checkbox "Chọn tất cả" theo nhóm
    groupChecks.forEach(groupBox => {
        groupBox.addEventListener('change', function () {
            if (checkRestaurantClosingTime()) return;
            
            const groupId = this.dataset.group;
            const items = document.querySelectorAll(`.cart-checkbox[data-group="${groupId}"]`);
            items.forEach(i => i.checked = this.checked);
            
            updateGlobalCheckbox();
            calculateGrandTotal();
        });
    });

    // Checkbox từng item
    itemChecks.forEach(cb => {
        cb.addEventListener('change', function () {
            if (checkRestaurantClosingTime()) return;
            
            const groupId = this.dataset.group;
            
            updateGroupCheckbox(groupId);
            updateGlobalCheckbox();
            calculateGrandTotal();
        });
    });

    // Hàm update group checkbox
    function updateGroupCheckbox(groupId) {
        const groupItems = document.querySelectorAll(`.cart-checkbox[data-group="${groupId}"]`);
        const groupChecked = [...groupItems].every(i => i.checked);
        const groupBox = document.querySelector(`.selectAllGroup[data-group="${groupId}"]`);
        if (groupBox) {
            groupBox.checked = groupChecked;
        }
    }

    // Hàm update global checkbox
    function updateGlobalCheckbox() {
        const allChecked = [...itemChecks].every(i => i.checked);
        const anyChecked = [...itemChecks].some(i => i.checked);
        
        if (globalCheck) {
            globalCheck.checked = allChecked;
            globalCheck.indeterminate = !allChecked && anyChecked;
        }
    }

    // 👉 Khởi tạo
    calculateGrandTotal();
    disableCartActions();
    
    // Check mỗi phút để update trạng thái
    setInterval(() => {
        calculateGrandTotal();
        disableCartActions();
    }, 60000); // 60 giây
});