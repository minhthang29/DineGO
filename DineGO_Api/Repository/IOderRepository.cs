using System;
using System.Collections.Generic;
using Core.Models;
using Core.Models.Client.Custom;
using Core.Models.Client;

namespace DineGO_Api.Repository
{
    public interface IOrderRepository
    {
        List<Order> GetOrders();
        Order GetOrderById(int id);
        // int CreateOrderWithDetails(Order order, List<OrderDetail> details);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
        public bool CreateOrUpdateOrderFromCart(List<int> cartFoodIds, string voucherCode);
        List<Order> GetOrdersByCustomerId(int customerId);
        CustomViewOrderDetails GetOrderDetailsByOrderId(int orderId);
        List<Order> GetOrdersByRestaurantId(int resId);
        Order CreateOrderWithDetails(CreateOrderWithDetailsRequest orderData);
    }
}
