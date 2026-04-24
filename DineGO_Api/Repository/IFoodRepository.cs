using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Core.Models.Client.Custom;
namespace DineGO_Api.Repository
{
    public interface IFoodRepository
    {
        List<Food> GetFoods();
        Food FindFoodById(int id);
        List<Food> GetFoodsByMenuId(int menuId);
        void SaveFood(Food food);
        void UpdateFood(Food food);
        void DeleteFood(int id);

        List<RestaurantWithFoodsViewModel> GetFoodsGroupedByRestaurant(int? cusId);

        List<RestaurantWithFoodsViewModel> SearchFoods(string? keyword, string? restaurantName, decimal? minPrice, decimal? maxPrice, string? userAddress, double? userLat, double? userLng);
        List<RestaurantMarker> GetRestaurantMarkers();


    }
}