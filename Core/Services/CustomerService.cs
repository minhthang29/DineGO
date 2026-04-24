using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Client.Custom;
using Core.Constant;
using Core.Common;

namespace Core.Services
{
    /// <summary>
    /// Service for handling customer-related business logic.
    /// </summary>
    /// <author>phuonghh</author>
    public class CustomerService
    {
        private readonly ApiService _apiService;
        private readonly S3BucketAWS _s3Bucket;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="apiService">API service dependency.</param>
        /// <param name="s3Bucket">S3 bucket service dependency.</param>
        public CustomerService(ApiService apiService, S3BucketAWS s3Bucket)
        {
            _apiService = apiService;
            _s3Bucket = s3Bucket;
        }

        /// <summary>
        /// Gets the customer profile view model with related data.
        /// </summary>
        /// <param name="cus_id">Customer ID.</param>
        /// <returns>A <see cref="CustomProfileViewModel"/> containing customer, restaurant, and reservation information.</returns>
        public async Task<CustomProfileViewModel> GetProfileViewModel(int cus_id)
        {
            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cus_id}");
            var restaurantOwners = await _apiService.GetAsync<List<RestaurantOwner>>(string.Format(ApiEndpoints.RESTAURANT_OWNER_BY_CUS_ID, cus_id));
            var restaurant = await _apiService.GetAsync<List<Restaurant>>(ApiEndpoints.RESTAURANT);
            var reservation = await _apiService.GetAsync<List<Reservation>>(ApiEndpoints.RESERVATION);

            return new CustomProfileViewModel
            {
                Customer = customer,
                RestaurantOwners = restaurantOwners,
                Restaurant = restaurant,
                Reservation = reservation
            };
        }
    }
}