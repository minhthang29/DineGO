let currentUserId = 0;
let userReady = false;

let iceQueue = [];
let remoteDescSet = false;


const chatBox = document.getElementById("chat-box");
const chatUser = document.getElementById("chat-user-name");
const chatContent = document.getElementById("chat-content");
const input = document.getElementById("chat-message-input");
const sendBtn = document.getElementById("chat-send-btn");

// UI mới
const chatIconBtn = document.getElementById("chat-icon-btn");
const chatListPanel = document.getElementById("chat-list-panel");
const closeChatListBtn = document.getElementById("close-chat-list");
const chatListBody = document.getElementById("chat-list-body");

let currentOffset = 0;
let isLoadingHistory = false;

chatContent.addEventListener("scroll", async () => {
    if (chatContent.scrollTop === 0 && !isLoadingHistory) {
        isLoadingHistory = true;

        const receiverId = parseInt(chatBox.getAttribute("data-id"));
        await connection.invoke("LoadMoreChatHistory", receiverId, currentOffset);

        // sau khi gọi xong thì tăng offset
        currentOffset += 10;
        setTimeout(() => isLoadingHistory = false, 300); // tránh spam
    }
});

let resOwnerOffset = 0;
let isLoadingResOwner = false;


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { withCredentials: true })
    .build();

connection.start()
    .then(() => console.log("✅ Hub connected"))
    .catch(err => console.error("❌ Hub error:", err));

// ⏳ Nhận ID từ server sau khi connect
connection.on("SetCurrentUserId", function (id) {
    currentUserId = id;
    userReady = true;
});
connection.on("FriendOnline", (id) => updateUserStatus(id, true));
connection.on("FriendOffline", (id) => updateUserStatus(id, false));

// 🔘 Toggle mở danh sách đoạn chat
chatIconBtn.addEventListener("click", () => {
    if (!userReady || !currentUserId) {
        // 👉 Nếu chưa đăng nhập, chuyển đến trang /Auth/Login
        window.location.href = "/Auth/Login";
        return;
    }

    chatListPanel.classList.toggle("d-none");
    chatIconBtn.classList.remove("has-unread");
    connection.invoke("LoadFriendList");
});

closeChatListBtn.addEventListener("click", () => {
    chatListPanel.classList.add("d-none");
});

// 📥 Nhận tin nhắn realtime
connection.on("ReceiveMessage", function (senderId, message, isResOwnerChat) {
    if (isResOwnerChat) return; // ❌ bỏ qua nếu là chat với ResOwner

    if (senderId === 0) {
        const div = document.createElement("div");
        div.className = "chat-bubble system";
        const span = document.createElement("span");
        span.textContent = message;
        div.appendChild(span);
        chatContent.appendChild(div);

        chatContent.scrollTop = chatContent.scrollHeight;
        return;
    }

    // ✅ xử lý UI cho Customer ↔ Customer
    const currentChatId = parseInt(chatBox.getAttribute("data-id"));
    const isChatBoxVisible = !chatBox.classList.contains("chat-hidden");
    const isMe = Number(senderId) === currentUserId;

    if (!isMe && typeof senderId === "number" && (!isChatBoxVisible || currentChatId !== senderId)) {
        const item = document.querySelector(`.chat-item[data-id="${senderId}"]`);
        if (item && !item.classList.contains("has-new-message")) {
            item.classList.add("has-new-message");
            const lastMsg = item.querySelector(".last-message");
            if (lastMsg) lastMsg.textContent = message.length > 50 ? message.substring(0, 47) + "..." : message;
        }

        const icon = document.getElementById("chat-icon-btn");
        if (icon) icon.classList.add("has-unread");

        const sound = document.getElementById("chat-sound");
        if (sound) {
            sound.currentTime = 0;
            sound.play().catch(() => { });
        }

        return;
    }

    const div = document.createElement("div");
    div.className = "chat-bubble " + (isMe ? "me" : "you");
    div.innerHTML = `<span>${message}</span>`;
    chatContent.appendChild(div);
    chatContent.scrollTop = chatContent.scrollHeight;
});

