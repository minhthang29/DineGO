using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using Core.Models.Client.Custom;
using DineGO_Api.Repository;
namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for handling operations related to foods such as retrieval, creation, update, deletion, and menu filtering.
    /// </summary>
    /// <author>KhoiNV</author>
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;

        /// <summary>
        /// Constructor that injects the food repository for handling food data.
        /// </summary>
        public FoodController(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        /// <summary>
        /// Retrieves a list of all foods.
        /// </summary>
        /// <returns>List of foods.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_foodRepository.GetFoods());
        }

        /// <summary>
        /// Retrieves a food item by its ID.
        /// </summary>
        /// <param name="id">The ID of the food.</param>
        /// <returns>Food details or 404 if not found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var food = _foodRepository.FindFoodById(id);
            if (food == null)
            {
                return NotFound($"Food with ID {id} not found.");
            }
            return Ok(food);
        }

        /// <summary>
        /// Retrieves foods by their menu ID.
        /// </summary>
        /// <param name="menuId">The ID of the menu.</param>
        /// <returns>List of foods under that menu.</returns>
        [HttpGet("menu/{menuId}")]
        public IActionResult GetByMenu(int menuId)
        {
            return Ok(_foodRepository.GetFoodsByMenuId(menuId));
        }

        /// <summary>
        /// Adds a new food item.
        /// </summary>
        /// <param name="food">The food object to add.</param>
        /// <returns>Object containing the new food ID.</returns>
        [HttpPost]
        public IActionResult Create(Food food)
        {
            _foodRepository.SaveFood(food);
            return Ok(new { food_id = food.food_id });
        }

        /// <summary>
        /// Updates food information.
        /// </summary>
        /// <param name="food">The updated food object.</param>
        /// <returns>List of all foods after update.</returns>
        [HttpPut]
        public IActionResult Update(Food food)
        {
            _foodRepository.UpdateFood(food);
            return Ok(_foodRepository.GetFoods());
        }

        /// <summary>
        /// Deletes a food item by its ID.
        /// </summary>
        /// <param name="id">The ID of the food to delete.</param>
        /// <returns>List of all foods after deletion.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _foodRepository.DeleteFood(id);
            return Ok(_foodRepository.GetFoods());
        }
        /// <summary>
        /// Search a food item by its ID.
        /// </summary>
        /// <returns>List of all foods after searched.</returns>
        [HttpGet("search")]
        public IActionResult Search(
      string? keyword,
      string? restaurantName,
      decimal? minPrice,
      decimal? maxPrice,
      string? userAddress,
      double? userLat,
      double? userLng)
        {
            var result = _foodRepository.SearchFoods(keyword, restaurantName, minPrice, maxPrice, userAddress, userLat, userLng);
            return Ok(result);
        }
        /// <summary>
        /// Get food group by restaurant
        /// </summary>
        /// <returns>food group by restaurant.</returns>
        [HttpGet("group-by-restaurant")]
        public IActionResult GetFoodsGroupedByRestaurant([FromQuery] int? cusId)
        {
            var data = _foodRepository.GetFoodsGroupedByRestaurant(cusId);
            return Ok(data);
        }

        [HttpGet("markers")]
        public ActionResult<List<RestaurantMarker>> GetRestaurantMarkers()
        {
            var markers = _foodRepository.GetRestaurantMarkers();
            return Ok(markers);
        }

    }
}