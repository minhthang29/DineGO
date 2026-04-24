document.addEventListener("DOMContentLoaded", async function () {
    // Wait for auth processing to complete
    await new Promise(resolve => setTimeout(resolve, 100));
    
    const notiBtn = document.getElementById("notification-icon-btn");
    const notiPanel = document.getElementById("notification-panel");
    const closeNotiBtn = document.getElementById("close-notification-panel");
    const notificationListBody = document.getElementById("notification-list-body");
    const notificationBadge = document.getElementById("notificationBadge");
    
    // Get cusId from localStorage (after auth processing)
    const cusId = localStorage.getItem("cus_id");
    // 👇 Không cần API base URL nữa, gọi qua Controller của Client
    let hubUrl = "https://localhost:5001/notificationHub"; // 👈 Chỉ giữ SignalR Hub URL

    console.log('NotificationIcon.js loaded with cusId:', cusId);

    // Toggle panel
    if (notiBtn && notiPanel) {
        notiBtn.addEventListener("click", function () {
            notiPanel.classList.toggle("d-none");
            notiPanel.style.display = notiPanel.classList.contains("d-none") ? "none" : "block";
        });
    }
    
    if (closeNotiBtn && notiPanel) {
        closeNotiBtn.addEventListener("click", function () {
            notiPanel.classList.add("d-none");
            notiPanel.style.display = "none";
        });
    }
    
    document.addEventListener("click", function (e) {
        if (notiPanel && notiBtn && !notiPanel.contains(e.target) && !notiBtn.contains(e.target)) {
            notiPanel.classList.add("d-none");
            notiPanel.style.display = "none";
        }
    });

    // Tab switching
    const tabAll = document.getElementById("tab-all");
    const tabUnread = document.getElementById("tab-unread");
    
    if (tabAll) {
        tabAll.addEventListener("click", function() {
            this.classList.add("active");
            if (tabUnread) tabUnread.classList.remove("active");
            fetchAndRenderNotifications();
        });
    }
    
    if (tabUnread) {
        tabUnread.addEventListener("click", function() {
            this.classList.add("active");
            if (tabAll) tabAll.classList.remove("active");
            fetchAndRenderNotifications(true);
        });
    }

    // 👇 Update notification badge
    function updateNotificationBadge(unreadCount) {
        if (!notificationBadge) return;
        
        if (unreadCount > 0) {
            notificationBadge.textContent = unreadCount > 99 ? "99+" : unreadCount;
            notificationBadge.style.display = "inline-block";
        } else {
            notificationBadge.style.display = "none";
        }
    }

    // 👇 Format date
    function formatDate(dateStr) {
        if (!dateStr) return "";
        const d = new Date(dateStr);
        return d.toLocaleString("vi-VN", { 
            hour: '2-digit', 
            minute: '2-digit', 
            day: '2-digit', 
            month: '2-digit', 
            year: 'numeric' 
        });
    }

    // 👇 Show success toast
    function showSuccessToast(message) {
        console.log('Success:', message);
        // Tạm thời dùng console log, sau này có thể thêm toast library
        // Hoặc tạo custom toast element
    }

    // 👇 Show error toast
    function showErrorToast(message) {
        console.error('Error:', message);
        // Tạm thời dùng console error
    }

    // 👇 Fetch notifications qua Client Controller
    async function fetchAndRenderNotifications(onlyUnread = false) {
        if (!cusId || !notificationListBody) {
            console.log('Cannot fetch notifications: missing cusId or container');
            return;
        }
        
        try {
            // 👈 Gọi qua Client Controller
            const response = await fetch(`/Notification/GetLatest?cusId=${cusId}`, {
                method: 'GET',
                headers: { 
                    'Accept': 'application/json'
                }
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const notifications = await response.json();
            console.log('Fetched notifications:', notifications);

            // Update badge
            const unreadCount = notifications.filter(n => !n.noti_is_read).length;
            updateNotificationBadge(unreadCount);

            // Render list
            notificationListBody.innerHTML = "";
            let filtered = onlyUnread ? notifications.filter(n => !n.noti_is_read) : notifications;
            
            if (filtered.length === 0) {
                notificationListBody.innerHTML = `<div class="text-center text-muted py-3">Không có thông báo nào.</div>`;
                return;
            }
            
            filtered.forEach(n => {
                // 👇 Tạo notification item với click handler
                const notificationItem = document.createElement('div');
                notificationItem.className = `notification-item ${n.noti_is_read ? "" : "unread"} d-flex align-items-start gap-2`;
                notificationItem.style.cursor = 'pointer';
                notificationItem.dataset.notiId = n.noti_id;
                notificationItem.dataset.notiAction = n.noti_action || '';
                
                notificationItem.innerHTML = `
                    <img src="/common/images/logo.png" class="rounded-circle" width="40" height="40" />
                    <div class="flex-grow-1">
                        <div>${n.noti_title ? `<b>${n.noti_title}</b><br/>` : ""}${n.noti_content}</div>
                        <div class="d-flex align-items-center gap-2 mt-1">
                            <span class="badge bg-primary" style="font-size:10px;"><i class="fas fa-bell"></i></span>
                            <span class="text-secondary" style="font-size:12px;">${formatDate(n.noti_date)}</span>
                        </div>
                    </div>
                    ${!n.noti_is_read ? `<span class="ms-auto mt-2"><i class="fas fa-circle text-primary" style="font-size:10px;"></i></span>` : ""}
                `;
                
                // 👇 Add click event listener
                notificationItem.addEventListener('click', function() {
                    handleNotificationClick(n.noti_id, n.noti_action, n.noti_is_read);
                });
                
                notificationListBody.appendChild(notificationItem);
            });
        } catch (error) {
            console.error('Error fetching notifications:', error);
            if (notificationListBody) {
                notificationListBody.innerHTML = `<div class="text-danger text-center py-3">Lỗi khi tải thông báo: ${error.message}</div>`;
            }
            updateNotificationBadge(0);
        }
    }

    // 👇 Handle notification click
    async function handleNotificationClick(notiId, notiAction, isRead) {
        try {
            console.log(`Clicking notification ${notiId}, isRead: ${isRead}`);
            
            // Đánh dấu đã đọc nếu chưa đọc
            if (!isRead) {
                await markAsRead(notiId);
            }
            
            // Xử lý action link
            if (notiAction) {
                handleNotificationAction(notiAction);
            }
            
            // Refresh notification list
            fetchAndRenderNotifications();
            
        } catch (error) {
            console.error('Error handling notification click:', error);
        }
    }

    // 👇 Handle notification action
    function handleNotificationAction(action) {
        if (!action) return;
        
        try {
            console.log('Handling notification action:', action);
            
            // Parse action string - có thể là URL hoặc JSON
            if (action.startsWith('http') || action.startsWith('/')) {
                // Direct URL
                window.location.href = action;
            } else if (action.startsWith('{')) {
                // JSON action
                const actionData = JSON.parse(action);
                handleActionData(actionData);
            } else {
                // Simple action string
                handleSimpleAction(action);
            }
        } catch (error) {
            console.error('Error parsing notification action:', error);
            // Fallback: treat as URL
            if (action.includes('/')) {
                window.location.href = action;
            }
        }
    }

    // 👇 Handle JSON action data
    function handleActionData(actionData) {
        switch (actionData.type) {
            case 'redirect':
                if (actionData.newTab) {
                    window.open(actionData.url, '_blank');
                } else {
                    window.location.href = actionData.url;
                }
                break;
                
            case 'order_details':
                window.location.href = `/Customer/OrderDetails/${actionData.orderId}`;
                break;
                
            case 'tracking':
                window.location.href = `/Customer/TrackingDelivery`;
                break;
                
            case 'promotion':
                window.location.href = `/Home/Promotions/${actionData.promoId}`;
                break;
                
            default:
                console.warn('Unknown action type:', actionData.type);
                if (actionData.url) {
                    window.location.href = actionData.url;
                }
        }
    }

    // 👇 Handle simple action string
    function handleSimpleAction(action) {
        switch (action.toLowerCase()) {
            case 'order_tracking':
                window.location.href = '/Customer/TrackingDelivery';
                break;
                
            case 'profile':
                window.location.href = '/Customer/Profile';
                break;
                
            case 'cart':
                window.location.href = '/Cart';
                break;
                
            default:
                console.warn('Unknown simple action:', action);
        }
    }

    // 👇 Mark notification as read qua Client Controller
    async function markAsRead(notiId) {
        try {
            console.log(`Marking notification ${notiId} as read for customer ${cusId}`);
            
            const response = await fetch('/Notification/MarkAsRead', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify({
                    notiId: parseInt(notiId),
                    cusId: parseInt(cusId)
                })
            });
            
            const result = await response.json();
            console.log('Mark as read result:', result);
            
            if (result.success) {
                console.log('Notification marked as read successfully:', notiId);
            } else {
                console.error('Failed to mark as read:', result.message);
            }
            
        } catch (error) {
            console.error('Error marking notification as read:', error);
        }
    }

    // 👇 Mark all notifications as read qua Client Controller
    async function markAllAsRead() {
        try {
            console.log(`Marking all notifications as read for customer ${cusId}`);
            
            const response = await fetch(`/Notification/MarkAllAsRead/${cusId}`, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json'
                }
            });
            
            const result = await response.json();
            console.log('Mark all as read result:', result);
            
            if (result.success) {
                console.log(`Marked ${result.markedCount} notifications as read`);
                fetchAndRenderNotifications(); // Refresh list
                
                // Show success message
                if (result.markedCount > 0) {
                    showSuccessToast(`Đã đánh dấu ${result.markedCount} thông báo là đã đọc`);
                } else {
                    showSuccessToast('Tất cả thông báo đã được đọc');
                }
            } else {
                console.error('Failed to mark all as read:', result.message);
                showErrorToast('Không thể đánh dấu thông báo đã đọc');
            }
            
        } catch (error) {
            console.error('Error marking all notifications as read:', error);
            showErrorToast('Lỗi khi đánh dấu thông báo đã đọc');
        }
    }

    // 👇 Show notification toast (for new notifications via SignalR)
    function showNotificationToast(data) {
        const toast = document.getElementById("notification-toast");
        const toastContent = document.getElementById("notification-toast-content");
        
        if (!toast || !toastContent) {
            console.log('Notification toast elements not found');
            return;
        }
        
        toastContent.innerHTML = `
            <img src="${data.image || '/common/images/logo.png'}" class="notification-toast-img" alt="Thông báo" />
            <div class="notification-toast-main">
                <div class="notification-toast-title">${data.title || "Thông báo mới"}</div>
                <div class="notification-toast-desc">${data.content || ""}</div>
                ${data.link ? `<a href="${data.link}" target="_blank" class="notification-toast-link">${data.linkText || "Xem chi tiết"}</a>` : ""}
                <div class="notification-toast-date">${formatDate(data.date)}</div>
            </div>
        `;
        
        toast.classList.add("show");
        toast.classList.remove("d-none");

        // Auto hide after 4 seconds
        setTimeout(() => {
            toast.classList.remove("show");
            toast.classList.add("d-none");
        }, 10000);
    }

    // 👇 SignalR setup
    if (window.signalR && cusId) {
        try {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl + `?userId=${cusId}`)
                .build();

            connection.on("ReceiveNotification", function (data) {
                console.log('Received notification via SignalR:', data);
                showNotificationToast(data);
                fetchAndRenderNotifications(); // Refresh notification list
            });

            connection.start()
                .then(() => console.log('SignalR connected to:', hubUrl))
                .catch(err => console.error("SignalR connection error:", err));
        } catch (error) {
            console.error('SignalR setup error:', error);
        }
    } else {
        console.log('SignalR not available or no customer ID');
    }

    // 👇 Expose functions globally
    window.markAllAsRead = markAllAsRead;

    // Initial load
    if (cusId) {
        fetchAndRenderNotifications();
    } else {
        console.log('No customer ID found, skipping notification fetch');
        if (notificationListBody) {
            notificationListBody.innerHTML = '<div class="text-center text-muted py-3">Vui lòng đăng nhập để xem thông báo.</div>';
        }
    }
});