// 📜 Nhận lịch sử chat (giới hạn 10)
connection.on("ReceiveChatHistoryCustomer", function (payload) {
    const messages = Array.isArray(payload) ? payload : payload.messages;
    const isPending = payload.is_pending;

    const content = document.getElementById("chat-content");
    content.innerHTML = "";

    const lastTen = messages.slice(-10);
    lastTen.forEach(msg => {
        const isMe = Number(msg.sender_id) === currentUserId;
        const div = document.createElement("div");
        div.className = "chat-bubble " + (isMe ? "me" : "you");
        const span = document.createElement("span");
        span.textContent = msg.message;
        div.appendChild(span);
        content.appendChild(div);
    });

    if (isPending) {
        const pendingNotice = document.createElement("div");
        pendingNotice.classList.add("chat-pending");
        pendingNotice.innerHTML = `<i>💡 Người này chưa chấp nhận kết bạn. Bạn vẫn có thể gửi tin nhắn.</i>`;
        content.prepend(pendingNotice);
    }

    content.scrollTop = content.scrollHeight;
});


// 👥 Nhận danh sách bạn bè
connection.on("ReceiveFriendList", function (friends) {
    chatListBody.innerHTML = "";

    if (!friends.length) {
        chatListBody.innerHTML = "<div style='padding:8px;color:gray'>Không có bạn bè.</div>";
        return;
    }

    friends.forEach(friend => {
        const div = document.createElement("div");

        const rawMsg = friend.last_message || "Chưa có tin nhắn";
        const shortMsg = rawMsg.length > 20 ? rawMsg.substring(0, 17) + "..." : rawMsg;
        const safeMsg = shortMsg.replace(/</g, "&lt;").replace(/>/g, "&gt;");

        div.className = "chat-item";
        if (friend.has_unread) {
            div.classList.add("has-new-message");
        }
        div.setAttribute("data-id", friend.cus_id);
        div.setAttribute("data-name", friend.cus_name);

        div.innerHTML = `
            <img src="${friend.cus_image || 'https://cdn-icons-png.flaticon.com/512/149/149071.png'}"
                alt="${friend.cus_name}"
                onerror="this.onerror=null;this.src='https://cdn-icons-png.flaticon.com/512/149/149071.png';" />
            <div class="info">
                <div class="name">
                    ${friend.cus_name}
                    <span class="${friend.is_online ? 'dot-online' : 'dot-offline'}"></span>
                </div>
                <div class="last-message">${safeMsg}</div>
            </div>`;

        chatListBody.appendChild(div);
    });
});

// 💬 Mở khung chat
document.addEventListener("click", function (e) {
    const chatItem = e.target.closest(".chat-item");
    if (chatItem) {
        const name = chatItem.dataset.name;
        const id = parseInt(chatItem.dataset.id);
        const isResOwner = chatItem.dataset.isresowner === "true";

        if (isResOwner) {
            // 👉 Chat với ResOwner
            const roBox = document.getElementById("chat-box-resowner");
            document.getElementById("chat-resowner-name").textContent = name;
            roBox.classList.remove("chat-hidden");
            roBox.setAttribute("data-id", id);
            chatListPanel.classList.add("d-none");

            const contentRo = document.getElementById("chat-content-ro");
            contentRo.innerHTML = `<p>💬 Đang chat với <b>${name}</b>...</p>`;

            resOwnerOffset = 10;
            connection.invoke("LoadChatHistoryWithResOwner", id);

            const itemToClear = document.querySelector(`.chat-item[data-id="${id}"][data-isresowner="true"]`);
            if (itemToClear) itemToClear.classList.remove("has-new-message");
            chatIconBtn.classList.remove("has-unread");
        } else {
            // 👉 Chat với bạn bè (Customer)
            chatUser.textContent = name;

            const statusSpan = document.getElementById("chat-user-status");
            const friendDot = chatItem.querySelector(".dot-online");
            if (friendDot) {
                statusSpan.classList.remove("dot-offline");
                statusSpan.classList.add("dot-online");
                statusSpan.title = "Đang online";
            } else {
                statusSpan.classList.remove("dot-online");
                statusSpan.classList.add("dot-offline");
                statusSpan.title = "Đang offline";
            }

            chatBox.classList.remove("chat-hidden");
            chatBox.setAttribute("data-id", id);

            const itemToClear = document.querySelector(`.chat-item[data-id="${id}"]:not([data-isresowner="true"])`);
            if (itemToClear) itemToClear.classList.remove("has-new-message");

            chatContent.innerHTML = `<p>💬 Đang chat với <b>${name}</b>...</p>`;
            currentOffset = 10;
            connection.invoke("LoadChatHistory", id);
            chatListPanel.classList.add("d-none");
        }
    }
});

