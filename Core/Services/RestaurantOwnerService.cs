using Core.Common;
using Core.Constant;
using Core.Models;
using Core.Services;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace Core.Services
{
    public class RestaurantOwnerService
    {
        private readonly ApiService _apiService;
        private readonly ImageHelper _imageHelper;

        public RestaurantOwnerService(ApiService apiService, ImageHelper imageHelper)
        {
            _apiService = apiService;
            _imageHelper = imageHelper;
        }

        public async Task<List<RestaurantOwner>> GetAllAsync()
        {
            return await _apiService.GetAsync<List<RestaurantOwner>>(ApiEndpoints.RESTAURANT_OWNER);
        }

        public async Task<RestaurantOwner> GetByIdAsync(int id)
        {
            return await _apiService.GetAsync<RestaurantOwner>($"{ApiEndpoints.RESTAURANT_OWNER_BY_ID}{id}");
        }

        public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant, List<IFormFile> images)
        {
            var fileNames = new List<string>();

            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    var fileName = await _imageHelper.UploadImageWithThumbnailAsync(image, "restaurants", thumbWidth: 600);
                    fileNames.Add(fileName);
                }

                restaurant.res_images = JsonSerializer.Serialize(fileNames);
            }

            var result = await _apiService.PostAsync<Restaurant, Restaurant>($"{ApiEndpoints.RESTAURANT}", restaurant);
            return result;
        }

       public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant, List<IFormFile> images)
        {
            // 1️⃣ Lấy danh sách reservation của nhà hàng từ API
            var reservations = await _apiService.GetAsync<List<Reservation>>(
                $"{ApiEndpoints.RESERVATION_BY_RESID}{restaurant.res_id}"
            );

            // 2️⃣ Kiểm tra có reservation nào từ hôm nay trở đi không
            var hasFutureReservations = reservations.Any(r =>
                !r.reser_is_deleted &&
                r.reser_status != 2 &&
                r.reser_date >= DateTime.Now
            );

            if (hasFutureReservations)
            {
                return null; // Không cho phép cập nhật nếu có reservation trong tương lai
            }

            // 3️⃣ Xử lý upload ảnh như cũ
            if (images != null && images.Count > 0)
            {
                var fileNames = new List<string>();

                foreach (var image in images)
                {
                    var fileName = await _imageHelper.UploadImageWithThumbnailAsync(image, "restaurants", thumbWidth: 600);
                    fileNames.Add(fileName);
                }

                // Ghi đè khi có ảnh mới
                restaurant.res_images = System.Text.Json.JsonSerializer.Serialize(fileNames);
            }

            // 4️⃣ Gọi API để update nhà hàng
            var result = await _apiService.PutAsync<Restaurant, Restaurant>(
                $"{ApiEndpoints.RESTAURANT}", restaurant
            );

            return result;
        }

        public async Task<List<Verification>> GetVerificationsByRestaurantIdAsync(int res_id)
        {
            return await _apiService.GetAsync<List<Verification>>($"{ApiEndpoints.VERIFICATION_BY_RESID}{res_id}");
        }

        public async Task<bool> RegisterVerificationAsync(Verification model, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Upload file lên S3
                var fileUrl = await _imageHelper.UploadPdfAsync(file, "verifications");
                model.ver_file_attachment = fileUrl;
            }
            var result = await _apiService.PostAsync<Verification, Verification>($"{ApiEndpoints.VERIFICATION}", model);
            return result != null;
        }

    }
}