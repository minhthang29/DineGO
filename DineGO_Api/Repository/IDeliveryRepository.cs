using Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DineGO_Api.Repository
{
    public interface IDeliveryRepository
    {
        List<Delivery> GetDeliveries();
        Delivery? GetDeliveryById(int id);
        List<Delivery> GetDeliveriesByOrderId(int orderId);
        List<Delivery> GetDeliveriesByCustomerId(int customerId);
        void AddDelivery(Delivery delivery);
        void UpdateDelivery(Delivery delivery);
        void DeleteDelivery(int id);
    }
}
