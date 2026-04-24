
document.addEventListener("DOMContentLoaded", function () {
    let currentCustomerId = 0;
    let currentResId = 0;
    let resOffset = 0;
    let isLoadingRes = false;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub", { withCredentials: true })
        .build();

    const roChatContent = document.getElementById("ro-chat-content");

    connection.start().then(() => {
        console.log("🔗 Connection established.");
    });

    connection.on("SetCurrentResOwnerId", function (id) {
        currentResOwnerId = id;
        connection.invoke("LoadResOwnerRestaurantsWithFollowers");
    });

    connection.on("ReceiveRestaurantsWithFollowers", function (data) {
        const resList = document.getElementById("ro-restaurant-list");
        resList.innerHTML = "";
        data.forEach(res => {
            const div = document.createElement("div");
            div.className = "ro-restaurant-item";
            div.dataset.id = res.res_id;
            div.dataset.name = res.res_name;
            div.dataset.followers = JSON.stringify(res.followers);
            div.textContent = res.res_name;
            resList.appendChild(div);
        });
    });

    document.addEventListener("click", function (e) {
        const resItem = e.target.closest(".ro-restaurant-item");
        if (resItem) {
            // ✅ Đánh dấu restaurant đang active
            document.querySelectorAll(".ro-restaurant-item").forEach(el => el.classList.remove("active"));
            resItem.classList.add("active");

            currentResId = parseInt(resItem.dataset.id);
            const followers = JSON.parse(resItem.dataset.followers);
            const list = document.getElementById("ro-customer-list");
            list.innerHTML = "";
            if (!followers || followers.length === 0) {
                list.innerHTML = "<p class='text-muted'>Không có khách hàng nào.</p>";
                return;
            }
            followers.forEach(c => {
                const div = document.createElement("div");
                div.className = "ro-customer-item" + (c.has_unread ? " has-new-message" : "");
                div.dataset.id = c.cus_id;
                div.dataset.name = c.cus_name;

                const lastMsg = c.last_message?.length > 40 ? c.last_message.substring(0, 37) + "..." : c.last_message;
                const dotClass = c.is_online ? "dot-online" : "dot-offline";

                div.innerHTML = `
                <div class="ro-customer-name">
                    ${c.cus_name}
                    <span class="${dotClass}" title="${c.is_online ? 'Online' : 'Offline'}"></span>
                </div>
                <div class="last-message small text-muted">${lastMsg}</div>
            `;
                list.appendChild(div);
            });
        }

        const cusItem = e.target.closest(".ro-customer-item");
        if (cusItem) {
            // ✅ Gán lại currentResId từ nhà hàng đang active
            const activeRes = document.querySelector(".ro-restaurant-item.active");
            if (activeRes) {
                currentResId = parseInt(activeRes.dataset.id);
            }

            currentCustomerId = parseInt(cusItem.dataset.id);
            resOffset = 10;
            document.getElementById("ro-chat-customer-name").textContent = cusItem.dataset.name;
            document.getElementById("ro-chat-box").dataset.id = currentCustomerId;
            roChatContent.innerHTML = "<p><i>Đang tải tin nhắn...</i></p>";
            cusItem.classList.remove("has-new-message");
            connection.invoke("LoadChatHistoryFromResOwner", currentCustomerId, currentResId);
        }
    });

    document.getElementById("ro-chat-send-btn").addEventListener("click", sendMessage);
    document.getElementById("ro-chat-message-input").addEventListener("keydown", async (e) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            await sendMessage();
        }
    });

    async function sendMessage() {
    const input = document.getElementById("ro-chat-message-input");
    const msg = input.value.trim();
    if (!msg || !currentCustomerId || !currentResId) return;

    // ❌ KHÔNG render bubble ở đây
    // ✅ Gửi cho Hub, để Hub tự phát lại realtime về qua `ReceiveMessage`
    await connection.invoke("SendMessageFromResOwner", currentCustomerId, msg, currentResId);
    input.value = "";
}


    connection.on("ReceiveChatHistoryResOwner", function (payload) {
        const messages = payload?.messages || [];
        const resId = payload?.res_id;
        const resOwnerId = payload?.resowner_id;

        roChatContent.innerHTML = "";
        const lastTen = messages.slice(-10);
        lastTen.forEach(msg => {
            const isMe = msg.message?.startsWith("ro:");
            const displayMsg = isMe ? msg.message.substring(3) : msg.message;
            const div = document.createElement("div");
            div.className = "chat-bubble " + (isMe ? "ro-me" : "ro-you");
            const span = document.createElement("span");
            span.textContent = displayMsg || "[Không có tin nhắn]";
            div.appendChild(span);
            roChatContent.appendChild(div);
        });
        roChatContent.scrollTop = roChatContent.scrollHeight;
    });

    connection.on("ReceiveMessage", function (senderId, message, isResOwnerChat) {
    if (!isResOwnerChat) return;

    const currentId = parseInt(document.getElementById("ro-chat-box").dataset.id);
    const isMe = message?.startsWith("ro:");
    const displayMsg = isMe ? message.substring(3) : message;

    if (senderId === currentId) {
        // ✅ Đang mở đúng khách hàng → hiển thị ngay
        const div = document.createElement("div");
        div.className = "chat-bubble " + (isMe ? "ro-me" : "ro-you");
        const span = document.createElement("span");
        span.textContent = displayMsg || "[Không có tin nhắn]";
        div.appendChild(span);
        roChatContent.appendChild(div);
        roChatContent.scrollTop = roChatContent.scrollHeight;
    } else {
        // 🔴 Nếu chưa mở → cập nhật chấm đỏ + nội dung
        const item = document.querySelector(`.ro-customer-item[data-id="${senderId}"]`);
        if (item) {
            item.classList.add("has-new-message");
            const lastMsgDiv = item.querySelector(".last-message");
            if (lastMsgDiv) {
                lastMsgDiv.textContent = displayMsg.length > 40 ? displayMsg.substring(0, 37) + "..." : displayMsg;
            }
        }

        const sound = document.getElementById("chat-sound");
        if (sound) {
            sound.currentTime = 0;
            sound.play().catch(() => {});
        }
    }
});

    roChatContent.addEventListener("scroll", async () => {
        if (roChatContent.scrollTop === 0 && !isLoadingRes) {
            isLoadingRes = true;
            await connection.invoke("LoadMoreChatHistoryFromResOwner", currentCustomerId, currentResId, resOffset);
            resOffset += 10;
            setTimeout(() => isLoadingRes = false, 300);
        }
    });

    connection.on("ReceiveMoreChatHistoryResOwner", function (payload) {
        const messages = payload?.messages || [];
        const resId = payload?.res_id;
        const resOwnerId = payload?.resowner_id;

        const scrollBefore = roChatContent.scrollHeight;
        messages.forEach(msg => {
            const isMe = msg.message?.startsWith("ro:");
            const displayMsg = isMe ? msg.message.substring(3) : msg.message;
            const div = document.createElement("div");
            div.className = "chat-bubble " + (isMe ? "ro-me" : "ro-you");
            const span = document.createElement("span");
            span.textContent = displayMsg || "[Không có nội dung]";
            div.appendChild(span);
            roChatContent.insertBefore(div, roChatContent.firstChild);
        });
        const scrollAfter = roChatContent.scrollHeight;
        roChatContent.scrollTop = scrollAfter - scrollBefore;
    });
});
document.addEventListener("click", function (e) {
  if (e.target.closest(".ro-restaurant-item")) {
    document.querySelector(".ro-restaurant-sidebar")?.classList.remove("active");
    document.querySelector(".ro-chat-sidebar")?.classList.add("active");
    document.querySelector(".ro-chat-box")?.classList.remove("active");
  }
  if (e.target.closest(".ro-customer-item")) {
    document.querySelector(".ro-chat-sidebar")?.classList.remove("active");
    document.querySelector(".ro-chat-box")?.classList.add("active");
  }
});

document.getElementById("btn-back-to-restaurants")?.addEventListener("click", () => {
  document.querySelector(".ro-chat-sidebar")?.classList.remove("active");
  document.querySelector(".ro-restaurant-sidebar")?.classList.add("active");
});

document.getElementById("btn-back-to-customers")?.addEventListener("click", () => {
  document.querySelector(".ro-chat-box")?.classList.remove("active");
  document.querySelector(".ro-chat-sidebar")?.classList.add("active");
});
