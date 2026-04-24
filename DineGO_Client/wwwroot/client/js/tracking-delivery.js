document.addEventListener('DOMContentLoaded', function () {
    const tabs = document.querySelectorAll('.order-status-tabs .nav-link');
    const cards = document.querySelectorAll('.delivery-card');

    tabs.forEach(tab => {
        tab.addEventListener('click', function (e) {
            e.preventDefault();
            tabs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');
            const status = this.getAttribute('data-status');
            cards.forEach(card => {
                if (status === 'all' || card.getAttribute('data-status') === status) {
                    card.style.display = '';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
}); 