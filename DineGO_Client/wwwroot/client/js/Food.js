let currentIndex = 3;
let map, marker, selectedLatLng, originalLatLng;
let resetBound = false;
const groups = document.querySelectorAll('.restaurant-group');
const btn = document.getElementById('loadMoreBtn');

btn?.addEventListener('click', () => {
    let nextGroup = groups[currentIndex];
    if (nextGroup) {
        nextGroup.classList.remove("d-none");
        currentIndex++;
    }
    if (currentIndex >= groups.length) {
        btn.style.display = "none";
    }
});

document.addEventListener("DOMContentLoaded", function () {

    $('#mapModal').on('shown.bs.modal', function () {
        const cookieAccepted = document.cookie.includes("CookieConsent=true");
        if (!cookieAccepted) {
            alert("Bạn cần chấp nhận cookie để sử dụng tính năng định vị.");
            $('#mapModal').modal('hide');
            return;
        }

        setTimeout(() => {
            if (!map) {
                try {
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
                    if (typeof restaurantMarkers !== "undefined" && Array.isArray(restaurantMarkers)) {
                        restaurantMarkers.forEach(r => {
                            const lat = parseFloat(r.latitude || r.lat);
                            const lng = parseFloat(r.longitude || r.lng);

                            const redIcon = L.icon({
                                iconUrl: 'https://maps.google.com/mapfiles/ms/icons/red-dot.png',
                                iconSize: [32, 32],
                                iconAnchor: [16, 32],
                                popupAnchor: [0, -32]
                            });

                            if (!isNaN(lat) && !isNaN(lng)) {
                                const imageUrl = r.image
                                    ? `https://dinego-bucket-aws.s3.ap-south-1.amazonaws.com/restaurants/thumb_${r.image}`
                                    : '/client/images/res1.jpeg'; // ✅ Ảnh mặc định nếu không có

                                const popupContent = `
                         <div style="text-align:center;">
                         <img src="${imageUrl}" alt="${r.name}" 
                           style="width:100px;height:80px;border-radius:4px;object-fit:cover;" 
                            onerror="this.onerror=null;this.src='${imageUrl}';" />
                            <br />
                            <b>${r.name}</b><br/>
                            <span>${r.address}</span>
                            </div>
                            `;

                                L.marker([lat, lng], { icon: redIcon })
                                    .addTo(map)
                                    .bindPopup(popupContent);
                            } else {
                                console.warn("❌ Không thể tạo marker vì thiếu lat/lng:", r);
                            }
                        });

                    }


                    navigator.geolocation.getCurrentPosition(
                        async (position) => {
                            const lat = position.coords.latitude;
                            const lng = position.coords.longitude;
                            map.setView([lat, lng], 15);

                            // auto martker address user
                            selectedLatLng = { lat, lng };
                            originalLatLng = { lat, lng };
                            marker = L.marker(selectedLatLng).addTo(map);

                            const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lng}`);
                            const data = await res.json();

                            const district = data.address.city_district || data.address.district || "";
                            const province = data.address.state || data.address.city || data.address.county || "";
                            const location = [district, province].filter(Boolean).join(', ');

                            document.getElementById("selectedLocation").textContent = location;
                            document.getElementById("userAddress").value = location;

                            // Gửi về server nếu muốn
                            await fetch("/Location/SetUserLocation", {
                                method: "POST",
                                headers: { "Content-Type": "application/json" },
                                body: JSON.stringify(location)
                            });
                        },
                        (err) => {
                            console.warn("Không thể lấy vị trí GPS:", err.message);
                        }
                    );

                    // Cho phép người dùng click để đổi vị trí
                    map.on('click', async function (e) {
                        selectedLatLng = e.latlng;

                        if (marker) map.removeLayer(marker);
                        marker = L.marker(selectedLatLng).addTo(map);

                        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${e.latlng.lat}&lon=${e.latlng.lng}`);
                        const data = await res.json();
                        const district = data.address.city_district || data.address.district || "";
                        const province = data.address.state || data.address.city || data.address.county || "";

                        // Ghép các phần không rỗng lại
                        const parts = [district, province].filter(Boolean);
                        const location = parts.join(', ');

                        // Hiển thị kết quả rõ ràng
                        document.getElementById("selectedLocation").textContent = `${location}`;
                        document.getElementById("userAddress").value = location;



                        await fetch("/Location/SetUserLocation", {
                            method: "POST",
                            headers: { "Content-Type": "application/json" },
                            body: JSON.stringify(location)
                        });
                    });


                    console.log("✅ Leaflet map created");
                } catch (err) {
                    console.error("❌ Lỗi tạo map:", err);
                }
            } else {
                map.invalidateSize();
                console.log("📦 Map invalidated");
            }
        }, 200);
    });


    document.getElementById("confirmLocation").addEventListener("click", () => {
        const location = document.getElementById("userAddress").value;
        if (!location) {
            alert("Vui lòng chọn vị trí trên bản đồ.");
            return;
        }

        $('#mapModal').modal('hide');
        const form = document.querySelector('form[method="get"]');
        if (form) {
            document.getElementById("userLat").value = selectedLatLng.lat;
            document.getElementById("userLng").value = selectedLatLng.lng;
            setTimeout(() => {
                form.submit();
            }, 300);
        }
    });

});
//select user address
const provinceApi = 'https://provinces.open-api.vn/api/?depth=1';
const districtApi = id => `https://provinces.open-api.vn/api/p/${id}?depth=2`;
const wardApi = id => `https://provinces.open-api.vn/api/d/${id}?depth=2`;

