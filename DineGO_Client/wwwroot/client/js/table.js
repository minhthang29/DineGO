// tableManager.js

function setupManageTablePage() {
    const areaSelector = document.getElementById("areaSelector");
    const layout = document.getElementById("table-layout");
    new Choices('#areaSelector', { searchEnabled: false, itemSelectText: '' });
    areaSelector.addEventListener("change", loadTablesForManage);
    loadTablesForManage();
}

function loadTablesForManage() {
    const areaId = document.getElementById("areaSelector").value;
    fetch(`/Table/GetTables?area_id=${areaId}`)
        .then(res => res.json())
        .then(data => {
            const layout = document.getElementById("table-layout");
            const noTableMessage = document.getElementById("noTableMessage");
            layout.innerHTML = '';

            if (!data || data.length === 0) {
                noTableMessage?.classList.remove("d-none");
                return;
            }

            noTableMessage?.classList.add("d-none");

            data.forEach(t => {
                const table = document.createElement("div");
                table.className = "table-item";

                const nameDiv = document.createElement("div");
                nameDiv.className = "table-name";
                nameDiv.textContent = t.label;

                const seatDiv = document.createElement("div");
                seatDiv.className = "table-seat text-muted";
                seatDiv.textContent = `${t.type} người`;

                table.appendChild(nameDiv);
                table.appendChild(seatDiv);

                table.addEventListener("click", () => {
                    if (t.status !== 0) {
                        showNotification(`Bàn "${t.label}" hiện tại đang được sử dụng!`, "danger");
                        return;
                    }

                    setupTableModal(t);
                });

                layout.appendChild(table);
            });
        });
}

function setupReservationPage() {
    const areaSelector = document.getElementById("areaSelector");
    const statusFilter = document.getElementById("statusFilter");
    const dateSelector = document.getElementById("dateSelector");

    new Choices('#areaSelector', { searchEnabled: false, itemSelectText: '' });
    new Choices('#statusFilter', { searchEnabled: false, itemSelectText: '' });

    areaSelector.addEventListener("change", loadTablesForReservation);
    statusFilter.addEventListener("change", loadTablesForReservation);
    dateSelector.addEventListener("change", loadTablesForReservation);

    loadTablesForReservation();
}
function loadTablesForReservation() {
    const areaId = document.getElementById("areaSelector").value;
    const statusFilter = parseInt(document.getElementById("statusFilter").value);
    const date = document.getElementById("dateSelector").value;
    const slotContainer = document.getElementById("ownerSlotContainer");
    slotContainer.innerHTML = ""; // clear slot khi load lại

    fetch(`/Table/GetTables?area_id=${areaId}&date=${date}`)
        .then(res => res.json())
        .then(data => {
            const layout = document.getElementById("table-layout");
            const noTableMessage = document.getElementById("noTableMessage");
            layout.innerHTML = '';

            const filteredData = (statusFilter === -1)
                ? data
                : data.filter(t => t.status === statusFilter);

            if (!filteredData || filteredData.length === 0) {
                noTableMessage.classList.remove("d-none");
                return;
            }

            noTableMessage.classList.add("d-none");

            filteredData.forEach(t => {
                const nameDiv = document.createElement("div");
                nameDiv.className = "table-name";
                nameDiv.textContent = t.label;

                const seatDiv = document.createElement("div");
                seatDiv.className = "table-seat text-muted";
                seatDiv.textContent = `${t.type} người`;

                const table = document.createElement("div");
                table.className = "table-item";
                table.setAttribute("data-table-id", t.id);
                table.appendChild(nameDiv);
                table.appendChild(seatDiv);

                // Khi click vào bàn, load slot còn trống và cho phép chọn slot
                table.addEventListener("click", () => {
                    document.querySelectorAll(".table-item").forEach(item => item.classList.remove("selected"));
                    table.classList.add("selected");

                    fetchAllSlots(t.id, date, slotContainer);
                });

                // phải append trong vòng forEach
                layout.appendChild(table);
            });
        });
}

