using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Client.Custom;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class OrderDetailDAO
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderDAO _orderDao; // Thêm dòng này

        public OrderDetailDAO(ApplicationDbContext context, OrderDAO orderDao)
        {
            _context = context;
            _orderDao = orderDao; // Thêm dòng này
        }

        public List<OrderDetail> GetOrderDetails()
        {
            try
            {
                return _context.OrderDetails
                    .Include(d => d.order)
                    .Include(d => d.cart)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching OrderDetails: {e.Message}");
            }
        }
        public List<OrderDetail> GetDetailsByOrderId(int orderId)
        {
            try
            {
                return _context.OrderDetails
                    .Include(d => d.order)
                    .Include(d => d.cart)
                    .Where(d => d.order_id == orderId)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching OrderDetails by orderId: {e.Message}");
            }
        }
        public CustomViewOrderDetails GetOrderDetailsByOrderId(int orderId)
        {
            var orderDetails = GetDetailsByOrderId(orderId);

            // Lấy tất cả cart_id từ các OrderDetail
            var cartIds = orderDetails
                .Where(od => od.cart_id.HasValue)
                .Select(od => od.cart_id.Value)
                .Distinct()
                .ToList();

            // Lấy tất cả CartFood liên quan đến các cart_id
            var listCartFood = _context.CartFoods
                .Include(cf => cf.food)
                .Where(cf => cf.cart_id.HasValue && cartIds.Contains(cf.cart_id.Value))
                .ToList();

            // Lấy tất cả Food liên quan đến các CartFood
            var listFood = listCartFood
                .Select(cf => cf.food)
                .Where(f => f != null)
                .Distinct()
                .ToList();
            var restaurant = _orderDao.FindOrderById(orderId)?.restaurant;
            return new CustomViewOrderDetails
            {
                Order = _orderDao.FindOrderById(orderId),
                OrderDetails = orderDetails,
                listCartFood = listCartFood,
                listFood = listFood,
                Restaurant = restaurant
            };
        }

        public OrderDetail FindOrderDetailById(int id)
        {
            try
            {
                return _context.OrderDetails
                    .Include(d => d.order)
                    .SingleOrDefault(x => x.order_detail_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding OrderDetail: {e.Message}");
            }
        }

        public void SaveOrderDetail(OrderDetail detail)
        {
            try
            {
                _context.OrderDetails.Add(detail);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving OrderDetail: {e.Message}");
            }
        }

        public void SaveOrderDetails(List<OrderDetail> details)
        {
            try
            {
                _context.OrderDetails.AddRange(details);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving OrderDetails: {e.Message}");
            }
        }

        public void UpdateOrderDetail(OrderDetail detail)
        {
            try
            {
                _context.Entry(detail).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating OrderDetail: {e.Message}");
            }
        }

        public void DeleteOrderDetail(int id)
        {
            try
            {
                var detail = _context.OrderDetails.SingleOrDefault(x => x.order_detail_id == id);
                if (detail != null)
                {
                    _context.OrderDetails.Remove(detail);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting OrderDetail: {e.Message}");
            }
        }
        /// <summary>
        /// Add new OrderDetail
        /// </summary>
        public OrderDetail AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
                return orderDetail;
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding OrderDetail: {e.Message}");
            }
        }
    }
}