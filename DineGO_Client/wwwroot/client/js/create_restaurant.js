
//Create/ Update restaurant in resowner page _ Thang _ Start
function setupImagePreview(inputId, addBtnId, previewImgId, removeBtnId) {
    const input = document.getElementById(inputId);
    const addBtn = document.getElementById(addBtnId);
    const previewImg = document.getElementById(previewImgId);
    const removeBtn = document.getElementById(removeBtnId);
    const imageBox = document.getElementById("imageBoxContainer");

    // Click container => mở file input nếu chưa có ảnh
    if (imageBox) {
        imageBox.addEventListener("click", () => {
            if (previewImg.style.display === "none") {
                input.click();
            }
        });
    }

    // Khi chọn ảnh
    input.addEventListener("change", function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                previewImg.src = e.target.result;
                previewImg.style.display = "block";
                if (removeBtn) removeBtn.style.display = "block";
                if (addBtn) addBtn.style.display = "none";
            };
            reader.readAsDataURL(this.files[0]);
        }
    });

    // Khi bấm xoá ảnh
    if (removeBtn) {
        removeBtn.addEventListener("click", function (e) {
            e.stopPropagation(); // tránh trigger click container
            previewImg.src = "";
            previewImg.style.display = "none";
            removeBtn.style.display = "none";
            if (addBtn) addBtn.style.display = "block";
            input.value = "";
        });
    }
}

document.addEventListener("DOMContentLoaded", function () {
    setupImagePreview(
        "restaurantImageInput",
        "addRestaurantImageBtn",
        "restaurantPreviewImg",
        "removeRestaurantImageBtn"
    );
});
//Create restaurant in resowner page _ Thang _ End