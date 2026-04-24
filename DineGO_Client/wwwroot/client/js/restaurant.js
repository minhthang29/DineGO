let map, marker, selectedLatLng = null, originalLatLng = null;
const provinceApi = "https://provinces.open-api.vn/api/p/";

// ========== CẬP NHẬT ĐỊA CHỈ THEO LAT/LNG ==========
async function updateAddress(lat, lng) {
    try {
        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lng}`);
        const data = await res.json();

        const district = data.address.city_district || data.address.district || "";
        const province = data.address.state || data.address.city || data.address.county || "";
        const location = [district, province].filter(Boolean).join(", ");

        document.getElementById("selectedLocation").textContent = location;
        document.getElementById("userAddress").value = location;
    } catch (err) {
        console.error("❌ Lỗi lấy địa chỉ:", err);
    }
}

// ========== LOAD DANH SÁCH TỈNH ==========

async function loadProvinces() {
    try {
        const res = await fetch(provinceApi);
        const data = await res.json();
        const select = document.getElementById("province");
        select.innerHTML = `<option value="">-- Tỉnh/Thành phố --</option>`;
        data.forEach(p => {
            select.innerHTML += `<option value="${p.name}">${p.name}</option>`;
        });
    } catch (err) {
        console.error("❌ Lỗi load tỉnh:", err);
    }
}

// ✅ Gọi hàm khi trang đã sẵn sàng
document.addEventListener("DOMContentLoaded", function() {
    loadProvinces(); 
});
// ========== ZOOM ĐẾN TỈNH ==========
async function goToProvinceCenter(provinceName) {
    try {
        const res = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(provinceName)}, Vietnam`);
        const data = await res.json();

        if (data.length > 0) {
            const lat = parseFloat(data[0].lat);
            const lon = parseFloat(data[0].lon);

            map.setView([lat, lon], 12);

            if (marker) map.removeLayer(marker);
            marker = L.marker([lat, lon]).addTo(map);
            selectedLatLng = { lat, lng: lon };

            await updateAddress(lat, lon);
        }
    } catch (err) {
        console.error("❌ Lỗi tọa độ tỉnh:", err);
    }
}

// ========== SỰ KIỆN TRANG ==========
document.addEventListener("DOMContentLoaded", function () {
    loadProvinces();

    // Khi chọn tỉnh → zoom map
    document.getElementById("province").addEventListener("change", function () {
        const provinceName = this.value;
        if (provinceName && map) {
            goToProvinceCenter(provinceName);
        }
    });

    // Mở modal bản đồ
    $('#mapModal').on('shown.bs.modal', function () {
        setTimeout(() => {
            if (!map) {
                // Khởi tạo map
                map = L.map('map', {
                    center: [10.762622, 106.660172],
                    zoom: 13,
                    minZoom: 12,
                    maxZoom: 18,
                    maxBounds: [
                        [8.0, 102.0],
                        [24.0, 110.0]
                    ],
                    maxBoundsViscosity: 1.0
                });

                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; OpenStreetMap contributors'
                }).addTo(map);

                // Lấy vị trí hiện tại
                navigator.geolocation.getCurrentPosition(async (pos) => {
                    const lat = pos.coords.latitude;
                    const lng = pos.coords.longitude;
                    originalLatLng = { lat, lng };
                    selectedLatLng = { lat, lng };
                    map.setView([lat, lng], 15);
                    marker = L.marker([lat, lng]).addTo(map);
                    await updateAddress(lat, lng);
                });

                // Click chọn vị trí mới
                map.on('click', async (e) => {
                    selectedLatLng = e.latlng;
                    if (marker) map.removeLayer(marker);
                    marker = L.marker(selectedLatLng).addTo(map);
                    await updateAddress(e.latlng.lat, e.latlng.lng);
                });
            } else {
                map.invalidateSize();
            }
        }, 300);
    });

    // Nút Reset → về vị trí hiện tại
    document.getElementById("btnResetLocation").addEventListener("click", () => {
        if (!originalLatLng) {
            alert("Không tìm thấy vị trí gốc!");
            return;
        }
        const { lat, lng } = originalLatLng;
        map.setView([lat, lng], 15);
        if (marker) map.removeLayer(marker);
        marker = L.marker([lat, lng]).addTo(map);
        updateAddress(lat, lng);
    });

    // Nút Xác nhận → gửi form
    document.getElementById("confirmLocation").addEventListener("click", () => {
        if (!selectedLatLng) {
            alert("Vui lòng chọn vị trí trên bản đồ.");
            return;
        }
        $('#mapModal').modal('hide');
        const form = document.querySelector('form[method="get"]');
        if (form) {
            document.getElementById("userLat").value = selectedLatLng.lat;
            document.getElementById("userLng").value = selectedLatLng.lng;
            setTimeout(() => form.submit(), 300);
        }
    });
});
