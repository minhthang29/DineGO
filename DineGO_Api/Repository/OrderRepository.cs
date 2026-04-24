using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using DineGO_Api.Data;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Core.Models.Client.Custom;
using Core.Models.Client;

namespace DineGO_Api.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _orderDao;
        private readonly OrderDetailDAO _orderDetailDao;
        private readonly CartFoodDAO _cartFoodDao;

        public OrderRepository(OrderDAO orderDao, OrderDetailDAO orderDetailDao, CartFoodDAO cartFoodDao)
        {
            _orderDao = orderDao;
            _orderDetailDao = orderDetailDao;
            _cartFoodDao = cartFoodDao;
        }

        public List<Order> GetOrders()
        {
            return _orderDao.GetOrders();
        }

        public Order GetOrderById(int id)
        {
            return _orderDao.FindOrderById(id);
        }
        public CustomViewOrderDetails GetOrderDetailsByOrderId(int orderId)
        {
            return _orderDetailDao.GetOrderDetailsByOrderId(orderId);
        }

        // public int CreateOrderWithDetails(Order order, List<OrderDetail> details)
        // {
        //     _orderDao.SaveOrder(order);

        //     foreach (var detail in details)
        //     {
        //         detail.order_id = order.order_id;
        //     }

        //     _orderDetailDao.SaveOrderDetails(details);
        //     return order.order_id;
        // }

        public void UpdateOrder(Order order)
        {
            _orderDao.UpdateOrder(order);
        }

        public void DeleteOrder(int id)
        {
            _orderDao.DeleteOrder(id);
        }
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            return _orderDao.GetOrdersByCustomerId(customerId);
        }
        public bool CreateOrUpdateOrderFromCart(List<int> cartFoodIds, string voucherCode)
        {
            var cartFoods = _cartFoodDao.GetCartFoods()
                .Where(cf => cartFoodIds.Contains(cf.cart_food_id)
                             && cf.cart != null && cf.food != null
                             && cf.cart.cus_id != null && cf.cart.restaurant != null)
                .ToList();

            if (!cartFoods.Any()) return false;

            // Gom nhóm theo (cus_id, res_id)
            var grouped = cartFoods
                .GroupBy(cf => (cf.cart!.cus_id!.Value, cf.cart.restaurant!.res_id));

            foreach (var group in grouped)
            {
                var (cusId, resId) = group.Key;

                // Tìm hoặc tạo Order status = 0
                var order = _orderDao.GetOrders()
                    .FirstOrDefault(o => o.cus_id == cusId && o.res_id == resId && o.order_status == 0);

                if (order == null)
                {
                    order = new Order
                    {
                        cus_id = cusId,
                        res_id = resId,
                        order_date = DateTime.Now,
                        order_status = 0,
                        order_total = 0,
                        voucher_code_applied = voucherCode
                    };
                    _orderDao.SaveOrder(order);
                }

                // Lấy các OrderDetail hiện tại
                var existingDetails = _orderDetailDao.GetOrderDetails()
                    .Where(d => d.order_id == order.order_id && d.cart_id != null)
                    .ToList();

                // gom theo food_id, bỏ qua cart_id
                var foodMap = new Dictionary<int, OrderDetail>();
                foreach (var od in existingDetails)
                {
                    var cf = _cartFoodDao.FindCartFoodById(od.cart_id.Value);
                    if (cf?.food_id != null)
                    {
                        int foodId = cf.food_id.Value;
                        if (!foodMap.ContainsKey(foodId))
                        {
                            foodMap[foodId] = od;
                        }
                    }
                }

                var newDetails = new List<OrderDetail>();

                foreach (var cf in group)
                {
                    int foodId = cf.food_id ?? 0;
                    if (foodId == 0 || cf.cart_id == null) continue;

                    int quantity = cf.food_quantity ?? 1;
                    decimal price = cf.food.food_price;

                    if (foodMap.TryGetValue(foodId, out var exist))
                    {
                        exist.order_quantity += quantity;
                        _orderDetailDao.UpdateOrderDetail(exist);
                    }
                    else
                    {
                        var detail = new OrderDetail
                        {
                            cart_id = cf.cart_id,
                            order_id = order.order_id,
                            order_quantity = quantity,
                            order_price = price
                        };
                        newDetails.Add(detail);
                        foodMap[foodId] = detail;
                    }
                }

                if (newDetails.Any())
                {
                    _orderDetailDao.SaveOrderDetails(newDetails);
                }

                // Cập nhật tổng tiền
                var updatedDetails = _orderDetailDao.GetOrderDetails()
                    .Where(d => d.order_id == order.order_id)
                    .ToList();

                order.order_total = updatedDetails.Sum(d => d.order_price * d.order_quantity);
                _orderDao.UpdateOrder(order);
            }

            return true;
        }

        public List<Order> GetOrdersByRestaurantId(int resId)
        {
            return _orderDao.GetOrdersByRestaurantId(resId);
        }
        /// <summary>
        /// Tạo đơn hàng với thông tin chi tiết và cố định
        /// </summary>
        public Order CreateOrderWithDetails(CreateOrderWithDetailsRequest orderData)
        {
            try
            {
                // Tạo đơn hàng
                var order = new Order
                {
                    cus_id = orderData.cus_id,
                    res_id = orderData.res_id,
                    order_date = orderData.order_date,
                    order_status = orderData.order_status,
                    order_subtotal = orderData.order_subtotal,
                    delivery_fee = orderData.delivery_fee,
                    order_price_discount = orderData.order_price_discount,
                    voucher_code_applied = orderData.voucher_code_applied,
                    voucher_type_applied = orderData.voucher_type_applied,
                    voucher_original_value = orderData.voucher_original_value,
                    order_total = orderData.order_total,
                    estimated_delivery_time = orderData.estimated_delivery_time,
                    customer_name_snapshot = orderData.customer_name_snapshot,
                    customer_phone_snapshot = orderData.customer_phone_snapshot,
                    delivery_address_snapshot = orderData.delivery_address_snapshot,
                    order_is_deleted = false
                };

                // Thêm order qua DAO
                var createdOrder = _orderDao.AddOrder(order);
                if (createdOrder == null) return null;

                // Tạo chi tiết đơn hàng
                foreach (var detail in orderData.OrderDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        order_id = createdOrder.order_id,
                        cart_id = detail.cart_id,
                        food_id = detail.food_id,
                        order_quantity = detail.order_quantity,
                        order_price = detail.order_price,
                        food_name_snapshot = detail.food_name_snapshot,
                        food_price_snapshot = detail.food_price_snapshot,
                        food_image_snapshot = detail.food_image_snapshot,
                        prep_time_snapshot = detail.prep_time_snapshot
                    };

                    _orderDetailDao.AddOrderDetail(orderDetail);
                }

                return createdOrder;
            }
            catch
            {
                return null;
            }
        }
    }
}