document.addEventListener('DOMContentLoaded', () => {
    const provinceSel = document.getElementById('province');
    const districtSel = document.getElementById('district');
    const wardSel = document.getElementById('ward');
    const addressInput = document.getElementById('userAddress');

    // Load tỉnh
    loadProvinces();

    // Cập nhật địa chỉ từ dropdown
    window.updateUserAddress = function () {
        const parts = [
            wardSel.selectedOptions[0]?.text || '',
            districtSel.selectedOptions[0]?.text || '',
            provinceSel.selectedOptions[0]?.text || ''
        ].filter(Boolean);
        addressInput.value = parts.join(', ');
        console.log("📍 Địa chỉ chọn từ dropdown:", addressInput.value);
    };
    let updateAddressFromCoords = async function (lat, lng) {
        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lng}`);
        const data = await res.json();

        const ward = data.address.suburb || data.address.neighbourhood || "";
        const district = data.address.city_district || data.address.district || "";
        const province = data.address.state || data.address.city || data.address.county || "";

        const parts = [ward, district, province].filter(Boolean);
        const location = parts.join(', ');

        document.getElementById("selectedLocation").textContent = location;
        document.getElementById("userAddress").value = location;
    };
    // Modal bản đồ (Leaflet)
    $('#mapModal').on('shown.bs.modal', function () {
        const cookieAccepted = document.cookie.includes("CookieConsent=true");
        if (!cookieAccepted) {
            alert("Bạn cần chấp nhận cookie để sử dụng định vị.");
            $('#mapModal').modal('hide');
            return;
        }

        setTimeout(() => {
            // ✅ Luôn cập nhật lại vị trí gốc mỗi lần mở modal
            navigator.geolocation.getCurrentPosition(async (position) => {
                const lat = position.coords.latitude;
                const lng = position.coords.longitude;
                originalLatLng = { lat, lng };
            });

            if (!map) {
                map = L.map('map').setView([10.762622, 106.660172], 13);
                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; OpenStreetMap contributors'
                }).addTo(map);

                navigator.geolocation.getCurrentPosition(async (pos) => {
                    const lat = pos.coords.latitude, lng = pos.coords.longitude;
                    map.setView([lat, lng], 15);
                    selectedLatLng = { lat, lng };
                    marker = L.marker(selectedLatLng).addTo(map);
                    await updateAddressFromCoords(lat, lng);
                });

                map.on('click', async (e) => {
                    selectedLatLng = e.latlng;
                    if (marker) map.removeLayer(marker);
                    marker = L.marker(selectedLatLng).addTo(map);
                    await updateAddressFromCoords(e.latlng.lat, e.latlng.lng);
                });
            }

            if (!resetBound) {
                const resetBtn = document.getElementById("btnResetLocation");
                if (resetBtn) {
                    resetBtn.addEventListener("click", async () => {
                        if (!originalLatLng) {
                            alert("Không tìm thấy vị trí gốc!");
                            return;
                        }

                        const { lat, lng } = originalLatLng;
                        map.setView([lat, lng], 15);

                        if (marker) map.removeLayer(marker);
                        marker = L.marker([lat, lng]).addTo(map);

                        try {
                            await updateAddressFromCoords(lat, lng);
                        } catch (err) {
                            console.error("❌ Lỗi khi cập nhật địa chỉ:", err);
                        }
                    });

                    resetBound = true;
                }
            }

        }, 200);



        // Khi xác nhận map
        document.getElementById("confirmLocation").onclick = () => {
            if (!addressInput.value) {
                alert("Bạn chưa chọn vị trí!");
                return;
            }
            $('#mapModal').modal('hide');
        };
    });
});

// Load tỉnh/quận/phường
async function loadProvinces() {
    const res = await fetch(provinceApi);
    const data = await res.json();
    const select = document.getElementById("province");
    select.innerHTML = `<option value="">-- Tỉnh/Thành phố --</option>`;
    data.forEach(p => {
        select.innerHTML += `<option value="${p.code}">${p.name}</option>`;
    });
}

async function loadDistricts() {
    const provinceId = document.getElementById("province").value;
    const res = await fetch(districtApi(provinceId));
    const data = await res.json();
    const select = document.getElementById("district");
    select.innerHTML = `<option value="">-- Quận/Huyện --</option>`;
    document.getElementById("ward").innerHTML = `<option value="">-- Phường/Xã --</option>`;
    data.districts.forEach(d => {
        select.innerHTML += `<option value="${d.code}">${d.name}</option>`;
    });
}

async function loadWards() {
    const districtId = document.getElementById("district").value;
    const res = await fetch(wardApi(districtId));
    const data = await res.json();
    const select = document.getElementById("ward");
    select.innerHTML = `<option value="">-- Phường/Xã --</option>`;
    data.wards.forEach(w => {
        select.innerHTML += `<option value="${w.name}">${w.name}</option>`;
    });
}
// Đặt gần loadProvinces(), loadDistricts(), loadWards()
async function goToProvinceCenter(provinceName) {
    try {
        const res = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(provinceName)}, Vietnam`);
        const data = await res.json();

        if (data && data.length > 0) {
            const lat = parseFloat(data[0].lat);
            const lon = parseFloat(data[0].lon);
            map.setView([lat, lon], 12); // zoom đến tỉnh
        } else {
            console.warn("❌ Không tìm thấy vị trí cho tỉnh:", provinceName);
        }
    } catch (err) {
        console.error("Lỗi khi lấy tọa độ tỉnh:", err);
    }
}

