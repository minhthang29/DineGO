using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for managing restaurant owner operations including creation, retrieval, update, and deletion.
    /// </summary>
    /// <author>Phuonghh, Sieuhdd</author>
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantOwnerController : Controller
    {
        private readonly IRestaurantOwnerRepository _restaurantOwnerRepository;

        /// <summary>
        /// Constructor that injects the restaurant owner repository.
        /// </summary>
        public RestaurantOwnerController(IRestaurantOwnerRepository restaurantOwnerRepository)
        {
            _restaurantOwnerRepository = restaurantOwnerRepository;
        }

        /// <summary>
        /// Retrieves a list of all restaurant owners.
        /// </summary>
        /// <returns>List of restaurant owners.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_restaurantOwnerRepository.GetRestaurantOwners());
        }

        /// <summary>
        /// Retrieves a specific restaurant owner by ID.
        /// </summary>
        /// <param name="Id">The ID of the restaurant owner.</param>
        /// <returns>Restaurant owner data if found.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int Id)
        {
            return Ok(_restaurantOwnerRepository.FindRestaurantOwnerById(Id));
        }

        /// <summary>
        /// Retrieves restaurant owners associated with a specific customer ID.
        /// </summary>
        /// <param name="Id">The customer ID.</param>
        /// <returns>List of matching restaurant owners.</returns>
        [HttpGet("cusId")]
        public IActionResult GetRestaurantOwnerByCusId(int Id)
        {
            return Ok(_restaurantOwnerRepository.FindRestaurantOwnersByCusId(Id));
        }

        /// <summary>
        /// Adds a new restaurant owner.
        /// </summary>
        /// <param name="restaurantOwner">The restaurant owner object to be added.</param>
        /// <returns>The ID of the newly created restaurant owner.</returns>
        [HttpPost]
        public IActionResult AddOwner(RestaurantOwner restaurantOwner)
        {
            _restaurantOwnerRepository.SaveRestaurantOwner(restaurantOwner);
            return Ok(new { resOwner_id = restaurantOwner.res_owner_id });
        }

        /// <summary>
        /// Updates an existing restaurant owner's information.
        /// </summary>
        /// <param name="restaurantOwner">The updated restaurant owner object.</param>
        /// <returns>List of restaurant owners after update.</returns>
        [HttpPut]
        public IActionResult UpdateOwner(RestaurantOwner restaurantOwner)
        {
            _restaurantOwnerRepository.UpdateRestaurantOwner(restaurantOwner);
            return Ok(new { resOwner_id = restaurantOwner.res_owner_id });
        }

        /// <summary>
        /// Deletes a restaurant owner by ID.
        /// </summary>
        /// <param name="Id">The ID of the restaurant owner to delete.</param>
        /// <returns>List of restaurant owners after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteOwner(int Id)
        {
            _restaurantOwnerRepository.DeleteRestaurantOwner(Id);
            return Ok(new { resOwner_id = Id });
        }
    }
}
