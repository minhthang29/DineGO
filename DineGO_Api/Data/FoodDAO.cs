using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Client.Custom;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using Core.Helper;
namespace DineGO_Api.Data
{
    public class FoodDAO
    {
        private readonly ApplicationDbContext _context;
        private readonly GeoHelper _geoHelper = new GeoHelper();

        public FoodDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Food> GetFoods()
        {
            try
            {
                return _context.Foods
                    .Where(f => f.food_is_deleted != true)// chỉ lấy món chưa xoá
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching foods: {e.Message}");
            }
        }


        public Food FindFoodById(int id)
        {
            try
            {
                return _context.Foods
     .SingleOrDefault(f => f.food_id == id && f.food_is_deleted != true);

            }
            catch (Exception e)
            {
                throw new Exception($"Error finding food: {e.Message}");
            }
        }


        public List<Food> GetFoodsByMenuId(int menuId)
        {
            try
            {
                return _context.Foods
                    .Where(f => f.menu_id == menuId && f.food_is_deleted != true)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching foods by menu: {e.Message}");
            }
        }


        public void SaveFood(Food food)
        {
            try
            {
                _context.Foods.Add(food);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving food: {e.Message}");
            }
        }

        public void UpdateFood(Food food)
        {
            try
            {
                _context.Entry(food).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating food: {e.Message}");
            }
        }

