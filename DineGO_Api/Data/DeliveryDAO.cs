using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class DeliveryDAO
    {
        private readonly ApplicationDbContext _context;

        public DeliveryDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Delivery> GetDeliveries()
        {
            return _context.Deliveries.Include(d => d.order).ToList();
        }

        public Delivery? GetDeliveryById(int id)
        {
            return _context.Deliveries.Include(d => d.order).SingleOrDefault(d => d.de_id == id);
        }

        public List<Delivery> GetDeliveriesByOrderId(int orderId)
        {
            return _context.Deliveries.Where(d => d.order_id == orderId).ToList();
        }

        public List<Delivery> GetDeliveriesByCustomerId(int customerId)
        {
            return _context.Deliveries
                .Include(d => d.order)
                    .ThenInclude(o => o.restaurant)
                .Include(d => d.order)
                    .ThenInclude(o => o.orderDetails)
                        .ThenInclude(od => od.cart)
                            .ThenInclude(c => c.cartFoods)
                                .ThenInclude(cf => cf.food)
                .Where(d => d.order.cus_id == customerId)
                .OrderByDescending(d => d.order.order_date)
                .ToList();
        }

        public void AddDelivery(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            _context.SaveChanges();
        }

        public void UpdateDelivery(Delivery delivery)
        {
            _context.Entry(delivery).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteDelivery(int id)
        {
            var delivery = _context.Deliveries.Find(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                _context.SaveChanges();
            }
        }
    }
}
