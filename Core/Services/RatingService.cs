using Core.Constant;
using Core.Models;
using Core.Models.Client.Custom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    /// <summary>
    /// Handles restaurant rating operations by communicating with the API.
    /// </summary>
    /// <author>KhoiNV</author>
    public class RatingService
    {
        private readonly ApiService _apiService;

        public RatingService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Retrieves all ratings for a specific restaurant.
        /// </summary>
        /// <param name="resId">The ID of the restaurant.</param>
        /// <returns>List of ratings for the restaurant.</returns>
        /// <author>KhoiNV</author>
        public async Task<List<RestaurantRating>> GetRatingsByRestaurantIdAsync(int resId)
        {
            return await _apiService.GetAsync<List<RestaurantRating>>($"{ApiEndpoints.RATING}/restaurant/{resId}");
        }

        /// <summary>
        /// Creates or updates a rating for a restaurant by a customer.
        /// </summary>
        /// <param name="model">The rating view model to submit.</param>
        /// <returns>Returns status and rating ID from API.</returns>
        /// <author>KhoiNV</author>
        public async Task<object> AddOrUpdateRatingAsync(RestaurantRatingViewModel model)
        {
            return await _apiService.PostAsync<object, RestaurantRatingViewModel>($"{ApiEndpoints.RATING}", model);
        }

        public async Task<bool> HasCompletedOrderAsync(int cusId, int resId)
        {
           return await _apiService.GetAsync<bool>($"{ApiEndpoints.RATING_HAS_COMPLETED_ORDER}{cusId}/{resId}");
        }

    }
}