async function loadReservedTimesToday() {
    const container = document.getElementById("todayReservationList");
    container.innerHTML = `<div class="text-muted">Đang tải...</div>`;

    try {
        const response = await fetch(`/Reservation/GetReservedTimesToday`);
        const data = await response.json();

        if (!data || data.length === 0) {
            container.innerHTML = `<div class="text-success">Chưa có đơn đặt nào hôm nay.</div>`;
            return;
        }

        // Map trạng thái sang class bootstrap (giống fetchAllSlots)
        const statusClassMap = {
            0: "btn-warning",   // Chờ xác nhận
            1: "btn-danger",    // Đã nhận
            2: "btn-secondary", // Đã hủy
            3: "btn-primary",   // Đã đến
            4: "btn-dark",      // Không đến
            5: "btn-info"       // Hoàn tất
        };

        // Render danh sách đơn đặt bàn hôm nay dưới dạng button
        container.innerHTML = data.map(r => {
            const btnClass = statusClassMap[r.reser_status] || "btn-light";
            return `
                <button type="button" 
                        class="btn ${btnClass} m-1 today-res-btn" 
                        data-reser-id="${r.reser_id}" 
                        data-status="${r.reser_status}" 
                        data-time="${r.time}">
                    ${r.table_name} - ${r.time}
                </button>
            `;
        }).join("");

        // Event click -> mở chi tiết reservation
        document.querySelectorAll(".today-res-btn").forEach(btn => {
            btn.addEventListener("click", () => {
                const reserId = btn.dataset.reserId;
                if (!reserId || reserId === "0") {
                    alert("Không tìm thấy thông tin đặt bàn.");
                    return;
                }
                getReservationDetail(reserId); // ✅ dùng lại hàm bạn đã có
            });
        });

    } catch (err) {
        console.error("❌ Lỗi loadReservedTimesToday:", err);
        container.innerHTML = `<div class="text-danger">Không tải được dữ liệu</div>`;
    }
}


