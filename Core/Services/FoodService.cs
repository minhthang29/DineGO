using Core.Constant;
using Core.Services;
using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models.Client.Custom;
using Core.Helper;
using System.Linq;
namespace Core.Services
{
    /// <summary>
    /// Handles food-related operations by communicating with the API.
    /// </summary>
    /// <author>KhoiNV</author>
    public class FoodService
    {
        private readonly ApiService _apiService;
        private readonly GeoHelper _geoHelper;

        public FoodService(ApiService apiService, GeoHelper geoHelper)
        {
            _apiService = apiService;
            _geoHelper = geoHelper;
        }

        /// <summary>
        /// Retrieves all food items.
        /// </summary>
        /// <returns>A list of all foods.</returns>
        /// <author>KhoiNV</author>
        public async Task<List<Food>> GetAllFoodsAsync()
        {
            var foods = await _apiService.GetAsync<List<Food>>(ApiEndpoints.FOOD);
            return foods;
        }
        /// <summary>
        /// Retrieves a specific food item by its ID.
        /// </summary>
        /// <param name="id">The ID of the food item.</param>
        /// <returns>The food item matching the specified ID.</returns>
        /// <author>KhoiNV</author>
        public async Task<Food> GetFoodByIdAsync(int id)
        {
            return await _apiService.GetAsync<Food>($"{ApiEndpoints.FOOD}/{id}");
        }
        /// <summary>
        /// Retrieves all food items associated with a specific menu ID.
        /// </summary>
        /// <param name="menuId">The ID of the menu.</param>
        /// <returns>A list of foods under the specified menu.</returns>
        /// <author>KhoiNV</author>
        public async Task<List<Food>> GetFoodsByMenuIdAsync(int menuId)
        {
            return await _apiService.GetAsync<List<Food>>($"{ApiEndpoints.FOOD}/menu/{menuId}");
        }

        /// <summary>
        /// Searches food items by keyword, restaurant name, and price range, then groups the result by restaurant.
        /// </summary>
        /// <param name="keyword">Search keyword for food name.</param>
        /// <param name="restaurantName">Filter by restaurant name.</param>
        /// <param name="minPrice">Minimum price filter.</param>
        /// <param name="maxPrice">Maximum price filter.</param>
        /// <author>KhoiNV</author>
        public async Task<List<RestaurantWithFoodsViewModel>> SearchFoodsAsync(
        string? keyword,
        string? restaurantName,
        decimal? minPrice,
        decimal? maxPrice,
        string? userAddress,
        double? userLat = null,
        double? userLng = null)
        {
            var query = $"search?";
            if (!string.IsNullOrEmpty(keyword)) query += $"keyword={keyword}&";
            if (!string.IsNullOrEmpty(restaurantName)) query += $"restaurantName={restaurantName}&";
            if (!string.IsNullOrEmpty(userAddress)) query += $"userAddress={userAddress}&";
            if (minPrice.HasValue) query += $"minPrice={minPrice}&";
            if (maxPrice.HasValue) query += $"maxPrice={maxPrice}&";
            query = query.TrimEnd('&');

            var result = await _apiService.GetAsync<List<RestaurantWithFoodsViewModel>>($"{ApiEndpoints.FOOD}/{query}");

            if (userLat != null && userLng != null)
            {
                foreach (var r in result)
                {
                    if (r.ResLatitude != null && r.ResLongitude != null)
                    {
                        r.DistanceKm = _geoHelper.CalculateDistanceKm(
                            userLat.Value, userLng.Value,
                            r.ResLatitude.Value, r.ResLongitude.Value
                        );
                    }
                }

                result = result
                    .OrderBy(r => !(r.ResLatitude.HasValue && r.ResLongitude.HasValue)) 
                    .ThenBy(r => r.DistanceKm == 0 ? double.MaxValue : r.DistanceKm)   
                    .ToList();

            }


            return result;
        }


        /// <summary>
        /// Retrieves all food items grouped by their associated restaurant.
        /// </summary>
        /// <returns>A list of restaurants each containing their food items.</returns>
        /// <author>KhoiNV</author>
        public async Task<List<RestaurantWithFoodsViewModel>> GetFoodsGroupedByRestaurantAsync(int? cusId = null)
        {
            var url = $"{ApiEndpoints.FOOD}/group-by-restaurant";

            if (cusId.HasValue)
                url += $"?cusId={cusId.Value}";

            var result = await _apiService.GetAsync<List<RestaurantWithFoodsViewModel>>(url);
            return result ?? new();
        }

        public async Task<object> CreateFoodAsync(Food food)
        {
            return await _apiService.PostAsync<object, Food>(ApiEndpoints.FOOD, food);
        }

        public async Task<object> UpdateFoodAsync(Food food)
        {
            return await _apiService.PutAsync<object, Food>($"{ApiEndpoints.FOOD}", food);
        }

        public async Task<object> DeleteFoodAsync(Food food)
        {
            food.food_is_deleted = true;
            return await _apiService.PutAsync<object, Food>($"{ApiEndpoints.FOOD}/{food.food_id}", food);
        }
        /// <summary>
        /// Get a list of restaurants with elevations to display markers on the map
        /// </summary>
        /// <author>KhoiNV</author>
        public async Task<List<RestaurantMarker>> GetRestaurantMarkersAsync()
        {
            return await _apiService.GetAsync<List<RestaurantMarker>>("food/markers");
        }



    }
}
