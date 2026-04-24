using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace DineGO_Api.Repository
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly DeliveryDAO _dao;

        public DeliveryRepository(DeliveryDAO dao)
        {
            _dao = dao;
        }

        public List<Delivery> GetDeliveries() => _dao.GetDeliveries();

        public Delivery? GetDeliveryById(int id) => _dao.GetDeliveryById(id);

        public List<Delivery> GetDeliveriesByOrderId(int orderId) => _dao.GetDeliveriesByOrderId(orderId);

        public List<Delivery> GetDeliveriesByCustomerId(int customerId) => _dao.GetDeliveriesByCustomerId(customerId);

        public void AddDelivery(Delivery delivery) => _dao.AddDelivery(delivery);

        public void UpdateDelivery(Delivery delivery) => _dao.UpdateDelivery(delivery);

        public void DeleteDelivery(int id) => _dao.DeleteDelivery(id);
    }
}