async function fetchAllSlots(tableId, date, slotContainer) {
    slotContainer.innerHTML = `<div class="text-muted">Đang tải...</div>`;

    try {
        const response = await fetch(`/Reservation/GetAllSlots?tableId=${tableId}&date=${date}`);
        const slots = await response.json();

        if (!slots || slots.length === 0) {
            slotContainer.innerHTML = `<div class="text-danger">Không có khung giờ nào.</div>`;
            return;
        }

        // Map trạng thái sang class bootstrap (slot available = xanh lá)
        const statusClassMap = {
            pending: "btn-warning",      // vàng
            accepted: "btn-danger",      // đỏ
            show: "btn-primary",         // xanh dương
            available: "btn-success",    // ✅ xanh lá cho slot trống
            "no-show": "btn-success",    // xanh lá
            canceled: "btn-success",     // xanh lá
            completed: "btn-info",       // xám nhạt
            blocked: "btn-dark disabled" // xám đậm + disable
        };

        slotContainer.innerHTML = slots
            .map(s => {
                const btnClass = statusClassMap[s.status] || "btn-secondary";
                return `<button type="button" 
                        class="btn ${btnClass} m-1 slot-btn ${s.status}" 
                        data-time="${s.time}" 
                        data-table-id="${tableId}"
                        data-reser-id="${s.reserId || 0}" 
                        data-status="${s.status}"
                        ${s.status === "blocked" ? "disabled" : ""}>
                        ${s.time}
                    </button>`;
            }).join("");

        // Event cho slot available: Mở modal tạo walk-in reservation (thay vì chỉ select)
        document.querySelectorAll(".slot-btn.available").forEach(btn => {
            btn.addEventListener("click", () => {
                document.querySelectorAll(".slot-btn").forEach(b => b.classList.remove("active"));
                btn.classList.add("active");
                const time = btn.dataset.time;
                createWalkInReservation(tableId, date, time); // ✅ Tạo reservation walk-in
            });
        });

        // Event cho slot có reservation (pending, accepted, show, …) - giữ nguyên
        document.querySelectorAll(".slot-btn:not(.available)").forEach(btn => {
            const status = btn.dataset.status;
            if (status === "canceled" || status === "no-show" || status === "blocked") {
                return; // bỏ qua
            }

            btn.addEventListener("click", () => {
                const reserId = btn.dataset.reserId;
                if (!reserId || reserId === "0") {
                    alert("Không tìm thấy thông tin đặt bàn.");
                    return;
                }
                getReservationDetail(reserId);
            });
        });

    } catch (err) {
        console.error("Lỗi load slot:", err);
        slotContainer.innerHTML = `<div class="text-danger">Lỗi tải giờ trống</div>`;
    }
}
// Function tạo modal (đơn giản: chỉ note)
async function createWalkInReservation(tableId, date, time) {
    const modalId = "walkInModal";
    
    // Xóa modal cũ nếu tồn tại
    const existingModal = document.getElementById(modalId);
    if (existingModal) existingModal.remove();

    const modalElement = document.createElement("div");
    modalElement.className = "modal fade";
    modalElement.id = modalId;
    modalElement.tabIndex = -1;
    modalElement.setAttribute("aria-hidden", "true");
    modalElement.setAttribute("aria-labelledby", `${modalId}Label`);
    modalElement.innerHTML = `
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="${modalId}Label">Tạo đặt bàn cho khách</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label for="walkInNote" class="form-label">Ghi chú (tùy chọn, ví dụ: Tên: ..., SĐT: ...):</label>
                        <textarea class="form-control" id="walkInNote" rows="3" placeholder="Nhà hàng đặt - Tên: Test, SĐT: 0123456789"></textarea>
                    </div>
                    <input type="hidden" id="walkInTableId" value="${tableId}">
                    <input type="hidden" id="walkInDate" value="${date}">
                    <input type="hidden" id="walkInTime" value="${time}">
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                    <button type="button" class="btn btn-success" id="saveWalkInBtn">Tạo (Đã đặt)</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modalElement);
    
    // Set data
    document.getElementById("walkInTableId").value = tableId;
    document.getElementById("walkInDate").value = date;
    document.getElementById("walkInTime").value = time;
    document.getElementById("walkInNote").value = "";

    setTimeout(() => {
        const bsModal = new bootstrap.Modal(modalElement);
        bsModal.show();
        
        const saveBtn = document.getElementById("saveWalkInBtn");
        if (saveBtn) saveBtn.addEventListener("click", saveWalkInReservation);
        
        modalElement.addEventListener("hidden.bs.modal", () => modalElement.remove());
    }, 10);
}

async function saveWalkInReservation() {
    const note = document.getElementById("walkInNote")?.value?.trim();
    const tableId = document.getElementById("walkInTableId")?.value;
    const date = document.getElementById("walkInDate")?.value;
    const time = document.getElementById("walkInTime")?.value;

    if (!tableId || !date || !time) {
        alert("Lỗi dữ liệu.");
        return;
    }

    const saveBtn = document.getElementById("saveWalkInBtn");
    if (saveBtn) saveBtn.disabled = true;

    // FormData đơn giản (không cần customer)
    const formData = new FormData();
    formData.append("table_id", tableId);
    formData.append("res_id", "1");  // Adjust nếu cần (fixed hoặc từ session)
    formData.append("reser_date_date", date);
    formData.append("reser_date_time", time);
    formData.append("isAdminMode", "true");  // Trigger admin
    formData.append("adminNote", note || "");  // Note tùy chọn

    try {
        const response = await fetch("/Reservation/CreateReservation", {
            method: "POST",
            body: formData
        });

        const result = await response.json();
        if (result.success) {
            showNotification(result.message || "Tạo đặt bàn thành công!", "success");
            
            const modalEl = document.getElementById("walkInModal");
            if (modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
            }

            // Reload UI
            const selectedTable = document.querySelector(".table-item.selected");
            if (selectedTable) {
                const tableIdNum = selectedTable.dataset.tableId;
                const dateVal = document.getElementById("dateSelector")?.value;
                const slotContainer = document.getElementById("ownerSlotContainer");
                if (dateVal && slotContainer) {
                    fetchAllSlots(tableIdNum, dateVal, slotContainer);
                }
            }
            if (typeof loadReservedTimesToday === "function") loadReservedTimesToday();
        } else {
            alert("❌ " + (result.message || "Lỗi tạo đặt bàn."));
        }
    } catch (err) {
        console.error("Lỗi:", err);
        alert("Không thể tạo đặt bàn.");
    } finally {
        if (saveBtn) saveBtn.disabled = false;
    }
}

// Hàm lấy chi tiết reservation và hiển thị vào modal
function getReservationDetail(reserId) {
    const footer = document.getElementById("reservationModalFooter");
    footer.innerHTML = ''; 
    fetch(`/Reservation/GetReservationInfo/${reserId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Không tìm thấy reservation với ID " + reserId);
            }
            return response.json();
        })
        .then(reservation => {

            // Gán dữ liệu vào modal
            document.getElementById("customerName").innerText = reservation.customer?.cus_name || "N/A";
            document.getElementById("customerPhone").innerText = reservation.customer?.cus_phone || "N/A";
            document.getElementById("tableName").innerText = reservation.table?.table_name || "N/A";
            document.getElementById("reservationDate").innerText = reservation.reser_date
                ? new Date(reservation.reser_date).toLocaleString("vi-VN")
                : "N/A";
            document.getElementById("reservationNote").innerText = reservation.reser_note || "-";
            const statusInfo = getStatusInfo(reservation.reser_status);
            const statusEl = document.getElementById("reservationStatus");
            statusEl.innerText = statusInfo.text;
            statusEl.className = statusInfo.class;

            // Tính toán enable/disable
            let now = new Date();
            let reserDate = reservation.reser_date ? new Date(reservation.reser_date) : null;

            let allowShow = false;
            let allowNoShow = false;

            if (reserDate) {
                // ✅ Show: sau giờ đặt bàn
                allowShow = now >= reserDate;

                // 🚫 No-show: sau giờ đặt bàn + 30 phút
                let threshold = new Date(reserDate.getTime() + 30 * 60000);
                allowNoShow = now >= threshold;
            }

            if (reservation.reser_status === 3) {
                // Nếu đã có mặt -> chỉ hiện nút Hoàn thành
                footer.innerHTML = `
        <button class="btn btn-primary"
            onclick="updateReserStatus(${reservation.reser_id}, 5)">
            🎉 Hoàn thành
        </button>
    `;
            } else if (reservation.reser_status === 1) {
                // Mặc định: hiện 2 nút Có mặt & Không có mặt
                footer.innerHTML = `
        <button class="btn ${allowShow ? "btn-success" : "btn-secondary"} me-2"
            ${allowShow ? `onclick="updateReserStatus(${reservation.reser_id}, 3)"` : "disabled"}>
            ✅ Có mặt
        </button>
        <button class="btn ${allowNoShow ? "btn-warning" : "btn-secondary"}"
            ${allowNoShow ? `onclick="updateReserStatus(${reservation.reser_id}, 4)"` : "disabled"}>
            🚫 Không có mặt
        </button>
    `;
            }

            // Mở modal
            const modal = new bootstrap.Modal(document.getElementById("reservationDetailModal"));
            modal.show();
        })
        .catch(error => {
            console.error("Lỗi khi load reservation:", error);
            alert("Không thể tải thông tin đặt bàn.");
        });
}

