using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class RestaurantOwnerRepository : IRestaurantOwnerRepository
    {
        private readonly RestaurantOwnerDAO _restaurantOwnerDAO;
        public RestaurantOwnerRepository(RestaurantOwnerDAO restaurantOwnerDAO)
        =>
            _restaurantOwnerDAO = restaurantOwnerDAO;

        public void DeleteRestaurantOwner(int Id)
        =>
            _restaurantOwnerDAO.DeleteRestaurantOwner(Id);

        public RestaurantOwner FindRestaurantOwnerById(int Id)
        =>
             _restaurantOwnerDAO.FindRestaurantOwnerById(Id);
        public List<RestaurantOwner> FindRestaurantOwnersByCusId(int cusId)
        =>
             _restaurantOwnerDAO.FindRestaurantOwnersByCusId(cusId);

        public List<RestaurantOwner> GetRestaurantOwners()

            => _restaurantOwnerDAO.GetRestaurantOwners();

        public void SaveRestaurantOwner(RestaurantOwner restaurantOwner)
        =>
            _restaurantOwnerDAO.SaveRestaurantOwner(restaurantOwner);

        public void UpdateRestaurantOwner(RestaurantOwner restaurantOwner)
        =>
            _restaurantOwnerDAO.UpdateRestaurantOwner(restaurantOwner);
    }
}
