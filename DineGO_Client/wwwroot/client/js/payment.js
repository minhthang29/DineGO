document.addEventListener("DOMContentLoaded", () => {
    const reserId = parseInt(document.getElementById("qrContainer").dataset.reserid);
    const reservationCreatedAt = new Date(document.getElementById("qrContainer").dataset.createdat);
    const countdownElement = document.getElementById("countdown");

    // --- Hàm đếm ngược 10 phút ---
    function getRemainingSeconds() {
        const now = new Date();
        const deadline = new Date(reservationCreatedAt.getTime() + 10 * 60 * 1000);
        const diff = Math.floor((deadline - now) / 1000);
        return diff > 0 ? diff : 0;
    }

    let countdown = getRemainingSeconds();

    function updateCountdown() {
        const minutes = Math.floor(countdown / 60);
        const seconds = countdown % 60;
        countdownElement.textContent =
            `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

        if (countdown === 0) {
            clearInterval(countdownTimer);
            clearInterval(paidChecker);
            document.getElementById("qrContainer").style.display = "none";
            document.getElementById("expiredMessage").classList.remove("d-none");

            // Hết hạn thì gọi Cancel API
            fetch(`/Reservation/CancelIfExpired?reser_id=${reserId}`, { method: "PUT" });

            return;
        }
        countdown--;
    }

    // --- Hàm check thanh toán ---
    async function checkPaid(reserId) {
        try {
            const response = await fetch(`/Reservation/CheckPaid?reser_id=${reserId}`);
            const result = await response.json();

            if (result.success) {
                showNotification(`Đặt bàn thành công.`, "success");
                showGlobalLoader();
                setTimeout(() => {
                    window.location.href = "/Customer/OrderHistory";
                }, 3000);
            } else {
                console.log("⌛ " + result.message);
            }
        } catch (err) {
            console.error("❌ Lỗi checkPaid:", err);
        }
    }


    // --- Timer ---
    const countdownTimer = setInterval(updateCountdown, 1000);
    const paidChecker = setInterval(() => checkPaid(reserId), 31000);

});