function getStatusInfo(status) {
    switch (status) {
        case 0: return { text: "Chờ thanh toán", class: "text-warning" };
        case 1: return { text: "Đã đặt", class: "text-danger" };
        case 3: return { text: "Đang dùng bữa", class: "text-primary" };
        case 4: return { text: "Không có mặt", class: "text-dark" };
        case 5: return { text: "Hoàn thành", class: "text-info" };
        default: return { text: "Không xác định", class: "text-muted" };
    }
}

async function updateReserStatus(reserId, reserStatus) {
    try {
        const response = await fetch(`/Reservation/UpdateReserStatus?reser_id=${reserId}&reser_status=${reserStatus}`, {
            method: "PUT"
        });

        const result = await response.json();
        if (result.success) {
            showNotification("Cập nhật trạng thái thành công.", "success");

            // Đóng modal
            const modalEl = document.getElementById("reservationDetailModal");
            const modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();

            // Reload lại UI (nếu có hàm load)
            if (typeof refreshReservationUI === "function") {
                refreshReservationUI();
            }
            setTimeout(() => {
                location.reload();
            }, 800);
        } else {
            alert("❌ " + result.message);
        }
    } catch (error) {
        console.error("Lỗi updateReserStatus:", error);
        alert("Không thể cập nhật trạng thái đặt bàn.");
    }
}





