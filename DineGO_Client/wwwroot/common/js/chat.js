const helloMessage = "Xin Chào! Tôi là AI Bot của DineGO. Bạn cần giúp đỡ gì?";
let indexNumberChat = 0;
let aiConnection;

function toggleChat() {
    const chatDialog = document.getElementById("chat-ai-dialog");
    if (chatDialog.style.display === "none" || chatDialog.style.display === "") {
        chatDialog.style.display = "block";

        if (!aiConnection) {
            initializeAIConnection();
        }

        const typingEl = document.getElementById("typing-message" + indexNumberChat);
        if (typingEl) {
            simulateTypingEffect(helloMessage, typingEl.querySelector("p"), () => {
                addQuickSuggestions(); // 👈 thêm gợi ý sau khi gõ xong
            });
            indexNumberChat++;
        }
    } else {
        chatDialog.style.display = "none";
    }
}

function simulateTypingEffect(text, element, callback) {
    let index = 0;
    function type() {
        if (index < text.length) {
            element.textContent += text.charAt(index);
            index++;
            setTimeout(type, 50);
        } else if (callback) {
            callback();
        }
    }
    type();
}

function addQuickSuggestions() {
    const chatBody = document.querySelector(".chat-ai-body");
    const suggestionDiv = document.createElement("div");
    suggestionDiv.classList.add("message", "response-message");
    suggestionDiv.innerHTML = `
        <button class="quick-suggest">Bạn có thể đề xuất món</button>
        <button class="quick-suggest">Tôi muốn món cay</button>
    `;
    chatBody.appendChild(suggestionDiv);

    // Gửi text khi bấm vào
    suggestionDiv.querySelectorAll(".quick-suggest").forEach(btn => {
        btn.addEventListener("click", () => {
            const text = btn.textContent;
            document.querySelector(".chat-ai-footer input").value = text;
            document.querySelector(".chat-ai-footer button").click();
            suggestionDiv.remove(); // ẩn sau khi chọn
        });
    });
}

document.querySelector(".chat-ai-footer button").addEventListener("click", function () {
    const input = document.querySelector(".chat-ai-footer input");
    const message = input.value.trim();
    if (!message) return;

    const chatBody = document.querySelector(".chat-ai-body");

    const userMessage = document.createElement("div");
    userMessage.classList.add("message", "user-message");
    userMessage.innerHTML = `<p>${message}</p>`;
    chatBody.appendChild(userMessage);
    chatBody.scrollTop = chatBody.scrollHeight;

    const loadingDiv = document.createElement("div");
    loadingDiv.classList.add("message", "response-message");
    loadingDiv.id = "ai-loading-message";
    const loadingP = document.createElement("p");
    loadingP.innerHTML = `<strong>🤖 </strong><span id="dot-loading">.</span>`;
    loadingDiv.appendChild(loadingP);
    chatBody.appendChild(loadingDiv);
    chatBody.scrollTop = chatBody.scrollHeight;

    startDotLoading();

    if (aiConnection) {
        aiConnection.invoke("SendAIMessage", message);
    }

    input.value = "";
});

let dotInterval;
function startDotLoading() {
    const dot = document.getElementById("dot-loading");
    let count = 1;
    dotInterval = setInterval(() => {
        count = (count % 3) + 1;
        dot.textContent = '.'.repeat(count);
    }, 400);
}
function stopDotLoading() {
    clearInterval(dotInterval);
    const loadingEl = document.getElementById("ai-loading-message");
    if (loadingEl) loadingEl.remove();
}

function initializeAIConnection() {
    aiConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    aiConnection.start()
        .then(() => console.log("✅ AI Hub connected"))
        .catch(err => console.error("❌ Hub error:", err));

    aiConnection.on("ReceiveAISuggestion", function (result) {
        stopDotLoading();

        const chatBody = document.querySelector(".chat-ai-body");
        const responseDiv = document.createElement("div");
        responseDiv.classList.add("message", "response-message");

        let html = `<p><strong>🤖 ${result.response}</strong></p>`;

        if (result.foods && result.foods.length > 0) {
            html += "<ul style='padding-left: 20px; margin-top: 8px'>";
            result.foods.forEach(f => {
                html += `<li><a href="/Food/Details/${f.food_id}" target="_blank">${f.food_name}</a></li>`;
            });
            html += "</ul>";
        }

        responseDiv.innerHTML = html;
        chatBody.appendChild(responseDiv);
        chatBody.scrollTop = chatBody.scrollHeight;
    });
}
