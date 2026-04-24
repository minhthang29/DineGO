let map, marker;
let selectedLatLng = null;

function openLocationPicker() {
    $('#mapModal').modal('show');

    setTimeout(() => {
        const mapContainer = document.getElementById('map');
        if (!mapContainer) {
            console.error("❌ Không tìm thấy phần tử #map.");
            return;
        }

        if (map) {
            map.invalidateSize();
            return;
        }
        map = L.map('map', {
            center: [10.762622, 106.660172],
            zoom: 13,
            minZoom: 12,
            maxZoom: 18,
            maxBounds: [
                [8.0, 102.0],  // Tây Nam VN
                [24.0, 110.0]  // Đông Bắc VN
            ],
            maxBoundsViscosity: 1.0 // kéo bị bật lại khi ra ngoài
        });


        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        navigator.geolocation.getCurrentPosition(async (position) => {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            map.setView([lat, lng], 15);
            selectedLatLng = { lat, lng };
            marker = L.marker([lat, lng]).addTo(map);
            await updateAddress(lat, lng);
        });

        map.on('click', async (e) => {
            selectedLatLng = e.latlng;
            if (marker) map.removeLayer(marker);
            marker = L.marker(selectedLatLng).addTo(map);
            await updateAddress(e.latlng.lat, e.latlng.lng);
        });
    }, 300);
}

async function updateAddress(lat, lng) {
    try {
        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lng}`);
        const data = await res.json();
        const district = data.address.city_district || data.address.district || "";
        const province = data.address.state || data.address.city || data.address.county || "";
        const location = [district, province].filter(Boolean).join(', ');

        document.getElementById("selectedLocation").textContent = location;
        document.getElementById("addressInput").value = location;
    } catch (err) {
        console.error("❌ Lỗi lấy địa chỉ:", err);
    }
}

document.getElementById("confirmLocation").addEventListener("click", () => {
    if (!selectedLatLng) {
        alert("Vui lòng chọn vị trí trên bản đồ.");
        return;
    }

    const manualAddress = document.getElementById("manualAddress").value.trim();
    const autoAddress = document.getElementById("selectedLocation").textContent.trim();
    const fullAddress = manualAddress
        ? `${manualAddress}, ${autoAddress}`
        : autoAddress;

    document.getElementById("addressInput").value = fullAddress;

    // Gán giá trị latitude và longitude vào hidden input
    document.getElementById("cus_latitude").value = selectedLatLng.lat;
    document.getElementById("cus_longitude").value = selectedLatLng.lng;


    $('#mapModal').modal('hide');
});



document.getElementById('gpsButton').addEventListener('click', function () {
    if (!map) return;

    navigator.geolocation.getCurrentPosition(async (position) => {
        const lat = position.coords.latitude;
        const lng = position.coords.longitude;
        selectedLatLng = { lat, lng };

        map.setView([lat, lng], 15);
        if (marker) map.removeLayer(marker);
        marker = L.marker([lat, lng]).addTo(map);
        await updateAddress(lat, lng);
    }, (err) => {
        alert("Không thể lấy vị trí hiện tại. Vui lòng kiểm tra quyền truy cập GPS.");
        console.error("❌ Lỗi GPS:", err);
    });
});
$('#mapModal').on('shown.bs.modal', function () {
    if (map) {
        map.invalidateSize();
    }
});