// 📨 Gửi tin nhắn
sendBtn.addEventListener("click", async () => {
    const receiverId = parseInt(chatBox.getAttribute("data-id"));
    const message = input.value.trim();
    if (receiverId && message) {
        await sendMessageGeneric(receiverId, message);
        input.value = "";
    }
});

// ❌ Đóng khung chat
document.getElementById("chat-close-btn").addEventListener("click", () => {
    chatBox.classList.add("chat-hidden");
});

function updateUserStatus(friendId, isOnline) {
    // 👉 Cập nhật trong danh sách bạn
    const selector = `.chat-item[data-id="${friendId}"] .name .${isOnline ? 'dot-offline' : 'dot-online'}`;
    const friendDot = document.querySelector(selector);
    if (friendDot) {
        friendDot.classList.remove(isOnline ? 'dot-offline' : 'dot-online');
        friendDot.classList.add(isOnline ? 'dot-online' : 'dot-offline');
    }

    // 👉 Cập nhật trong khung chat mini (nếu đang chat với người đó)
    const chatBoxId = document.getElementById("chat-box").getAttribute("data-id");
    if (parseInt(chatBoxId) === friendId) {
        const statusSpan = document.getElementById("chat-user-status");
        statusSpan.classList.remove(isOnline ? 'dot-offline' : 'dot-online');
        statusSpan.classList.add(isOnline ? 'dot-online' : 'dot-offline');
        statusSpan.title = isOnline ? "Đang online" : "Đang offline";
    }
}
// 🔍 Tìm kiếm bạn bè mới qua SignalR Hub
const searchInput = document.getElementById("friend-search-input");
const searchResult = document.getElementById("friend-search-result");

searchInput.addEventListener("input", async () => {
    const keyword = searchInput.value.trim();
    if (keyword.length < 2) {
        searchResult.innerHTML = "";
        return;
    }

    await connection.invoke("SearchUsers", keyword); // 📡 gọi Hub
});

