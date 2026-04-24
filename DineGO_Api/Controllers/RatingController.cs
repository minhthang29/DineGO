using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Models.Client.Custom;
using DineGO_Api.Repository;

namespace DineGO_Api.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRestaurantRatingRepository _repository;

        public RatingController(IRestaurantRatingRepository repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// Get a list of restaurant ratings
        /// GET: /api/Rating/restaurant/5
        /// </summary>
        [HttpGet("restaurant/{res_id}")]
        public ActionResult<IEnumerable<RestaurantRating>> GetRatingsByRestaurantId(int res_id)
        {
            var ratings = _repository.GetRatingsByRestaurantId(res_id);
            return Ok(ratings);
        }

        /// <summary>
        /// Create or update a restaurant rating
        /// POST: /api/Rating
        /// </summary>
        [HttpPost]
        public IActionResult AddOrUpdateRating([FromBody] RestaurantRatingViewModel model)
        {
            if (!_repository.HasCompletedOrder(model.cus_id, model.res_id))
                return BadRequest();

            var existing = _repository.GetRatingByCustomer(model.cus_id, model.res_id);
            if (existing != null)
            {
                existing.rating_value = model.rating_value;
                existing.rating_comment = model.rating_comment;
                existing.rating_date = DateTime.Now;
                _repository.UpdateRating(existing);
                return Ok(new { status = "updated", rating_id = existing.rating_id });
            }

            var newRating = new RestaurantRating
            {
                cus_id = model.cus_id,
                res_id = model.res_id,
                rating_value = model.rating_value,
                rating_comment = model.rating_comment,
                rating_date = DateTime.Now
            };
            _repository.UpdateAverageRating(model.res_id);
            _repository.AddRating(newRating);
            return Ok(new { status = "created", rating_id = newRating.rating_id });
        }
        /// <summary>
        /// Check if customer has completed any order at this restaurant
        /// </summary>
        [HttpGet("has-completed-order/{cusId}/{resId}")]
        public IActionResult HasCompletedOrder(int cusId, int resId)
        {
            bool result = _repository.HasCompletedOrder(cusId, resId);
            return Ok(result);
        }


    }
}