        public void DeleteFood(int id)
        {
            try
            {
                var food = _context.Foods.SingleOrDefault(f => f.food_id == id);
                if (food != null)
                {
                    food.food_is_deleted = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting food: {e.Message}");
            }
        }


        public List<RestaurantWithFoodsViewModel> SearchFoods(string? keyword, string? restaurantName, decimal? minPrice, decimal? maxPrice, string? userAddress, double? userLat = null, double? userLng = null)
        {
            string keywordNoSign = RemoveDiacritics(keyword?.ToLower() ?? "");
            string resNameNoSign = RemoveDiacritics(restaurantName?.ToLower() ?? "");
            string userAddrNoSign = RemoveDiacritics(userAddress?.ToLower() ?? "");

            var foods = _context.Foods
            .Include(f => f.menu)
            .ThenInclude(m => m.restaurant)
            .Where(f => f.menu != null
              && f.menu.restaurant != null
              && f.menu.restaurant.res_is_use
              && f.menu.restaurant.res_is_authorized
              && f.menu.restaurant.res_is_deleted != true
              && f.food_is_deleted != true)
            .ToList();


            var filteredFoods = foods
                .Where(f =>
                    (string.IsNullOrEmpty(keyword) || RemoveDiacritics(f.food_name.ToLower()).Contains(keywordNoSign)) &&
                    (string.IsNullOrEmpty(restaurantName) || RemoveDiacritics(f.menu.restaurant.res_name.ToLower()).Contains(resNameNoSign)) &&
                    (!minPrice.HasValue || f.food_price >= minPrice.Value) &&
                    (!maxPrice.HasValue || f.food_price <= maxPrice.Value)
                )
                .ToList();

            var sortedFoods = filteredFoods
                .OrderByDescending(f => CalculateAddressMatchScore(f.menu.restaurant.res_address, userAddress))
                .ToList();

            return sortedFoods
                .GroupBy(f => f.menu.restaurant)
                .Select(g =>
                {
                    var res = g.Key;
                    double distance = 0;

                    if (userLat.HasValue && userLng.HasValue && res.res_latitude.HasValue && res.res_longitude.HasValue)
                    {
                        distance = _geoHelper.CalculateDistanceKm(userLat.Value, userLng.Value, res.res_latitude.Value, res.res_longitude.Value);
                    }

                    return new RestaurantWithFoodsViewModel
                    {
                        ResId = res.res_id,
                        ResName = res.res_name,
                        ResAddress = res.res_address,
                        ResImage = res.res_images,
                        ResLatitude = res.res_latitude,
                        ResLongitude = res.res_longitude,
                        DistanceKm = Math.Round(distance, 2),
                        Foods = g.Select(f => new FoodBasicViewModel
                        {
                            FoodId = f.food_id,
                            FoodName = f.food_name,
                            FoodPrice = f.food_price,
                            FoodImage = f.food_image,
                            FoodStatus = f.food_status
                        }).ToList()
                    };
                })
                .OrderBy(x => x.DistanceKm) 
                .ToList();
        }

        public List<RestaurantWithFoodsViewModel> GetFoodsGroupedByRestaurant(int? cusId)
        {
            try
            {
                var foods = _context.Foods
             .Include(f => f.menu)
             .ThenInclude(m => m.restaurant)
             .Where(f => f.food_is_deleted != true && f.menu != null && f.menu.restaurant != null && f.menu.restaurant.res_is_use &&  f.menu.restaurant.res_is_deleted != true && f.menu.restaurant.res_is_authorized)
            .ToList();


                // ✅ Lấy danh sách tag ưu tiên dưới dạng Dictionary<tag, score>
                Dictionary<string, double> priorityMap = new();
                if (cusId.HasValue)
                {
                    priorityMap = _context.Priorities
                        .Where(p => p.cus_id == cusId.Value)
                        .ToDictionary(p => p.tag.ToLower(), p => p.score);
                }

                // ✅ Nhóm theo nhà hàng và tính điểm ưu tiên
                var grouped = foods
                    .GroupBy(f => f.menu.restaurant)
                    .Select(g => new
                    {
                        Restaurant = g.Key,
                        Foods = g
                            .OrderByDescending(f => CalculatePriorityScore(f.food_tag, priorityMap))
                            .ToList(),
                        TotalScore = g
                            .Sum(f => CalculatePriorityScore(f.food_tag, priorityMap))
                    })
                    .OrderByDescending(g => g.TotalScore) // 🥇 Nhà hàng có tổng score cao sẽ được đưa lên trước
                    .Select(g => new RestaurantWithFoodsViewModel
                    {
                        ResId = g.Restaurant.res_id,
                        ResName = g.Restaurant.res_name,
                        ResAddress = g.Restaurant.res_address,
                        ResImage = g.Restaurant.res_images,
                        Foods = g.Foods.Select(f => new FoodBasicViewModel
                        {
                            FoodId = f.food_id,
                            FoodName = f.food_name,
                            FoodPrice = f.food_price,
                            FoodImage = f.food_image,
                            FoodStatus = f.food_status
                        }).ToList()
                    })
                    .ToList();

                return grouped;
            }
            catch (Exception e)
            {
                throw new Exception($"Error grouping foods: {e.Message}");
            }
        }
        /// <summary>
        /// Retrieves a list of restaurant markers that contain essential location information 
        /// for rendering on the map, including name, address, coordinates, and an optional image.
        /// </summary>
        /// <returns>
        /// A list of <see cref="RestaurantMarker"/> containing restaurants with valid latitude and longitude.
        /// </returns>
        /// <author>KhoiNV</author>
        public List<RestaurantMarker> GetRestaurantMarkers()
        {
            return _context.Restaurants
                .Where(r => r.res_latitude != null && r.res_longitude != null && r.res_is_deleted != true && r.res_is_authorized == true) 
                .Select(r => new RestaurantMarker
                {
                    name = r.res_name,
                    address = r.res_address,
                    latitude = r.res_latitude.Value,
                    longitude = r.res_longitude.Value,
                    image = r.res_images_json.FirstOrDefault()
                }).ToList();
        }


        /// <summary>
        /// Removes all diacritical marks (accents) from a Unicode string.
        /// This is useful for normalizing strings to a base form without accents, enabling accent-insensitive searches or comparisons.
        /// </summary>
        /// <param name="text">The input string to normalize.</param>
        /// <returns>A new string with all diacritics removed, preserving only the base characters.</returns>
        public string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        /// <summary>
        /// Calculates the address matching score between the user's address and the restaurant's address.
        /// The function splits the userAddress into individual parts (e.g., ward, district, province)
        /// and counts how many of those parts are present in the restaurant's address.
        /// Each match increases the score by 1.
        /// </summary>
        /// <param name="restaurantAddress">The full address of the restaurant (usually includes ward, district, province, etc.)</param>
        /// <param name="userAddress">The full address selected by the user, e.g., "Ward An Binh, District Ninh Kieu, Can Tho City"</param>
        /// <returns>
        /// An integer score (0–3) representing how many address parts matched:
        /// - 0: no match
        /// - 1: one part matched (e.g., ward only)
        /// - 2: two parts matched (e.g., ward and district)
        /// - 3: full match (ward, district, province)
        /// </returns>
        private int CalculateAddressMatchScore(string? restaurantAddress, string? userAddress)
        {
            if (string.IsNullOrWhiteSpace(restaurantAddress) || string.IsNullOrWhiteSpace(userAddress))
                return 0;

            var restAddr = RemoveDiacritics(restaurantAddress.ToLower());
            var parts = userAddress.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            int score = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                var cleanPart = RemoveDiacritics(parts[i].ToLower());

                if (restAddr.Contains(cleanPart))
                {
                    //priority district
                    if (i == 0) score += 3;
                    else if (i == 1) score += 2;
                    else score += 1;
                }
            }

            return score;
        }

        // ✅ Tính tổng điểm ưu tiên của một món ăn theo các tag
        private double CalculatePriorityScore(string? foodTags, Dictionary<string, double> priorityMap)
        {
            if (string.IsNullOrWhiteSpace(foodTags) || priorityMap == null || priorityMap.Count == 0)
                return 0;

            var tags = foodTags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.Trim().ToLowerInvariant());

            double score = 0;
            foreach (var tag in tags)
            {
                if (priorityMap.TryGetValue(tag, out double weight))
                    score += weight;
            }

            return score;
        }

        public List<Food> GetFoodsByTags(List<string> tags)
        {
            if (tags == null || tags.Count == 0) return new();

            return _context.Foods
                .Where(f => !string.IsNullOrEmpty(f.food_tag))
                .AsEnumerable() // chuyển sang LINQ in-memory để xử lý split tag
                .Where(f => f.food_tag.Split(',').Any(tag => tags.Contains(tag.Trim())))
                .ToList();
        }

    }
}