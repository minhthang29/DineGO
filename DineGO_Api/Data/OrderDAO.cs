using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using DineGO_Api.Services;

namespace DineGO_Api.Data
{
    public class OrderDAO
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public OrderDAO(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Get all Orders
        public List<Order> GetOrders()
        {
            try
            {
                return _context.Orders
                    .Include(o => o.customer)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Orders: {e.Message}");
            }
        }

        // Get Order by ID
        public Order FindOrderById(int id)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.restaurant)
                    .Include(o => o.customer)
                    .SingleOrDefault(o => o.order_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Order: {e.Message}");
            }
        }
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.restaurant)
                    .Where(o => o.cus_id == customerId)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Orders by customerId: {e.Message}");
            }
        }
        public void SaveOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                string content = "Đơn hàng của bạn đã được tạo. Chúng tôi sẽ xác nhận đơn hàng ngay.";
                string title = "Đơn hàng mới được tạo";
                _notificationService.NotifyCustomer(
                    order.cus_id,
                    title,
                    content,
                    "order"
                );
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Order: {e.Message}");
            }
        }

        public void UpdateOrder(Order order)
        {
            try
            {
                _context.Entry(order).State = EntityState.Modified;
                _context.SaveChanges();
                if (order.order_status == 3)
                {
                    var payment = new Payment
                    {
                        cart_id = null,
                        cus_id = order.cus_id,
                        reser_id = null,
                        pay_price = order.order_total,
                        pay_status = 1, // Đã thanh toán
                        pay_is_deleted = false,
                        pay_created_date = DateTime.Now,
                        pay_price_discount = order.order_price_discount
                    };
                    _context.Payments.Add(payment);
                    _context.SaveChanges();
                }
                string content = "";
                string title = "";

                if (order.order_status == 0)
                {
                    return; // Trạng thái mới tạo, không gửi thông báo
                }
                else if (order.order_status == 1)
                {
                    title = "Đơn hàng đã được xác nhận";
                    content = "Đơn hàng của bạn đã được xác nhận! Chúng tôi sẽ bắt đầu chuẩn bị món ngay.";
                }
                else if (order.order_status == 2)
                {
                    title = "Đơn hàng đang được chuẩn bị";
                    content = "Đơn hàng của bạn đã được chuẩn bị. Sẽ sớm được giao đến bạn!";
                }
                else if (order.order_status == 3)
                {
                    title = "Đơn hàng đã được hoàn thành";
                    content = "Đơn hàng của bạn đã được hoàn thành. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!";
                }
                else if (order.order_status == 4)
                {
                    title = "Đơn hàng đã bị hủy";
                    content = "Đơn hàng của bạn đã bị hủy. Nếu có thắc mắc, vui lòng liên hệ bộ phận hỗ trợ.";
                }
                _notificationService.NotifyCustomer(
                    order.cus_id,
                    title,
                    content,
                    "order"
                );
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating Order: {e.Message}");
            }
        }

        // Delete Order by ID
        public void DeleteOrder(int id)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.orderDetails)
                    .SingleOrDefault(x => x.order_id == id);
                if (order != null)
                {
                    _context.OrderDetails.RemoveRange(order.orderDetails);
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Order: {e.Message}");
            }
        }
        public List<Order> GetOrdersByRestaurantId(int resId)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.customer)
                    .Include(o => o.restaurant)
                    .Where(o => o.res_id == resId)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Orders by restaurantId: {e.Message}");
            }
        }
        /// <summary>
        /// Tạo đơn hàng với thông tin chi tiết và cố định
        /// </summary>
        public Order AddOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                
                // Gửi thông báo
                string content = "Đơn hàng của bạn đã được tạo. Chúng tôi sẽ xác nhận đơn hàng ngay.";
                string title = "Đơn hàng mới được tạo";
                _notificationService.NotifyCustomer(
                    order.cus_id,
                    title,
                    content,
                    "order"
                );
                
                return order;
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding Order: {e.Message}");
            }
        }

        /// <summary>
        /// Lấy đơn hàng theo ID (alias cho FindOrderById)
        /// </summary>
        public Order GetOrderById(int orderId)
        {
            return FindOrderById(orderId);
        }
    }
}