// //Update table status _ staff
// function setupManageTableModal(t) {
//     // --- 1. Set giá trị ban đầu vào modal ---
//     document.getElementById("detailTableId").value = t.id;
//     document.getElementById("detailTableLabel").value = t.label;
//     document.getElementById("detailTableSeat").value = t.type;
//     const date = document.getElementById("dateSelector").value;

//     const previewBox = document.getElementById("detail_multi_previewBox");
//     const addBtn = document.getElementById("detail_multi_addBtn");
//     previewBox.querySelectorAll(".image-full, .image-half, .image-grid").forEach(el => el.remove());

//     try {
//         const imageList = t.images || [];
//         imageList.forEach(imgName => {
//             const div = document.createElement("div");
//             div.className = "image-half";
//             div.innerHTML = `
//                 <img src="https://dinego-bucket-aws.s3.ap-south-1.amazonaws.com/tables/thumb_${imgName}" data-name="${imgName}" />
//                 <input type="hidden" name="old_images" value="${imgName}" />
//                 <button class="remove-btn">×</button>
//             `;
//             div.querySelector(".remove-btn").onclick = () => div.remove();
//             previewBox.insertBefore(div, addBtn);
//         });
//     } catch (err) {
//         console.error("Không thể hiển thị ảnh bàn:", err);
//     }

//     const infoBox = document.getElementById("reservationInfoBox");
//     const actionBox = document.getElementById("reservationActionBox");
//     infoBox.innerHTML = `<em>Đang tải thông tin đặt bàn...</em>`;
//     actionBox.innerHTML = "";

//     // --- 2. Lấy trạng thái bàn từ class CSS ---
//     const tableElem = document.querySelector(`.table-item[data-table-id="${t.id}"]`);
//     const classList = tableElem?.classList || [];

//     let mode = "unknown";
//     if (classList.contains("status-available")) mode = "available";
//     else if (classList.contains("status-reserved")) mode = "reserved";
//     else if (classList.contains("status-inuse")) mode = "inuse";

//     // --- 3. Xử lý từng trạng thái ---
//     if (mode === "available") {
//         infoBox.innerHTML = `
//             <div><strong>Trạng thái bàn:</strong> Đang trống</div>
//             <div><em>Chưa có thông tin đặt bàn.</em></div>
//         `;
//         actionBox.innerHTML = `<button class="btn btn-danger" onclick="markTableStatus(${t.id}, 2)">Đặt bàn</button>`;

//         const modal = new bootstrap.Modal(document.getElementById("detailTableModal"));
//         modal.show();
//         modal._element.addEventListener("shown.bs.modal", () => setupMultipleImagePreviewGrid("detail_multi"));
//         return;
//     }

//     if (mode === "reserved") {
//         infoBox.innerHTML = `
//             <div><strong>Trạng thái bàn:</strong> Đang chờ thanh toán</div>
//             <div><em>Bàn đang được giữ để thanh toán. Không thể đặt.</em></div>
//         `;
//         const modal = new bootstrap.Modal(document.getElementById("detailTableModal"));
//         modal.show();
//         modal._element.addEventListener("shown.bs.modal", () => setupMultipleImagePreviewGrid("detail_multi"));
//         return;
//     }

//     if (mode === "inuse") {
//         const dateStr = new Date(date).toISOString().split("T")[0];
//         // Gọi API lấy thông tin đặt bàn
//         fetch(`/Reservation/GetReservationInfo?table_id=${t.id}&date=${dateStr}`)
//             .then(res => {
//                 if (res.status === 204 || res.status === 404) return null;
//                 return res.json();
//             })
//             .then(r => {
//                 infoBox.innerHTML = '';
//                 actionBox.innerHTML = '';

