document.addEventListener("DOMContentLoaded", () => {
    const areaSelector = document.getElementById("areaSelector");
    const layout = document.getElementById("booking-table-layout");
    const selectedInput = document.getElementById("selectedTableId");
    const dateInput = document.querySelector("input[name='reser_date_date']");
    const slotContainer = document.getElementById("timeSlotContainer"); // fix id
    const selectedTimeInput = document.getElementById("selectedTimeSlot"); // hidden input

    // Choices.js init
    new Choices(areaSelector, { searchEnabled: false, itemSelectText: "", shouldSort: false });


    // Load lần đầu
    fetchAndRenderTables();

    areaSelector.addEventListener("change", fetchAndRenderTables);
    dateInput.addEventListener("change", fetchAndRenderTables);

    async function fetchAndRenderTables() {
        const areaId = parseInt(areaSelector.value);
        const date = dateInput.value;

        if (!areaId || !date) return;

        try {
            const response = await fetch(`/Table/GetTables?area_id=${areaId}&date=${date}`);
            const tables = await response.json();
            renderTables(tables , date);
        } catch (err) {
            console.error("Lỗi load bàn:", err);
        }
    }

    function renderTables(tables, date) {
        layout.innerHTML = '';
        const noTableMessage = document.getElementById("noTableMessage");

        if (!tables || tables.length === 0) {
            selectedInput.value = '';
            noTableMessage.classList.remove("d-none");
            return;
        } else {
            noTableMessage.classList.add("d-none");
        }

        tables.forEach(t => {
            const div = document.createElement("div");
            div.className = `table-item`;
            div.dataset.tableId = t.id;

            const imageSrc = (t.images?.length > 0)
                ? `https://dinego-bucket-aws.s3.ap-south-1.amazonaws.com/tables/thumb_${t.images[0]}`
                : "/common/images/logo.png";

            div.innerHTML = `
            <div class="table-hover-content">
                <div class="table-name fw-bold text-center text-danger position-relative">
                    ${t.label}
                    <span class="checkmark-icon d-none">✅</span>
                </div>
                <div class="table-seat text-muted text-center">${t.type} người</div>
            </div>
            <div class="table-hover-detail">
                <img src="${imageSrc}" class="table-img img-fluid rounded" alt="${t.label}" />
            </div>
        `;

            div.addEventListener("click", () => {
                layout.querySelectorAll(".table-item").forEach(item => {
                    item.classList.remove("selected");
                    item.querySelector(".checkmark-icon")?.classList.add("d-none");
                });

                div.classList.add("selected");
                div.querySelector(".checkmark-icon").classList.remove("d-none");
                selectedInput.value = t.id;


                // reset slot khi chọn bàn khác
                selectedTimeInput.value = "";
                fetchAvailableSlots(t.id, date);
            });

            layout.appendChild(div);
        });
    }

    async function fetchAvailableSlots(tableId, date) {
        slotContainer.innerHTML = `<div class="text-muted">Đang tải...</div>`;
        try {
            const response = await fetch(`/Reservation/GetAvailableSlots?tableId=${tableId}&date=${date}`);
            const slots = await response.json();

            if (!slots || slots.length === 0) {
                slotContainer.innerHTML = `<div class="text-danger">Không còn khung giờ trống.</div>`;
                return;
            }

            slotContainer.innerHTML = slots.map(s =>
                `<button type="button" class="btn btn-outline-danger m-1 slot-btn" data-time="${s}">
                    ${s}
                </button>`
            ).join("");

            // Chọn slot
            document.querySelectorAll(".slot-btn").forEach(btn => {
                btn.addEventListener("click", () => {
                    document.querySelectorAll(".slot-btn").forEach(b => b.classList.remove("active", "btn-danger"));
                    btn.classList.add("active", "btn-danger");
                    selectedTimeInput.value = btn.dataset.time; // Đảm bảo btn.dataset.time là "HH:mm"
                });
            });

        } catch (err) {
            console.error("Lỗi load slot:", err);
            slotContainer.innerHTML = `<div class="text-danger">Lỗi tải giờ trống</div>`;
        }
    }

    // ✅ Tạo kết nối
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/reservationHub") // trùng MapHub
        .build();

    connection.on("ReservationUpdated", (...args) => {
        console.log("📩 Nhận event ReservationUpdated:", args);
    });

    connection.on("ReservationUpdated", (tableId, date) => {
    console.log("🔔 Có thay đổi reservation:", tableId, date);

    const hubDate = new Date(date).toISOString().split("T")[0];

    // chỉ update slot nếu đang ở đúng ngày đó & đúng bàn đang được chọn
    if (dateInput.value === hubDate && selectedInput.value == tableId) {
        fetchAvailableSlots(tableId, hubDate);
    }
});

    // Bắt đầu kết nối
    connection.start()
        .then(() => console.log("✅ Connected to reservationHub"))
        .catch(err => console.error("❌ Lỗi connect hub:", err));
});
