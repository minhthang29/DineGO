//Popup_Form - Thang - Start
document.addEventListener("DOMContentLoaded", function () {
    let form = document.getElementById("dynamicForm");

    document.querySelectorAll(".open-dynamic-modal").forEach(button => {
        button.addEventListener("click", function () {
            let title = this.getAttribute("data-title");
            let fieldsAttr = this.getAttribute("data-fields");
            let action = this.getAttribute("data-action");
            let controller = this.getAttribute("data-controller");
            let method = this.getAttribute("data-method").toUpperCase(); // GET/POST
            let type = this.getAttribute("data-type") || "form"; // form | delete
            let id = this.getAttribute("data-id"); // dùng cho delete

            // Cấu hình form
            form.setAttribute("data-action", action);
            form.setAttribute("data-controller", controller);
            form.setAttribute("data-method", method);
            form.setAttribute("data-type", type);

            // Cập nhật tiêu đề popup
            document.getElementById("dynamicModalLabel").textContent = title;

            let formFieldsContainer = document.getElementById("formFields");
            formFieldsContainer.innerHTML = "";
            // Nếu là popup tạo chủ nhà hàng thì chèn điều khoản
            if (title === "Nhập thông tin chủ nhà hàng") {
                let termsDiv = document.createElement("div");
                termsDiv.className = "alert alert-info mb-3";
                termsDiv.innerHTML = `Khi tạo chủ nhà hàng, bạn đồng ý với các điều khoản sử dụng dịch vụ của DineGO.
                Vui lòng đọc kỹ <a href="/Term" target="_blank">điều khoản</a> trước khi xác nhận.`;
                formFieldsContainer.appendChild(termsDiv);
            }
            if (type === "delete") {
                formFieldsContainer.innerHTML = `
                    <input type="hidden" name="id" value="${id}" />
                    <p>Bạn có chắc chắn muốn xoá mục này không?</p>
                `;
            } else {
                let fields = fieldsAttr.split(",");
                fields.forEach(field => {
                    let [name, labelText] = field.includes("|") ? field.split("|") : [field, field];

                    let label = document.createElement("label");
                    label.textContent = labelText + ":";
                    label.classList.add("form-label");

                    let input = document.createElement("input");
                    input.type = name === "password" ? "password" : "text";
                    input.classList.add("form-control");
                    input.name = name;
                    input.id = name;

                    let div = document.createElement("div");
                    div.classList.add("mb-3");
                    div.appendChild(label);
                    div.appendChild(input);

                    formFieldsContainer.appendChild(div);
                });
            }

            let modal = new bootstrap.Modal(document.getElementById("dynamicModal"));
            modal.show();
        });
    });

    // Xử lý submit cho cả Create / Delete
    form.addEventListener("submit", function (event) {
        event.preventDefault();

        let formData = new FormData(this);
        let action = this.getAttribute("data-action");
        let controller = this.getAttribute("data-controller");
        let method = this.getAttribute("data-method");
        let type = this.getAttribute("data-type");

        let url = `/${controller}/${action}`;

        if (method === "GET") {
            let params = new URLSearchParams([...formData]).toString();
            window.location.href = `${url}?${params}`;
        } else {
            // Nếu là POST, PUT, DELETE...
            fetch(url, {
                method: method,
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                     showGlobalLoader();
                    if (data.redirectUrl) {
                        window.location.href = data.redirectUrl;
                        return;
                    }

                    const callbackName = form.getAttribute("data-success-callback");
                    if (callbackName && typeof window[callbackName] === "function") {
                        window[callbackName](); // Gọi callback nếu có
                        form.removeAttribute("data-success-callback"); // Reset

                        // ✅ Đóng modal tại đây để tránh chạy tiếp fallback bên dưới
                        const modalElement = document.getElementById("dynamicModal");
                        const modalInstance = bootstrap.Modal.getInstance(modalElement);
                        if (modalInstance) {
                            modalInstance.hide();
                        }

                        return; // ⛔ NGẮT tại đây, không tiếp tục chạy các xử lý khác!
                    }

                    // ⬇️ Fallback nếu không có callback
                    if (type === "form") {
                        location.reload();
                    } else {
                        console.log("POST Response:", data);
                    }

                    const modalElement = document.getElementById("dynamicModal");
                    const modalInstance = bootstrap.Modal.getInstance(modalElement);
                    if (modalInstance) {
                        modalInstance.hide();
                    }
                });

        }
    });
});
//Popup_Form - Thang - End
