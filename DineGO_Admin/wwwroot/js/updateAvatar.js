function openAvatarSettingModal(e) {
    e.preventDefault();
    document.getElementById('avatarSettingModal').classList.remove('hidden');
}
function closeAvatarSettingModal() {
    document.getElementById('avatarSettingModal').classList.add('hidden');
}
async function selectAvatar(avatarUrl) {
    const res = await fetch('/api/admin/update-avatar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ad_image: avatarUrl })
    });
    if (res.ok) {
        document.querySelector('#sidebar img[alt="Admin Avatar"]').src = "images/Avatar/" + avatarUrl;
        closeAvatarSettingModal();
        showNotification('Cập nhật avatar thành công!', 'success');
    } else {
        alert('Cập nhật avatar thất bại!');
    }
}
function showNotification(message, type) {
    const box = document.createElement('div');
    box.className = `notification-box fixed top-6 right-6 z-50 flex flex-col gap-2 px-6 py-4 rounded-lg shadow-lg min-w-[250px] ${type === 'success' ? 'bg-green-100 border-green-400 text-green-800' : 'bg-red-100 border-red-400 text-red-800'}`;
    box.innerHTML = `
        <div class="flex items-center gap-3">
            <svg class="w-6 h-6 text-green-500" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"/>
            </svg>
            <span class="font-semibold">${message}</span>
            
        </div>
        <div class="progress-bar h-1 bg-green-400 rounded transition-all duration-200" style="width:0%"></div>
        `;
    document.body.appendChild(box);

    const progress = box.querySelector('.progress-bar');
    setTimeout(() => {
        progress.style.width = '100%';
        progress.style.transition = 'width 2.5s linear';
    }, 10);

    // Tự động ẩn sau 2.7s
    setTimeout(() => {
        box.classList.add('opacity-0', 'pointer-events-none');
        setTimeout(() => box.remove(), 300);
    }, 2700);

    // Click để đóng ngay
    box.addEventListener('click', () => {
        box.classList.add('opacity-0', 'pointer-events-none');
        setTimeout(() => box.remove(), 300);
    });
}