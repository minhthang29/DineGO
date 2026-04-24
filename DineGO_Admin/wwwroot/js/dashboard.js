// Revenue Chart
const revenueCtx = document.getElementById('revenueChart').getContext('2d');
const revenueChart = new Chart(revenueCtx, {
    type: 'line',
    data: {
        labels: [],
        datasets: [{
            label: 'Doanh Thu',
            data: [],
            borderColor: 'rgb(99, 102, 241)',
            backgroundColor: 'rgba(99, 102, 241, 0.1)',
            fill: true,
            tension: 0.3
        }]
    },
    plugins: [ChartDataLabels],
    options: {
        plugins: {
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' },
                formatter: (value) => {
                    return value.toLocaleString('vi-VN');
                }
            }
        }
    }
});
fetch('/api/dashboard/revenue-by-month')
    .then(res => {
        if (res.status === 401) {
            window.location.href = '/Auth/Login';
            return Promise.reject('Unauthorized');
        }
        return res.json();
    })
    .then(data => {
        revenueChart.data.labels = data.labels;
        revenueChart.data.datasets[0].data = data.values;
        revenueChart.update();
    })
    .catch(err => {
        console.error('API error:', err);
    });
// Orders Chart
const ordersCtx = document.getElementById('ordersChart').getContext('2d');
const ordersChart = new Chart(ordersCtx, {
    type: 'bar',
    data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        datasets: [{
            label: 'Orders',
            data: [50, 75, 150, 200, 180, 220],
            backgroundColor: 'rgba(34, 197, 94, 0.5)',
            borderColor: 'rgb(34, 197, 94)',
            borderWidth: 1
        }]
    },
    plugins: [ChartDataLabels],
    options: {
        plugins: {
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' }
            }
        }
    }
});
fetch('/api/dashboard/orders-by-month')
    .then(res => res.json())
    .then(data => {
        ordersChart.data.labels = data.labels;
        ordersChart.data.datasets[0].data = data.values;
        ordersChart.update();
    });
// Customers Chart
const customersCtx = document.getElementById('customersChart').getContext('2d');
const customersChart = new Chart(customersCtx, {
    type: 'pie',
    data: {
        labels: ['New', 'Returning', 'VIP'],
        datasets: [{
            label: 'Customers',
            data: [300, 150, 50],
            backgroundColor: [
                'rgba(59, 130, 246, 0.7)',
                'rgba(251, 191, 36, 0.7)',
                'rgba(239, 68, 68, 0.7)'
            ],
            borderColor: [
                'rgb(59, 130, 246)',
                'rgb(251, 191, 36)',
                'rgb(239, 68, 68)'
            ],
            borderWidth: 1
        }]
    },
    plugins: [ChartDataLabels],
    options: {
        plugins: {
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' },
                formatter: (value, context) => {
                    const data = context.chart.data.datasets[0].data;
                    const total = data.reduce((a, b) => a + b, 0);
                    const percent = total ? (value / total * 100).toFixed(1) : 0;
                    return percent + '%';
                }
            }
        }
    }
});
fetch('/api/dashboard/customer-type')
    .then(res => res.json())
    .then(data => {
        customersChart.data.labels = data.labels;
        customersChart.data.datasets[0].data = data.values;
        customersChart.update();
    });

// Order Status Chart (Doughnut)
const orderStatusCtx = document.getElementById('orderStatusChart').getContext('2d');
const orderStatusChart = new Chart(orderStatusCtx, {
    type: 'doughnut',
    data: {
        labels: ['Đã thanh toán', 'Chờ xác nhận', 'Đã hủy', 'Đang giao'],
        datasets: [{
            label: 'Trạng thái đơn hàng',
            data: [320, 120, 40, 60],
            backgroundColor: [
                'rgba(34,197,94,0.7)',    // Đã thanh toán
                'rgba(59,130,246,0.7)',  // Chờ xác nhận
                'rgba(239,68,68,0.7)',   // Đã hủy
                'rgba(251,191,36,0.7)'   // Đang giao
            ],
            borderColor: [
                'rgb(34,197,94)',
                'rgb(59,130,246)',
                'rgb(239,68,68)',
                'rgb(251,191,36)'
            ],
            borderWidth: 1
        }]
    },
    plugins: [ChartDataLabels],
    options: {
        plugins: {
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' },
                formatter: (value, context) => {
                    const data = context.chart.data.datasets[0].data;
                    const total = data.reduce((a, b) => a + b, 0);
                    const percent = total ? (value / total * 100).toFixed(1) : 0;
                    return percent + '%';
                }
            }
        }
    }
});
fetch('/api/dashboard/order-status')
    .then(res => res.json())
    .then(data => {
        orderStatusChart.data.labels = data.labels;
        orderStatusChart.data.datasets[0].data = data.values;
        orderStatusChart.update();
    });
// Top 5 Nhà hàng doanh thu cao (Horizontal Bar)
const topRestaurantCtx = document.getElementById('topRestaurantChart').getContext('2d');
const topRestaurantChart = new Chart(topRestaurantCtx, {
    type: 'bar',
    data: {
        labels: ['Nhà hàng A', 'Nhà hàng B', 'Nhà hàng C', 'Nhà hàng D', 'Nhà hàng E'],
        datasets: [{
            label: 'Doanh thu (triệu VNĐ)',
            data: [120, 110, 95, 80, 75],
            backgroundColor: [
                'rgba(99,102,241,0.7)',
                'rgba(34,197,94,0.7)',
                'rgba(251,191,36,0.7)',
                'rgba(239,68,68,0.7)',
                'rgba(59,130,246,0.7)'
            ],
            borderWidth: 1
        }]
    },
    plugins: [ChartDataLabels],
    options: {
        indexAxis: 'y',
        plugins: {
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' }
            }
        }
    }
});
fetch('/api/dashboard/top-restaurant')
    .then(res => res.json())
    .then(data => {
        topRestaurantChart.data.labels = data.labels;
        topRestaurantChart.data.datasets[0].data = data.values;
        topRestaurantChart.update();
    });