// Sau khi loadProvinces() hoàn tất, gắn sự kiện change
document.addEventListener('DOMContentLoaded', () => {
    const provinceSel = document.getElementById("province");

    provinceSel.addEventListener("change", function () {
        const provinceName = this.selectedOptions[0]?.text;
        if (provinceName && map) {
            goToProvinceCenter(provinceName);
        }
    });
});

//validate form 
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("foodForm");

    form.addEventListener("submit", function (e) {
        let isValid = true;

        // Reset lỗi cũ
        document.getElementById("foodNameError").textContent = "";
        document.getElementById("foodDescriptionError").textContent = "";
        document.getElementById("foodPriceError").textContent = "";
        document.getElementById("foodStatusError").textContent = "";
        document.getElementById("foodImageError").textContent = "";

        // Validate Tên món
        const name = document.getElementById("foodName").value.trim();
        if (name === "") {
            document.getElementById("foodNameError").textContent = "Tên món không được để trống";
            isValid = false;
        } else if (name.length > 100) {
            document.getElementById("foodNameError").textContent = "Tên món tối đa 100 ký tự";
            isValid = false;
        }
        else if (name.length < 2) {
            document.getElementById("foodNameError").textContent = "Tên món chứa ít nhất 2 kí tự";
            isValid = false;
        }

        // Validate Mô tả
        const description = document.getElementById("foodDescription").value.trim();
        if (description === "") {
            document.getElementById("foodDescriptionError").textContent = "Mô tả không được để trống";
            isValid = false;
        } else if (description.length > 200) {
            document.getElementById("foodDescriptionError").textContent = "Mô tả tối đa 200 ký tự";
            isValid = false;
        }

        // Validate Giá
        const price = parseFloat(document.getElementById("foodPrice").value.trim());
        if (isNaN(price)) {
            document.getElementById("foodPriceError").textContent = "Giá không hợp lệ";
            isValid = false;
        } else if (price <= 1000 || price >= 2000000) {
            document.getElementById("foodPriceError").textContent =
                "Giá món phải lớn hơn 1.000 VND và nhỏ hơn 2.000.000 VND";
            isValid = false;
        }


        // Validate Trạng thái
        const status = document.getElementById("foodStatus").value.trim();
        if (status === "") {
            document.getElementById("foodStatusError").textContent = "Vui lòng chọn trạng thái";
            isValid = false;
        }

        // Validate Ảnh
        const image = document.getElementById("foodImage").files.length;
        if (image === 0) {
            document.getElementById("foodImageError").textContent = "Vui lòng chọn ít nhất 1 ảnh";
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
        }
    });
});
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("foodEditForm");

    form.addEventListener("submit", function (e) {
        let isValid = true;

        // Reset lỗi cũ
        document.getElementById("foodNameError").textContent = "";
        document.getElementById("foodDescriptionError").textContent = "";
        document.getElementById("foodPriceError").textContent = "";
        document.getElementById("foodStatusError").textContent = "";
        document.getElementById("foodImageError").textContent = "";

        // Validate Tên món
        const name = document.getElementById("foodName").value.trim();
        if (name === "") {
            document.getElementById("foodNameError").textContent = "Tên món không được để trống";
            isValid = false;
        } else if (name.length > 100) {
            document.getElementById("foodNameError").textContent = "Tên món tối đa 100 ký tự";
            isValid = false;
        }
        else if (name.length < 2) {
            document.getElementById("foodNameError").textContent = "Tên món chứa ít nhất 2 kí tự";
            isValid = false;
        }

        // Validate Mô tả
        const description = document.getElementById("foodDescription").value.trim();
        if (description === "") {
            document.getElementById("foodDescriptionError").textContent = "Mô tả không được để trống";
            isValid = false;
        } else if (description.length > 200) {
            document.getElementById("foodDescriptionError").textContent = "Mô tả tối đa 200 ký tự";
            isValid = false;
        }

        // Validate Giá
        const price = parseFloat(document.getElementById("foodPrice").value.trim());
        if (isNaN(price)) {
            document.getElementById("foodPriceError").textContent = "Giá không hợp lệ";
            isValid = false;
        } else if (price <= 1000 || price >= 2000000) {
            document.getElementById("foodPriceError").textContent =
                "Giá món phải lớn hơn 1.000 VND và nhỏ hơn 2.000.000 VND";
            isValid = false;
        }


        // Validate Trạng thái
        const status = document.getElementById("foodStatus").value.trim();
        if (status === "") {
            document.getElementById("foodStatusError").textContent = "Vui lòng chọn trạng thái";
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
        }
    });
});
document.getElementById("toggleSearch").addEventListener("click", function () {
    const section = document.getElementById("advancedSearch");
    const isHidden = section.style.display === "none";
    section.style.display = isHidden ? "flex" : "none";
    this.innerText = isHidden ? "🔽 Ẩn bớt" : "🔎 Mở rộng tìm kiếm";
});
function renderSelectedTags() {
    const input = document.getElementById("tagSelected");
    const selectedContainer = document.getElementById("selectedTags");

    selectedContainer.innerHTML = ""; // Xoá hết tag cũ

    if (input && input.value) {
        const tags = input.value.split(',').map(t => t.trim()).filter(t => t);
        tags.forEach(tag => {
            const tagEl = document.createElement("span");
            tagEl.className = "badge bg-danger text-white d-flex align-items-center gap-2 px-3 py-2";
            tagEl.innerHTML = `${tag} <i class="bi bi-x-circle-fill" style="cursor:pointer;"></i>`;
            tagEl.querySelector("i").onclick = () => removeTag(tag);
            selectedContainer.appendChild(tagEl);
        });
    }
}

function removeTag(tag) {
    const input = document.getElementById("tagSelected");
    let current = input.value.split(',').map(t => t.trim()).filter(t => t);
    input.value = current.filter(t => t !== tag).join(", ");
    renderSelectedTags(); // ✅ Gọi lại để cập nhật UI
}

function addTag(tag) {
    const input = document.getElementById("tagSelected");
    let current = input.value.split(',').map(t => t.trim()).filter(t => t);

    if (current.includes(tag)) return;
    if (current.length >= 3) {
        showNotification("Chỉ được chọn tối đa 3 tag!", "danger");
        return;
    }

    current.push(tag);
    input.value = current.join(", ");
    renderSelectedTags(); // ✅ Thêm xong thì render lại toàn bộ
}

window.addEventListener("load", renderSelectedTags); // ✅ Gọi khi load