//                 if (!r || Object.keys(r).length === 0) {
//                     infoBox.innerHTML = `
//                         <div><strong>Trạng thái bàn:</strong> Đã đặt</div>
//                         <div><em>Chưa có thông tin đặt bàn.</em></div>
//                     `;
//                     actionBox.innerHTML = `<button class="btn btn-success" onclick="markTableStatus(${t.id}, 0)">Hoàn thành</button>`;
//                     return;
//                 }

//                 const reser = r;
//                 infoBox.innerHTML = `
//                     <div><strong>Người đặt:</strong> ${reser.customer?.cus_name || "(chưa có)"}</div>
//                     <div><strong>SĐT:</strong> ${reser.customer?.cus_phone || "(chưa có)"}</div>
//                     <div><strong>Ngày giờ:</strong> ${reser.reser_date ? new Date(reser.reser_date).toLocaleString() : "(không có)"}</div>
//                     <div><strong>Ghi chú:</strong> ${reser.reser_note || "(không có)"}</div>
//                     <div><strong>Trạng thái bàn:</strong> Đã đặt</div>
//                 `;
//                 actionBox.innerHTML = `<button class="btn btn-success" onclick="markTableStatus(${t.id}, 0, ${reser.reser_id}, 3)">Hoàn thành</button>`;
//             })
//             .catch(err => {
//                 console.error("Lỗi khi fetch thông tin đặt bàn:", err);
//                 infoBox.innerHTML = `
//                     <div><strong>Trạng thái bàn:</strong> Đã đặt</div>
//                     <div><em>Không tìm thấy thông tin đặt bàn.</em></div>
//                 `;
//                 actionBox.innerHTML = `<button class="btn btn-success" onclick="markTableStatus(${t.id}, 0)">Hoàn thành</button>`;
//             })
//             .finally(() => {
//                 const modal = new bootstrap.Modal(document.getElementById("detailTableModal"));
//                 modal.show();
//                 modal._element.addEventListener("shown.bs.modal", () => setupMultipleImagePreviewGrid("detail_multi"));
//             });
//     }
// }


// function markTableStatus(tableId, newTableStatus, reservationId = null, newReservationStatus = null) {
//     if (!confirm("Bạn có chắc chắn muốn cập nhật trạng thái bàn?")) return;

//     const queryParams = new URLSearchParams({
//         table_id: tableId,
//         status: newTableStatus
//     });

//     if (reservationId !== null) queryParams.append("reser_id", reservationId);
//     if (newReservationStatus !== null) queryParams.append("reser_status", newReservationStatus);

//     fetch(`/Reservation/UpdateTableStatus?${queryParams.toString()}`, {
//         method: "PUT"
//     })
//         .then(res => {
//             if (!res.ok) throw new Error("Cập nhật thất bại");
//             return res.json();
//         })
//         .then(data => {
//             if (data.success) {
//                 alert("Cập nhật trạng thái bàn thành công!");
//                 document.getElementById("detailTableModal")?.querySelector(".btn-close")?.click();

//                 // Tự động refresh danh sách bàn
//                 const elem = document.querySelector(`[data-table-id="${tableId}"]`);
//                 if (elem) updateTableStatusUI(elem, newTableStatus);

//                 // if (typeof loadTablesForReservation === "function") loadTablesForReservation();
//                 // if (typeof loadTablesForManage === "function") loadTablesForManage();
//             } else {
//                 throw new Error("Phản hồi API không hợp lệ");
//             }
//         })
//         .catch(err => {
//             console.error("Lỗi khi cập nhật trạng thái bàn:", err);
//             alert("Không thể cập nhật trạng thái bàn. Vui lòng thử lại.");
//         });
// }




// function parseTableStatus(status) {
//     switch (status) {
//         case 0: return "Đang trống";
//         case 1: return "Đang chờ thanh toán";
//         case 2: return "Đã đặt";
//         case 3: return "Tạm đóng";
//         default: return "Không rõ";
//     }
// }