// 📥 Nhận kết quả từ Hub
connection.on("ReceiveSearchResult", function (users) {
    searchResult.innerHTML = "";

    if (!users || users.length === 0) {
        searchResult.innerHTML = "<div style='color: white;' class='px-2'>Không tìm thấy người dùng.</div>";
        return;
    }

    users.forEach(user => {
        // Tạo avatar
        const avatar = document.createElement("img");
        avatar.src = user.cus_image || "https://cdn-icons-png.flaticon.com/512/149/149071.png";
        avatar.alt = user.cus_name;
        avatar.style.width = "38px";
        avatar.style.height = "38px";
        avatar.style.borderRadius = "50%";
        avatar.style.objectFit = "cover";
        avatar.style.marginRight = "10px";
        avatar.onerror = function () {
            this.onerror = null;
            this.src = 'https://cdn-icons-png.flaticon.com/512/149/149071.png';
        };

        // Thông tin user
        const info = document.createElement("div");
        info.style.flex = "1";
        info.innerHTML = '<b>' + user.cus_name + '</b> <small class="text-muted">' + user.cus_username + '</small>';

        // Nút kết bạn
        const btn = document.createElement("button");
        btn.className = "btn btn-sm btn-outline-success";
        btn.textContent = "Kết bạn";

        btn.addEventListener("click", async () => {
            btn.disabled = true;
            btn.textContent = "⏳ Đang gửi...";
            try {
                await connection.invoke("SendFriendRequest", user.cus_id);
                btn.textContent = "Đã gửi";
            } catch (err) {
                console.error("❌ Lỗi gửi lời mời:", err);
                btn.textContent = "Kết bạn lại";
                btn.disabled = false;
            }
        });

        // Wrapper
        const wrapper = document.createElement("div");
        wrapper.className = "search-result-item d-flex align-items-center justify-content-between px-2 py-1 border rounded mb-1";
        wrapper.appendChild(avatar);
        wrapper.appendChild(info);
        wrapper.appendChild(btn);

        searchResult.appendChild(wrapper);
    });
});
// ✅ Xử lý nút ✔️ Chấp nhận kết bạn
document.getElementById("accept-friend-btn").addEventListener("click", async () => {
    const friendId = parseInt(chatBox.getAttribute("data-id"));
    if (!friendId) return;

    await connection.invoke("AcceptFriend", friendId);
    connection.invoke("LoadChatHistory", friendId);
});
// Nhấn Enter để gửi
input.addEventListener("keydown", async (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        const receiverId = parseInt(chatBox.getAttribute("data-id"));
        const message = input.value.trim();
        if (receiverId && message) {
            await sendMessageGeneric(receiverId, message);
            input.value = "";
        }
    }
});

// ========== VIDEO CALL POPUP LOGIC ==========

let localStream, peerConnection, targetUserId;
const videoPopup = document.getElementById("video-call-popup");
const incomingPopup = document.getElementById("incoming-popup");

// Hàm dừng nhạc chuông an toàn
function stopRingtone() {
    try {
        const ringtone = document.getElementById("call-ringtone");
        ringtone.pause();
        ringtone.currentTime = 0;
    } catch (e) { }
}

// Hiện thông báo trạng thái gọi
function showCallStatusMsg(msg) {
    const statusMsg = document.getElementById("call-status-msg");
    statusMsg.textContent = msg;
    statusMsg.classList.remove("d-none");
    setTimeout(() => statusMsg.classList.add("d-none"), 3000);
}

// Khi bị từ chối hoặc kết thúc call
connection.on("CallRejected", function (fromUserId) {
    stopRingtone();
    videoPopup.classList.remove("d-none");
    showCallStatusMsg("❌ Người nhận đã từ chối cuộc gọi.");
    setTimeout(() => { videoPopup.classList.add("d-none"); }, 2000);
    if (peerConnection) peerConnection.close();
    if (localStream) localStream.getTracks().forEach(t => t.stop());
});
connection.on("CallEnded", function (fromUserId) {
    stopRingtone();
    showCallStatusMsg("⛔ Cuộc gọi đã kết thúc.");
    setTimeout(() => { videoPopup.classList.add("d-none"); }, 2000);
    if (peerConnection) peerConnection.close();
    if (localStream) localStream.getTracks().forEach(t => t.stop());
});

// Nhấn nút gọi video
document.getElementById("start-video-btn").addEventListener("click", async () => {
    targetUserId = parseInt(document.getElementById("chat-box").dataset.id);
    if (!targetUserId) return;

    await setupPeer(true); // Người gọi
    const offer = await peerConnection.createOffer();
    await peerConnection.setLocalDescription(offer);
    connection.invoke("CallUser", targetUserId, JSON.stringify(offer));
});

