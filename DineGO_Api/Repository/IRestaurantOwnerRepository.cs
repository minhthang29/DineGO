using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IRestaurantOwnerRepository
    {
        List<RestaurantOwner> GetRestaurantOwners();

        RestaurantOwner FindRestaurantOwnerById(int Id);
        List<RestaurantOwner> FindRestaurantOwnersByCusId(int cusId);

        void SaveRestaurantOwner(RestaurantOwner restaurantOwner);

        void UpdateRestaurantOwner(RestaurantOwner restaurantOwner);

        void DeleteRestaurantOwner(int Id);
    }
}