//Detail, edit, delete table modal
function setupTableModal(t) {
    document.getElementById("detailTableId").value = t.id;
    document.getElementById("detailTableLabel").value = t.label;
    document.getElementById("detailTableSeat").value = t.type;

    const previewBox = document.getElementById("detail_multi_previewBox");
    const addBtn = document.getElementById("detail_multi_addBtn");
    previewBox.querySelectorAll(".image-full, .image-half, .image-grid").forEach(el => el.remove());

    try {
        const imageList = t.images || [];
        imageList.forEach(imgName => {
            const div = document.createElement("div");
            div.className = "image-half";
            div.innerHTML = `
                <img src="https://dinego-bucket-aws.s3.ap-south-1.amazonaws.com/tables/thumb_${imgName}" data-name="${imgName}" />
                <input type="hidden" name="old_images" value="${imgName}" />
                <button class="remove-btn">×</button>
            `;
            div.querySelector(".remove-btn").onclick = () => div.remove();
            previewBox.insertBefore(div, addBtn);
        });
    } catch (err) {
        console.error("Không thể hiển thị ảnh bàn:", err);
    }

    const modal = new bootstrap.Modal(document.getElementById("detailTableModal"));
    modal.show();

    modal._element.addEventListener("shown.bs.modal", function () {
        setupMultipleImagePreviewGrid("detail_multi");
    });
}



// Create table
document.addEventListener("DOMContentLoaded", function () {
    // Khởi tạo preview ảnh
    setupMultipleImagePreviewGrid("table_multi");
});

function confirmAddTableFromModal() {
    const label = document.getElementById("modalTableLabel").value.trim();
    const seat = document.getElementById("modalTableSeat").value.trim();
    const area = document.getElementById("areaSelector").value;
    const files = document.getElementById("table_multi_input").files;

    if (!label || !seat || files.length === 0) {
        alert("Vui lòng nhập đầy đủ thông tin và chọn ít nhất 1 ảnh.");
        return;
    }

    showGlobalLoader();

    const formData = new FormData();
    formData.append("label", label);
    formData.append("seat", seat);
    formData.append("area_id", area); // ✅ Đúng tên param server nhận

    for (let i = 0; i < files.length; i++) {
        formData.append("images", files[i]);
    }

    fetch("/Table/CreateTable", {
        method: "POST",
        body: formData
    })
        .then(res => {
            if (!res.ok) throw new Error("Lỗi khi lưu bàn.");
            return res.json();
        })
        .then(data => {
            if (data.success) {
                hideGlobalLoader();
                bootstrap.Modal.getInstance(document.getElementById("addTableModal")).hide();
                loadTablesForManage(); // Reload layout

                // Reset form
                document.getElementById("modalTableLabel").value = "";
                document.getElementById("modalTableSeat").value = "2";
                document.getElementById("table_multi_previewBox").innerHTML = `
                <div id="table_multi_addBtn" class="add-image-box">
                    <i class="bi bi-file-image fs-2"></i>
                    <span class="ms-2">Thêm ảnh</span>
                </div>`;
                setupMultipleImagePreviewGrid("table_multi");
            } else {
                alert("❌ Lỗi khi thêm bàn.");
            }
        })
        .catch(err => {
            hideGlobalLoader();
            console.error("❌", err);
            alert("❌ Có lỗi xảy ra khi lưu bàn.");
        });
}


//Details _ Update & Delete Table
function confirmUpdateTable() {
    const id = document.getElementById("detailTableId").value;
    const label = document.getElementById("detailTableLabel").value.trim();
    const seat = document.getElementById("detailTableSeat").value;

    if (!label || seat <= 0) {
        showNotification("Vui lòng nhập đầy đủ tên và số chỗ.", "warning");
        return;
    }

    const formData = new FormData();
    formData.append("table_id", id);
    formData.append("table_name", label);
    formData.append("table_seat", seat);

    // 📦 Ảnh cũ còn giữ lại
    const oldImages = Array.from(document.querySelectorAll("input[name='old_images']")).map(x => x.value);
    formData.append("old_images", JSON.stringify(oldImages));

    // 📥 Ảnh mới thêm vào
    const newImages = document.getElementById("detail_multi_input").files;
    for (let i = 0; i < newImages.length; i++) {
        formData.append("images", newImages[i]);
    }

    // Gửi API
    fetch('/Table/UpdateTable', {
        method: 'POST',
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                showNotification("Cập nhật bàn thành công!", "success");

                // Đóng modal
                bootstrap.Modal.getInstance(document.getElementById("detailTableModal")).hide();

                // 🚀 Reload lại danh sách bàn
                loadTablesForManage()

            } else {
                showNotification("Cập nhật thất bại!", "danger");
            }
        })
}