// Khi nhận cuộc gọi đến
connection.on("ReceiveCall", async (fromUserId, offer) => {
    targetUserId = fromUserId;
    document.getElementById("caller-name").textContent = "Người dùng " + fromUserId;
    videoPopup.classList.remove("d-none");
    incomingPopup.classList.remove("d-none");

    // 🔔 Phát nhạc chuông
    try {
        const ringtone = document.getElementById("call-ringtone");
        ringtone.currentTime = 0;
        ringtone.play();
    } catch (e) { }

    // Khi nhận cuộc gọi đến (offer)
    document.getElementById("accept-call-btn").onclick = async () => {
        incomingPopup.classList.add("d-none");
        stopRingtone();
        await setupPeer(false);
        await peerConnection.setRemoteDescription(new RTCSessionDescription(JSON.parse(offer)));
        remoteDescSet = true;
        for (let c of iceQueue) {
            try { await peerConnection.addIceCandidate(c); } catch (e) { console.error(e); }
        }
        iceQueue = [];
        const answer = await peerConnection.createAnswer();
        await peerConnection.setLocalDescription(answer);
        connection.invoke("AnswerCall", fromUserId, JSON.stringify(answer));
    };

    // Khi nhận answer từ bên kia
    connection.on("ReceiveAnswer", async (fromUserId, answer) => {
        await peerConnection.setRemoteDescription(new RTCSessionDescription(JSON.parse(answer)));
        remoteDescSet = true;
        for (let c of iceQueue) {
            try { await peerConnection.addIceCandidate(c); } catch (e) { console.error(e); }
        }
        iceQueue = [];
    });

    document.getElementById("reject-call-btn").onclick = () => {
        stopRingtone();
        connection.invoke("RejectCall", targetUserId);
        videoPopup.classList.add("d-none");
        if (peerConnection) peerConnection.close();
        if (localStream) localStream.getTracks().forEach(t => t.stop());
    };
});

// Nhận answer từ bên kia
connection.on("ReceiveAnswer", async (fromUserId, answer) => {
    await peerConnection.setRemoteDescription(new RTCSessionDescription(JSON.parse(answer)));
});

// Nhận ICE candidate
connection.on("ReceiveIceCandidate", async (fromUserId, candidate) => {
    if (!peerConnection) return;
    const rtcCandidate = new RTCIceCandidate(JSON.parse(candidate));
    if (remoteDescSet) {
        try {
            await peerConnection.addIceCandidate(rtcCandidate);
        } catch (e) {
            console.error("❌ ICE error:", e);
        }
    } else {
        iceQueue.push(rtcCandidate); // Chưa setRemoteDescription thì queue lại
    }
});


// Hàm setup peer connection & local stream
async function setupPeer(isCaller) {
    videoPopup.classList.remove("d-none");
    document.getElementById("call-status-msg").classList.add("d-none");
    document.getElementById("toggle-cam-btn").textContent = "🎥 Bật Camera";
    isCamOn = false;

    // Lấy ICE từ Xirsys (bất đồng bộ, chờ xong mới tạo peer)
    const iceServers = await getIceServersFromXirsys();

    peerConnection = new RTCPeerConnection({ iceServers });

    // CHỈ audio mặc định
    localStream = await navigator.mediaDevices.getUserMedia({ video: false, audio: true });
    document.getElementById("localVideo").srcObject = localStream;
    document.getElementById("localVideo").style.display = "none";
    document.getElementById("local-avatar").style.display = "block";

    localStream.getTracks().forEach(track => peerConnection.addTrack(track, localStream));

    peerConnection.ontrack = (event) => {
        console.log("ontrack received:", event.streams, event.track);
        if (event.streams[0].getVideoTracks().length > 0) {
            document.getElementById("remoteVideo").srcObject = event.streams[0];
            document.getElementById("remoteVideo").style.display = "block";
            document.getElementById("remote-avatar").style.display = "none";
        } else {
            document.getElementById("remoteVideo").style.display = "none";
            document.getElementById("remote-avatar").style.display = "block";
        }
    };

    peerConnection.onicecandidate = (event) => {
    if (event.candidate) {
        console.log("Local ICE candidate:", event.candidate);
        connection.invoke("SendIceCandidate", targetUserId, JSON.stringify(event.candidate));
    }
};

}

// Nút kết thúc cuộc gọi (bất kể bên nào)
document.getElementById("end-call-btn").onclick = () => {
    document.getElementById("end-call-btn").disabled = true;
    setTimeout(() => {
        document.getElementById("end-call-btn").disabled = false;
    }, 1500);
    stopRingtone();
    connection.invoke("EndCall", targetUserId);
    if (peerConnection) peerConnection.close();
    if (localStream) localStream.getTracks().forEach(t => t.stop());
    videoPopup.classList.add("d-none");
};

