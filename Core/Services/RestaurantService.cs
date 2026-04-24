using Core.Constant;
using Core.Services;
using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models.Client.Custom;

namespace Core.Services
{
    /// <summary>
    /// Handles restaurant-related operations by communicating with the API.
    /// </summary>
    /// <author>KhoiNV</author>
    public class RestaurantService
    {
        private readonly ApiService _apiService;

        public RestaurantService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<Restaurant> GetRestaurantByID(int res_id)
        {
            var restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{res_id}");
            return restaurant;
        }

        /// <summary>
        /// Retrieves the list of all restaurants from the API.
        /// </summary>
        /// <returns>List of restaurants.</returns>
        /// <author>KhoiNV</author>
        public async Task<List<Restaurant>> GetAllRestaurantsAsync()
        {
            var restaurants = await _apiService.GetAsync<List<Restaurant>>(ApiEndpoints.RESTAURANT);
            foreach (var r in restaurants)
            {
                var category = await _apiService.GetAsync<Category>($"{ApiEndpoints.CATEGORY_BY_ID}{r.cate_id}");
                r.category = category;
            }
            return restaurants;
        }

        public async Task<List<Restaurant>> GetAllRestaurantsForAdminAsync()
        {
            var restaurants = await _apiService.GetAsync<List<Restaurant>>(ApiEndpoints.RESTAURANT + "/admin");
            foreach (var r in restaurants)
            {
                var category = await _apiService.GetAsync<Category>($"{ApiEndpoints.CATEGORY_BY_ID}{r.cate_id}");
                r.category = category;
            }
            return restaurants;
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(int id)
        {
            var restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{id}");
            if (restaurant != null)
            {
                restaurant.category = await _apiService.GetAsync<Category>($"{ApiEndpoints.CATEGORY_BY_ID}{restaurant.cate_id}");
                restaurant.restaurantOwner = await _apiService.GetAsync<RestaurantOwner>($"{ApiEndpoints.RESTAURANT_OWNER_BY_ID}{restaurant.res_owner_id}");
            }
            return restaurant;
        }

        public async Task<List<Restaurant>> GetALLRestaurantByResOwnerAsync(int resOwner_id)
        {
            var restaurant = await _apiService.GetAsync<List<Restaurant>>($"{ApiEndpoints.RESTAURANT_BY_RESTAURANT_OWNER_ID}{resOwner_id}");
            return restaurant;
        }


        /// <summary>
        /// Adds a new restaurant via the API.
        /// </summary>
        /// <param name="restaurant">The restaurant object to add.</param>
        /// <returns>The newly added restaurant.</returns>
        public async Task<Restaurant> AddRestaurantAsync(Restaurant restaurant)
        {
            restaurant.res_is_use = true; // Default value for new restaurant
            return await _apiService.PostAsync<Restaurant, Restaurant>(ApiEndpoints.RESTAURANT, restaurant);
        }

        /// <summary>
        /// Updates an existing restaurant via the API.
        /// </summary>
        /// <param name="restaurant">The restaurant object with updated information.</param>
        /// <returns>The updated restaurant.</returns>
        public async Task<bool> UpdateRestaurantAsync(Restaurant restaurant)
        {
            await _apiService.PutAsync<object, object>($"{ApiEndpoints.RESTAURANT}", restaurant);
            return true;
        }

        /// <summary>
        /// Deletes a restaurant by its ID via the API.
        /// </summary>
        /// <param name="res_id">The ID of the restaurant to delete.</param>
        /// <returns>True if the deletion was successful.</returns>
        public async Task<bool> DeleteRestaurantAsync(int res_id)
        {
            await _apiService.DeleteAsync<object>($"{ApiEndpoints.RESTAURANT}?Id={res_id}");
            return true;
        }

        public async Task<bool> BlockRestaurantAsync(int res_id)
        {
            await _apiService.DeleteAsync<object>($"{ApiEndpoints.RESTAURANT}/block/{res_id}");
            return true;
        }
        public async Task<bool> ActiveRestaurantAsync(int res_id)
        {
            await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.RESTAURANT}/active/{res_id}", null);
            return true;
        }
        public async Task<bool> FollowRestaurantAsync(int cus_id, int res_id)
        {
            var url = $"{ApiEndpoints.FOLLOW_RESTAURANT}?cus_id={cus_id}&res_id={res_id}";
            return await _apiService.PostAsync<bool, object>(url, null);
        }

        public async Task<bool> UnfollowRestaurantAsync(int cus_id, int res_id)
        {
            var url = $"{ApiEndpoints.UNFOLLOW_RESTAURANT}?cus_id={cus_id}&res_id={res_id}";
            return await _apiService.DeleteAsync<bool>(url);
        }

        public async Task<bool> IsFollowingRestaurantAsync(int cus_id, int res_id)
        {
            var url = $"{ApiEndpoints.CHECK_FOLLOW_RESTAURANT}?cus_id={cus_id}&res_id={res_id}";
            var response = await _apiService.GetAsync<CheckFollowResponse>(url);
            return response?.isFollowing ?? false;
        }
    }
}