function openDeleteTableModal(tableId, tableName) {
    document.getElementById("tableToDeleteId").value = tableId;
    document.getElementById("tableToDeleteName").textContent = tableName;

    const modal = new bootstrap.Modal(document.getElementById("deleteTableModal"));
    modal.show();
}


function confirmDeleteTable() {
    const id = document.getElementById("tableToDeleteId").value;

    const formData = new FormData();
    formData.append("id", id);

    fetch('/Table/DeleteTable', {
        method: 'POST',
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                showNotification("Đã xoá bàn thành công!", "success");
                bootstrap.Modal.getInstance(document.getElementById("deleteTableModal")).hide();
                const detailModal = bootstrap.Modal.getInstance(document.getElementById("detailTableModal"));
                if (detailModal) detailModal.hide();
                loadTablesForManage()
            } else {
                showNotification("Bàn đang được đặt, không thể xóa!", "danger");
            }
        });
}


//Area _ Edit & Delete
$(document).on('click', '.btn-edit-area', function () {
    const areaId = $(this).data('id');
    const areaName = $(this).data('name');
    $('#editAreaId').val(areaId);
    $('#editAreaName').val(areaName);
    $('#editAreaModal').modal('show');
});

// Gửi form chỉnh sửa
$('#editAreaForm').submit(function (e) {
    e.preventDefault();
    const id = $('#editAreaId').val();
    const name = $('#editAreaName').val();

    $.ajax({
        url: '/Table/EditArea',
        type: 'POST',
        data: { id: id, name: name }, // tên field ok, model binder không phân biệt hoa thường
        success: function (res) {
            if (res.success) {
                showNotification(res.message || "Cập nhật khu vực thành công", "success");
                $('#editAreaModal').modal('hide');
                // Tải lại danh sách (tuỳ bạn): 
                location.reload();
            } else {
                showNotification(res.message || "Cập nhật thất bại", "danger");
            }
        },
        error: function () {
            showNotification("Không thể cập nhật khu vực. Vui lòng thử lại.", "danger");
        }
    });
});

let areaToDeleteId = 0;

// Khi bấm nút xoá
$(document).on('click', '.btn-delete-area', function () {
    areaToDeleteId = $(this).data('id');
    const areaName = $(this).data('name');

    $('#areaToDeleteId').val(areaToDeleteId);
    $('#areaToDeleteName').text(areaName);
    $('#deleteAreaModal').modal('show');
});

// Khi xác nhận xoá trong modal
$('#confirmDeleteAreaBtn').click(function () {
    const id = $('#areaToDeleteId').val();

    $.ajax({
        url: '/Table/DeleteArea',
        type: 'POST',
        data: { id: id },
        success: function (res) {
            if (res.success) {
                showNotification(res.message || "Xoá khu vực thành công", "success");
                $('#deleteAreaModal').modal('hide');
                location.reload();
            } else {
                showNotification(res.message || "Xoá khu vực thất bại", "danger");
            }
        },
        error: function () {
            showNotification("Không thể xoá khu vực. Vui lòng thử lại.", "danger");
        }
    });
});

function fetchAndShowAvailableSlots(tableId, date) {
    fetch(`/api/Reservation/GetAvailableSlots?table_id=${tableId}&date=${date}`)
        .then(res => res.json())
        .then(slots => {
            const slotBox = document.getElementById("ownerSlotContainer");
            if (!slots || slots.length === 0) {
                slotBox.innerHTML = `<div class="text-danger">Không còn khung giờ trống.</div>`;
                return;
            }
            slotBox.innerHTML = slots.map(s =>
                `<span class="badge bg-success m-1">${s}</span>`
            ).join("");
        });
}

// Khi trang load lại, kiểm tra xem có thông báo không
$(document).ready(function () {
    const storedNotification = localStorage.getItem("areaNotification");
    if (storedNotification) {
        const [message, type] = storedNotification.split('|');
        showNotification(message, type);
        localStorage.removeItem("areaNotification");
    }
});


