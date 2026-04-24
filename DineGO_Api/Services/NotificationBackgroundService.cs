using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Models;
using DineGO_Api.Data;
using DineGO_Api.SignalRHub;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationBackgroundService> logger,
            IHubContext<NotificationHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    if (!await db.Database.CanConnectAsync(stoppingToken))
                    {
                        _logger.LogWarning("Database not ready, retrying in 1 minute...");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                        continue;
                    }

                    // *** NEW: Check spam customers ***
                    await CheckSpamCustomers(db, stoppingToken);

                    // Existing notification logic...
                    await ProcessWelcomeNotifications(db, stoppingToken);
                    await ProcessScheduleNotifications(db, stoppingToken);
                    await ProcessNormalNotifications(db, stoppingToken);
                    await ProcessPaymentNotifications(db, stoppingToken);

                    var now = DateTime.Now;
                    // Xử lý notification loại "reservation reminder" (nhắc khách trước giờ ăn 30 phút) _ ThangTM
                    var reservations = await db.Reservations
                        .Include(r => r.restaurant)
                        .Where(r => r.reser_status == 1 // trạng thái đã xác nhận
                                    && r.reser_date > now   // chưa tới giờ
                                    && r.reser_date <= now.AddMinutes(30)) // trong vòng 30 phút tới
                        .ToListAsync(stoppingToken);

                    foreach (var reservation in reservations)
                    {
                        bool hasReminder = await db.NotificationCustomers
                        .AnyAsync(nc => nc.cus_id == reservation.cus_id
                                        && nc.notification.noti_type == "reservation_reminder"
                                        && nc.notification.noti_content.Contains($"[RID:{reservation.reser_id}]"),
                                stoppingToken);

                        if (!hasReminder)
                        {
                            var reminderNoti = new Notification
                            {
                                noti_title = "⏰ Sắp đến giờ đặt bàn",
                                noti_content = $"[RID:{reservation.reser_id}] Bạn có lịch hẹn tại {reservation.restaurant.res_name} lúc {reservation.reser_date:HH:mm 'ngày' dd/MM/yyyy}. Hãy chuẩn bị để đến đúng giờ nhé!",
                                noti_type = "reservation_reminder",
                                noti_date = DateTime.Now
                            };

                            db.Notifications.Add(reminderNoti);
                            await db.SaveChangesAsync(stoppingToken);

                            db.NotificationCustomers.Add(new NotificationCustomer
                            {
                                noti_id = reminderNoti.noti_id,
                                cus_id = reservation.cus_id.Value,
                                noti_customer_is_read = false,
                                noti_customer_send_date = DateTime.Now
                            });
                            await db.SaveChangesAsync(stoppingToken);

                            // Push realtime, dùng chính dữ liệu vừa lưu
                            await _hubContext.Clients.User(reservation.cus_id.ToString())
                            .SendAsync("ReceiveNotification", new
                            {
                                title = reminderNoti.noti_title,
                                content = reminderNoti.noti_content.Replace($"[RID:{reservation.reser_id}]", ""), // bỏ dấu RID
                                type = reminderNoti.noti_type,
                                date = reminderNoti.noti_date
                            });
                        }
                    }
                    _logger.LogInformation("Checked and sent notifications at {Time}", DateTime.Now);

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Changed to 10s
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationBackgroundService");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task CheckSpamCustomers(ApplicationDbContext db, CancellationToken stoppingToken)
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var startOfMonth = new DateTime(currentYear, currentMonth, 1);

                // Lấy customers có >= 3 đơn bị hủy trong tháng này
                var spamCustomers = await db.Orders
                    .Where(o => o.order_date >= startOfMonth && o.order_status == 4) // 4 = cancelled
                    .GroupBy(o => o.cus_id)
                    .Where(g => g.Count() >= 3)
                    .Select(g => new { CustomerId = g.Key, CancelledCount = g.Count() })
                    .ToListAsync(stoppingToken);

                foreach (var spamCustomer in spamCustomers)
                {
                    var customer = await db.Customers.FindAsync(spamCustomer.CustomerId);
                    if (customer == null || customer.cus_status == 1) continue; // Skip nếu đã là spam

                    // Check xem đã gửi thông báo spam trong tháng này chưa
                    bool alreadyNotified = await db.NotificationCustomers
                        .Include(nc => nc.notification)
                        .AnyAsync(nc => nc.cus_id == spamCustomer.CustomerId 
                                    && nc.notification.noti_type == "spam_warning"
                                    && nc.noti_customer_send_date >= startOfMonth, stoppingToken);

                    if (!alreadyNotified)
                    {
                        // *** SET STATUS = 1 ĐỂ ĐÁNH DẤU SPAM ***
                        customer.cus_status = 1;
                        db.Customers.Update(customer);
                        await db.SaveChangesAsync(stoppingToken);

                        // Tạo thông báo spam
                        var spamNotification = new Notification
                        {
                            noti_title = "Cảnh báo tài khoản",
                            noti_content = $"Tài khoản của bạn đã bị đánh dấu là spam do có {spamCustomer.CancelledCount} đơn hàng bị hủy trong tháng {currentMonth}/{currentYear}. Vui lòng cải thiện thói quen đặt hàng.",
                            noti_type = "spam_warning",
                            noti_date = DateTime.Now
                        };

                        db.Notifications.Add(spamNotification);
                        await db.SaveChangesAsync(stoppingToken);

                        // Gửi thông báo đến customer
                        var notiCustomer = new NotificationCustomer
                        {
                            noti_id = spamNotification.noti_id,
                            cus_id = spamCustomer.CustomerId,
                            noti_customer_is_read = false,
                            noti_customer_send_date = DateTime.Now
                        };

                        db.NotificationCustomers.Add(notiCustomer);
                        await db.SaveChangesAsync(stoppingToken);

                        // Gửi realtime notification
                        await _hubContext.Clients.User(spamCustomer.CustomerId.ToString())
                            .SendAsync("ReceiveNotification", new
                            {
                                title = spamNotification.noti_title,
                                content = spamNotification.noti_content,
                                type = spamNotification.noti_type,
                                date = DateTime.Now,
                                isSpamWarning = true,
                                customerStatus = 1 // Đánh dấu là spam
                            }, stoppingToken);

                        _logger.LogWarning($"Customer {spamCustomer.CustomerId} marked as SPAM (status=1): {spamCustomer.CancelledCount} cancelled orders this month");
                    }
                }

                if (spamCustomers.Any())
                {
                    _logger.LogInformation($"Processed spam detection for {spamCustomers.Count} customers");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking spam customers");
            }
        }

        private async Task ProcessWelcomeNotifications(ApplicationDbContext db, CancellationToken stoppingToken)
        {
            var welcomeNotification = await db.Notifications
                .Where(n => n.noti_type.ToLower() == "welcome")
                .OrderByDescending(n => n.noti_date)
                .FirstOrDefaultAsync(stoppingToken);

            if (welcomeNotification != null)
            {
                var newCustomers = await db.Customers
                    .Where(c => !db.NotificationCustomers
                        .Any(nc => nc.cus_id == c.cus_id && nc.notification.noti_type.ToLower() == "welcome"))
                    .ToListAsync(stoppingToken);

                foreach (var customer in newCustomers)
                {
                    db.NotificationCustomers.Add(new NotificationCustomer
                    {
                        noti_id = welcomeNotification.noti_id,
                        cus_id = customer.cus_id,
                        noti_customer_is_read = false,
                        noti_customer_send_date = DateTime.Now
                    });
                    await db.SaveChangesAsync(stoppingToken);

                    await _hubContext.Clients.User(customer.cus_id.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            title = welcomeNotification.noti_title,
                            content = welcomeNotification.noti_content,
                            type = welcomeNotification.noti_type,
                            date = DateTime.Now
                        }, stoppingToken);
                }
            }
        }

        private async Task ProcessScheduleNotifications(ApplicationDbContext db, CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            var scheduleNotifications = await db.Notifications
                .Where(n => n.noti_type.ToLower() == "schedule"
                            && n.noti_schedule != null
                            && n.noti_schedule <= now)
                .ToListAsync(stoppingToken);

            foreach (var scheduleNoti in scheduleNotifications)
            {
                var customers = await db.Customers
                    .Where(c => !db.NotificationCustomers
                        .Any(nc => nc.cus_id == c.cus_id && nc.noti_id == scheduleNoti.noti_id))
                    .ToListAsync(stoppingToken);

                foreach (var customer in customers)
                {
                    db.NotificationCustomers.Add(new NotificationCustomer
                    {
                        noti_id = scheduleNoti.noti_id,
                        cus_id = customer.cus_id,
                        noti_customer_is_read = false,
                        noti_customer_send_date = DateTime.Now
                    });
                    await db.SaveChangesAsync(stoppingToken);

                    await _hubContext.Clients.User(customer.cus_id.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            title = scheduleNoti.noti_title,
                            content = scheduleNoti.noti_content,
                            type = scheduleNoti.noti_type,
                            date = DateTime.Now
                        }, stoppingToken);
                }
            }
        }

        private async Task ProcessNormalNotifications(ApplicationDbContext db, CancellationToken stoppingToken)
        {
            var normalNotifications = await db.Notifications
                .Where(n => n.noti_type.ToLower() == "normal")
                .ToListAsync(stoppingToken);

            foreach (var normalNoti in normalNotifications)
            {
                var customers = await db.Customers
                    .Where(c => !db.NotificationCustomers
                        .Any(nc => nc.cus_id == c.cus_id && nc.noti_id == normalNoti.noti_id))
                    .ToListAsync(stoppingToken);

                foreach (var customer in customers)
                {
                    db.NotificationCustomers.Add(new NotificationCustomer
                    {
                        noti_id = normalNoti.noti_id,
                        cus_id = customer.cus_id,
                        noti_customer_is_read = false,
                        noti_customer_send_date = DateTime.Now
                    });
                    await db.SaveChangesAsync(stoppingToken);

                    await _hubContext.Clients.User(customer.cus_id.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            title = normalNoti.noti_title,
                            content = normalNoti.noti_content,
                            type = normalNoti.noti_type,
                            date = DateTime.Now
                        }, stoppingToken);
                }
            }
        }

        private async Task ProcessPaymentNotifications(ApplicationDbContext db, CancellationToken stoppingToken)
        {
            var paymentNotification = await db.Notifications
                .Where(n => n.noti_type.ToLower() == "payment")
                .OrderByDescending(n => n.noti_date)
                .FirstOrDefaultAsync(stoppingToken);

            if (paymentNotification != null)
            {
                var paidOrders = await db.Orders
                    .Where(o => o.order_status == 3) // 3 = đã thanh toán thành công
                    .ToListAsync(stoppingToken);

                foreach (var order in paidOrders)
                {
                    bool hasPaymentNoti = await db.NotificationCustomers
                        .AnyAsync(nc => nc.cus_id == order.cus_id 
                                        && nc.noti_id == paymentNotification.noti_id
                                        && nc.order_id == order.order_id, stoppingToken);

                    if (!hasPaymentNoti)
                    {
                        db.NotificationCustomers.Add(new NotificationCustomer
                        {
                            noti_id = paymentNotification.noti_id,
                            cus_id = order.cus_id,
                            order_id = order.order_id,
                            noti_customer_is_read = false,
                            noti_customer_send_date = DateTime.Now
                        });
                        await db.SaveChangesAsync(stoppingToken);

                        await _hubContext.Clients.User(order.cus_id.ToString())
                            .SendAsync("ReceiveNotification", new
                            {
                                title = paymentNotification.noti_title,
                                content = paymentNotification.noti_content,
                                type = paymentNotification.noti_type,
                                date = DateTime.Now
                            }, stoppingToken);
                    }
                }
            }
        }
    }
}