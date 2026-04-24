// Display common notification
function initNotificationBox() {
  const boxes = document.querySelectorAll('.notification-box');
  if (!boxes.length) return;

  boxes.forEach((box) => {
    const removeBox = () => {
      box.classList.add("hide");
      setTimeout(() => box.remove(), 300);
    };

    box.addEventListener("click", removeBox);
    setTimeout(removeBox, 3000);
  });
}

// Gọi khi DOM đã load
document.addEventListener("DOMContentLoaded", initNotificationBox);

// Display common notification ajax
function showNotification(message, type = "success") {
  const box = document.createElement("div");
  box.className = `alert alert-${type} notification-box`;

  const icon =
    type === "success"
      ? '<i class="bi bi-check-circle-fill text-success me-2"></i>'
      : type === "danger"
      ? '<i class="bi bi-exclamation-circle-fill text-danger me-2"></i>'
      : "";

  box.innerHTML = `${icon}${message}<div class="notification-progress"></div>`;
  document.body.appendChild(box);

  setTimeout(() => {
    box.classList.add("hide");
    setTimeout(() => box.remove(), 300);
  }, 3000);

  box.addEventListener("click", () => {
    box.classList.add("hide");
    setTimeout(() => box.remove(), 300);
  });
}