document.getElementById("close-video-btn").onclick = () => {
    stopRingtone();
    connection.invoke("EndCall", targetUserId);
    if (peerConnection) peerConnection.close();
    if (localStream) localStream.getTracks().forEach(t => t.stop());
    videoPopup.classList.add("d-none");
};

// Nút bật/tắt camera
let isCamOn = true;
document.getElementById("toggle-cam-btn").onclick = async function () {
    if (!localStream) return;
    isCamOn = !isCamOn;
    if (isCamOn) {
        // Thêm video track
        let videoStream = await navigator.mediaDevices.getUserMedia({ video: true });
        let videoTrack = videoStream.getVideoTracks()[0];
        peerConnection.getSenders().forEach(sender => {
            if (sender.track.kind === "video") sender.replaceTrack(videoTrack);
        });
        localStream.addTrack(videoTrack);
        document.getElementById("localVideo").srcObject = localStream;
        document.getElementById("localVideo").style.display = "block";
        document.getElementById("local-avatar").style.display = "none";
        this.textContent = "🎥 Tắt Camera";
    } else {
        // Tắt video track
        localStream.getVideoTracks().forEach(track => {
            track.stop();
            localStream.removeTrack(track);
        });
        document.getElementById("localVideo").style.display = "none";
        document.getElementById("local-avatar").style.display = "block";
        this.textContent = "🎥 Bật Camera";
    }
};

document.getElementById("tab-friend").onclick = () => {
    document.getElementById("tab-friend").classList.add("active");
    document.getElementById("tab-resowner").classList.remove("active");

    document.getElementById("chat-list-body").classList.remove("d-none");
    document.getElementById("chat-list-resowner-body").classList.add("d-none");
};

document.getElementById("tab-resowner").onclick = () => {
    document.getElementById("tab-resowner").classList.add("active");
    document.getElementById("tab-friend").classList.remove("active");

    document.getElementById("chat-list-body").classList.add("d-none");
    document.getElementById("chat-list-resowner-body").classList.remove("d-none");

    // Nếu lần đầu load danh sách ResOwner
    connection.invoke("LoadResOwnerList");
};
const chatListResOwnerBody = document.getElementById("chat-list-resowner-body");

connection.on("ReceiveResOwnerList", function (list) {
    chatListResOwnerBody.innerHTML = "";

    if (!list.length) {
        chatListResOwnerBody.innerHTML = "<div style='padding:8px;color:gray'>Không có nhà hàng nào.</div>";
        return;
    }

    list.forEach(ro => {
        const div = document.createElement("div");
        const rawMsg = ro.last_message || "Chưa có tin nhắn";
        const shortMsg = rawMsg.length > 20 ? rawMsg.substring(0, 17) + "..." : rawMsg;
        const safeMsg = shortMsg.replace(/</g, "&lt;").replace(/>/g, "&gt;");
        div.className = "chat-item";
        if (ro.has_unread) div.classList.add("has-new-message");

        div.setAttribute("data-id", ro.res_owner_id);
        div.setAttribute("data-name", ro.res_owner_name);
        div.setAttribute("data-isresowner", "true");

        div.innerHTML = `
            <img src="/common/images/logo.png" alt="RO" style="width:38px;height:38px;border-radius:50%;margin-right:10px" />
            <div class="info">
                <div class="name">${ro.res_owner_name}</div>
                <div class="last-message">${safeMsg}</div>
            </div>
        `;
        chatListResOwnerBody.appendChild(div);
    });
});

