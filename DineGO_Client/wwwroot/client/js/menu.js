document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("menuForm");

    form.addEventListener("submit", function (e) {
        let isValid = true;

        // Clear previous errors
        document.getElementById("menuNameError").textContent = "";
        document.getElementById("menuTypeError").textContent = "";
        document.getElementById("imageFileError").textContent = "";

        // Validate name menu
        const menuName = document.getElementById("menuName").value.trim();
        if (menuName === "") {
            document.getElementById("menuNameError").textContent = "Tên menu không được để trống";
            isValid = false;
        } else if (menuName.length > 20) {
            document.getElementById("menuNameError").textContent = "Tên menu tối đa 20 ký tự";
            isValid = false;
        }

        // Validate menu
        const menuType = document.getElementById("menuType").value.trim();
        if (menuType === "") {
            document.getElementById("menuTypeError").textContent = "Loại menu không được để trống";
            isValid = false;
        } else if (menuType.length > 20) {
            document.getElementById("menuTypeError").textContent = "Loại menu tối đa 20 ký tự";
            isValid = false;
        }

        // Validate image
        const imageFile = document.getElementById("imageFile").files[0];
        if (!imageFile) {
            document.getElementById("imageFileError").textContent = "Vui lòng chọn ảnh menu";
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("editMenuForm");

    form.addEventListener("submit", function (e) {
        let isValid = true;

        // Reset error messages
        document.getElementById("menuNameError").textContent = "";
        document.getElementById("menuTypeError").textContent = "";
        document.getElementById("imageFileError").textContent = "";

        // Validate menu name
        const menuName = document.getElementById("menuName").value.trim();
        if (menuName === "") {
            document.getElementById("menuNameError").textContent = "Tên menu không được để trống";
            isValid = false;
        } else if (menuName.length > 20) {
            document.getElementById("menuNameError").textContent = "Tên menu tối đa 20 ký tự";
            isValid = false;
        }

        // Validate menu type
        const menuType = document.getElementById("menuType").value.trim();
        if (menuType === "") {
            document.getElementById("menuTypeError").textContent = "Loại menu không được để trống";
            isValid = false;
        } else if (menuType.length > 20) {
            document.getElementById("menuTypeError").textContent = "Loại menu tối đa 20 ký tự";
            isValid = false;
        }


        if (!isValid) {
            e.preventDefault(); 
        }
    });
});