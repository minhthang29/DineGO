using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for handling operations related to restaurants such as retrieval, creation, update, deletion, and search.
    /// </summary>
    /// <author>Khoinv, Thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantRepository _restaurantsRepository;

        /// <summary>
        /// Constructor that injects the restaurant repository for handling restaurant data.
        /// </summary>
        public RestaurantController(IRestaurantRepository restaurantsRepository)
        {
            _restaurantsRepository = restaurantsRepository;
        }

        /// <summary>
        /// Retrieves a list of all restaurants.
        /// </summary>
        /// <returns>List of restaurants.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_restaurantsRepository.GetRestaurants());
        }

        [HttpGet("admin")]
        public IActionResult GetForAdmin()
        {
            return Ok(_restaurantsRepository.GetRestaurantsForAdmin());
        }

        /// <summary>
        /// Retrieves a restaurant by its ID.
        /// </summary>
        /// <param name="id">The ID of the restaurant.</param>
        /// <returns>Restaurant details or 404 if not found.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int id)
        {
            var restaurant = _restaurantsRepository.FindRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound(string.Format(NotificationConstants.RESTAURANT_WITH_ID_NOT_FOUND, id));
            }
            return Ok(restaurant);
        }

        /// <summary>
        /// Retrieves a restaurant by restaurant owner ID.
        /// </summary>
        /// <param name="id">The ID of the restaurant.</param>
        /// <returns>Restaurant details or 404 if not found.</returns>
        [HttpGet("res_owner_id")]
        public IActionResult GetOneByRestaurantOwner(int res_owner_id)
        {
            var restaurant = _restaurantsRepository.FindRestaurantByRestaurantOwnerId(res_owner_id);
            if (restaurant == null)
            {
                return NotFound(string.Format(NotificationConstants.RESTAURANT_WITH_ID_NOT_FOUND, res_owner_id));
            }
            return Ok(restaurant);
        }

        /// <summary>
        /// Adds a new restaurant.
        /// </summary>
        /// <param name="p">The restaurant object to add.</param>
        /// <returns>Object containing the new restaurant ID.</returns>
        [HttpPost]
        public IActionResult AddRestaurants(Restaurant p)
        {
            _restaurantsRepository.SaveRestaurant(p);
            return Ok(new { res_id = p.res_id });
        }

        /// <summary>
        /// Updates restaurant information.
        /// </summary>
        /// <param name="p">The restaurant object with updated data.</param>
        /// <returns>List of all restaurants after update.</returns>
        [HttpPut]
        public IActionResult UpdateRestaurants(Restaurant p)
        {
            _restaurantsRepository.UpdateRestaurant(p);
            return Ok(p);
        }

        /// <summary>
        /// Deletes a restaurant by its ID.
        /// </summary>
        /// <param name="Id">The ID of the restaurant to delete.</param>
        /// <returns>List of restaurants after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteRestaurants(int Id)
        {
            _restaurantsRepository.DeleteRestaurant(Id);
            return Ok(_restaurantsRepository.GetRestaurants());
        }
        [HttpDelete("block/{Id}")]
        public IActionResult BlockRestaurants(int Id)
        {
            _restaurantsRepository.BlockRestaurant(Id);
            return Ok(_restaurantsRepository.GetRestaurants());
        }
        [HttpPut("active/{Id}")]
        public IActionResult ActiveRestaurants(int Id)
        {
            _restaurantsRepository.ActiveRestaurant(Id);
            return Ok(_restaurantsRepository.GetRestaurants());
        }

        /// <summary>
        /// Searches for restaurants by name and address.
        /// </summary>
        /// <param name="name">The name of the restaurant.</param>
        /// <param name="address">The address of the restaurant.</param>
        /// <returns>List of matching restaurants.</returns>
        [HttpGet("search")]
        public IActionResult SearchRestaurants(string name, string address)
        {
            var result = _restaurantsRepository.SearchRestaurants(name, address);
            return Ok(result);
        }

        [HttpPost("follow")]
        public IActionResult Follow(int cus_id, int res_id)
        {
            bool result = _restaurantsRepository.FollowRestaurant(cus_id, res_id);
            if (!result) return BadRequest("Đã theo dõi hoặc nhà hàng không tồn tại.");
            return Ok("Theo dõi thành công.");
        }

        [HttpDelete("unfollow")]
        public IActionResult Unfollow(int cus_id, int res_id)
        {
            bool result = _restaurantsRepository.UnfollowRestaurant(cus_id, res_id);
            if (!result) return BadRequest("Chưa theo dõi hoặc nhà hàng không tồn tại.");
            return Ok("Bỏ theo dõi thành công.");
        }

        [HttpGet("check-follow")]
        public IActionResult IsFollowing(int cus_id, int res_id)
        {
            bool isFollowing = _restaurantsRepository.IsFollowingRestaurant(cus_id, res_id);
            return Ok(new { isFollowing });
        }

    }
}