document.getElementById("chat-send-btn-ro").addEventListener("click", async () => {
    const box = document.getElementById("chat-box-resowner");
    const id = parseInt(box.getAttribute("data-id"));
    const msg = document.getElementById("chat-message-input-ro").value.trim();
    if (id && msg) {
        await sendMessageGeneric(id, msg, true);
        document.getElementById("chat-message-input-ro").value = "";
    }
});
document.getElementById("chat-message-input-ro").addEventListener("keydown", async (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        const box = document.getElementById("chat-box-resowner");
        const id = parseInt(box.getAttribute("data-id"));
        const msg = document.getElementById("chat-message-input-ro").value.trim();
        if (id && msg) {
            await sendMessageGeneric(id, msg, true);
            document.getElementById("chat-message-input-ro").value = "";
        }
    }
});

document.getElementById("chat-close-btn-ro").addEventListener("click", () => {
    document.getElementById("chat-box-resowner").classList.add("chat-hidden");
});
connection.on("ReceiveChatHistoryResOwner", function (payload) {
    const messages = Array.isArray(payload) ? payload : payload.messages;
    const content = document.getElementById("chat-content-ro");
    content.innerHTML = "";

    const lastTen = messages.slice(-10);
    lastTen.forEach(msg => {
        const isMe = Number(msg.sender_id) === currentUserId; // người gửi là mình
        const div = document.createElement("div");
        div.className = "chat-bubble " + (isMe ? "me" : "you");
        const span = document.createElement("span");
        span.textContent = msg.message;
        div.appendChild(span);

        content.appendChild(div);
    });

    content.scrollTop = content.scrollHeight;
});
async function sendMessageGeneric(targetId, message, isResOwner = false) {
    if (!targetId || !message) return;

    const method = isResOwner ? "SendMessageToResOwner" : "SendMessage";
    await connection.invoke(method, targetId, message);
}
connection.on("ReceiveMoreChatHistory", function (messages) {
    const scrollBefore = chatContent.scrollHeight;

    messages.forEach(msg => {
        const isMe = Number(msg.sender_id) === currentUserId;
        const div = document.createElement("div");
        div.className = "chat-bubble " + (isMe ? "me" : "you");
        const span = document.createElement("span");
        span.textContent = msg.message;
        div.appendChild(span);
        chatContent.insertBefore(div, chatContent.firstChild);
    });

    // 👇 Giữ nguyên vị trí scroll sau khi prepend
    const scrollAfter = chatContent.scrollHeight;
    chatContent.scrollTop = scrollAfter - scrollBefore;
});
const chatContentRo = document.getElementById("chat-content-ro");

chatContentRo.addEventListener("scroll", async () => {
    if (chatContentRo.scrollTop === 0 && !isLoadingResOwner) {
        isLoadingResOwner = true;

        const resOwnerId = parseInt(document.getElementById("chat-box-resowner").getAttribute("data-id"));
        await connection.invoke("LoadMoreChatHistoryResOwner", resOwnerId, resOwnerOffset);

        resOwnerOffset += 10;
        setTimeout(() => isLoadingResOwner = false, 300);
    }
});
connection.on("ReceiveMoreChatHistoryResOwner", function (messages) {
    const scrollBefore = chatContentRo.scrollHeight;

    messages.forEach(msg => {
        const isMe = Number(msg.sender_id) === currentUserId;
        const div = document.createElement("div");
        div.className = "chat-bubble " + (isMe ? "me" : "you"); // hoặc "ro-me"/"ro-you" nếu có CSS riêng
        const span = document.createElement("span");
        span.textContent = msg.message;
        div.appendChild(span);
        chatContentRo.insertBefore(div, chatContentRo.firstChild);
    });

    const scrollAfter = chatContentRo.scrollHeight;
    chatContentRo.scrollTop = scrollAfter - scrollBefore;
});

// Thay thế hoàn toàn hàm này
async function getIceServersFromXirsys() {
    // Không cần fetch gì nữa, trả về cấu hình TURN của Metered luôn
    return [
        {
            urls: [
                "stun:openrelay.metered.ca:80",
                "turn:openrelay.metered.ca:80",
                "turn:openrelay.metered.ca:443",
                "turn:openrelay.metered.ca:443?transport=tcp",
                "turn:openrelay.metered.ca:80?transport=tcp"
            ],
            username: "openrelayproject",
            credential: "openrelayproject"
        }
    ];
}