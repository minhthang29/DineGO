using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Core.Models.Client.Custom;
namespace DineGO_Api.Repository
{
    public class FoodRepository : IFoodRepository
    {
        private readonly FoodDAO _foodDAO;

        public FoodRepository(FoodDAO foodDAO)
        {
            _foodDAO = foodDAO;
        }

        public List<Food> GetFoods() => _foodDAO.GetFoods();

        public Food FindFoodById(int id) => _foodDAO.FindFoodById(id);

        public List<Food> GetFoodsByMenuId(int menuId) => _foodDAO.GetFoodsByMenuId(menuId);

        public void SaveFood(Food food) => _foodDAO.SaveFood(food);

        public void UpdateFood(Food food) => _foodDAO.UpdateFood(food);

        public void DeleteFood(int id) => _foodDAO.DeleteFood(id);

        public List<RestaurantWithFoodsViewModel> GetFoodsGroupedByRestaurant(int? cusId)
            => _foodDAO.GetFoodsGroupedByRestaurant(cusId);
            
        public List<RestaurantWithFoodsViewModel> SearchFoods(string? keyword, string? restaurantName, decimal? minPrice, decimal? maxPrice, string? userAddress, double? userLat, double? userLng)
        => _foodDAO.SearchFoods(keyword, restaurantName, minPrice, maxPrice, userAddress, userLat, userLng);

        public List<RestaurantMarker> GetRestaurantMarkers()
        {
            return _foodDAO.GetRestaurantMarkers();
        }



    }
}