// So sánh doanh thu các dịch vụ (Stacked Bar)
const serviceRevenueCtx = document.getElementById('serviceRevenueChart').getContext('2d');
const serviceRevenueChart = new Chart(serviceRevenueCtx, {
    type: 'bar',
    data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        datasets: [
            {
                label: 'Đặt món',
                data: [500, 700, 900, 1100, 950, 1200],
                backgroundColor: 'rgba(59,130,246,0.7)'
            },
            {
                label: 'Đặt bàn',
                data: [200, 250, 300, 350, 320, 400],
                backgroundColor: 'rgba(251,191,36,0.7)'
            },
            {
                label: 'Giao hàng',
                data: [100, 150, 200, 250, 220, 300],
                backgroundColor: 'rgba(34,197,94,0.7)'
            }
        ]
    },
    plugins: [ChartDataLabels],
    options: {
        plugins: {
            tooltip: { mode: 'index', intersect: false },
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' }
            }
        },
        responsive: true,
        scales: {
            x: { stacked: true },
            y: { stacked: true }
        }
    }
});
fetch('/api/dashboard/service-revenue')
    .then(res => res.json())
    .then(data => {
        if (!Array.isArray(data) || data.length === 0) return;

        // Lấy labels từ service đầu tiên (giả sử các service cùng labels)
        serviceRevenueChart.data.labels = data[0].labels;
        console.log('Service Revenue Data:', data);
        // Map datasets cho từng service với tên hiển thị mong muốn
        serviceRevenueChart.data.datasets = data.map((srv, idx) => ({
            label:
                srv.service === "Order" ? "Đặt món" :
                srv.service === "Reservation" ? "Đặt bàn" :
                `Dịch vụ ${idx + 1}`,
            data: srv.values,
            backgroundColor:
                srv.service === "Order" ? 'rgba(59,130,246,0.7)' :
                srv.service === "Reservation" ? 'rgba(251,191,36,0.7)' :
                'rgba(34,197,94,0.7)'
        }));

        serviceRevenueChart.update();
    })
    .catch(err => {
        console.error('API error:', err);
    });


// Phân tích khách hàng theo nhóm (Radar)
const customerGroupRadarCtx = document.getElementById('customerGroupRadarChart').getContext('2d');
const customerGroupRadarChart = new Chart(customerGroupRadarCtx, {
    type: 'radar',
    data: {
        labels: ['Mua nhiều', 'Mua thường xuyên', 'Khách mới', 'Khách VIP', 'Khách tiềm năng'],
        datasets: [{
            label: 'Số lượng khách',
            data: [80, 120, 60, 30, 90],
            backgroundColor: 'rgba(99,102,241,0.2)',
            borderColor: 'rgb(99,102,241)',
            pointBackgroundColor: 'rgb(99,102,241)'
        }]
    },
    plugins: [ChartDataLabels],
    options: {
        plugins: {
            datalabels: {
                color: '#fff',
                font: { weight: 'bold' }
            }
        }
    }
});
fetch('/api/dashboard/customer-group')
    .then(res => res.json())
    .then(data => {
        customerGroupRadarChart.data.labels = data.labels;
        customerGroupRadarChart.data.datasets[0].data = data.values;
        customerGroupRadarChart.update();
    });


let modalChartInstance = null;
function maximizeChart(chartId, title) {
    const chart = Chart.getChart(chartId);
    if (!chart) return;
    document.getElementById('chartModal').classList.remove('hidden');
    document.querySelector('#chartModal h3').innerText = title || '';
    // Destroy old modal chart if exists (dù biến modalChartInstance có hay không)
    const modalCanvas = document.getElementById('modalChart');
    const oldModalChart = Chart.getChart(modalCanvas);
    if (oldModalChart) {
        oldModalChart.destroy();
    }
    console.log(JSON.parse(JSON.stringify(chart.config.options)));
    // Clone chart data & options
    const ctx = document.getElementById('modalChart').getContext('2d');
    const clonedOptions = JSON.parse(JSON.stringify(chart.config.options || {}));
    if (chart.config.type === 'doughnut' || chart.config.type === 'pie') {
        if (!clonedOptions.plugins) clonedOptions.plugins = {};
        clonedOptions.plugins.datalabels = {
            color: '#fff',
            font: { weight: 'bold' },
            formatter: (value, context) => {
                const data = context.chart.data.datasets[0].data;
                const total = data.reduce((a, b) => a + b, 0);
                const percent = total ? (value / total * 100).toFixed(1) : 0;
                return percent + '%';
            }
        };
    }else if (chart.config.type === 'line'){
        if (!clonedOptions.plugins) clonedOptions.plugins = {};
        clonedOptions.plugins.datalabels = {
            color: '#fff',
            font: { weight: 'bold' },
            formatter: (value) => {
                return value.toLocaleString('vi-VN');
            }
        };
    }
    window.modalChartInstance = new Chart(ctx, {
        type: chart.config.type,
        data: JSON.parse(JSON.stringify(chart.data)),
        plugins: chart.config.plugins || [ChartDataLabels],
        options: clonedOptions

    });
}
function closeChartModal() {
    document.getElementById('chartModal').classList.add('hidden');
    if (modalChartInstance) {
        modalChartInstance.destroy();
        modalChartInstance = null;
    }
}