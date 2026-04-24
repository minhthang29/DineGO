using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using DineGO_Api.Repository;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for managing like reactions on posts.
    /// </summary>
    /// <author>thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;

        /// <summary>
        /// Constructor that injects the like repository for data access.
        /// </summary>
        /// <param name="likeRepository">The like repository interface.</param>
        public LikeController(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        /// <summary>
        /// Retrieves the list of all like reactions.
        /// </summary>
        /// <returns>List of all likes in the system.</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var likes = _likeRepository.GetAll();
            return Ok(likes);
        }

        /// <summary>
        /// Adds or updates a reaction to a post by a customer.
        /// Removes the reaction if emotion type is 0 or less.
        /// </summary>
        /// <param name="request">The Like object containing post ID, customer ID, and emotion type.</param>
        /// <returns>
        /// JSON object containing:
        /// <list type="bullet">
        ///   <item><description>reacted: whether the post was liked</description></item>
        ///   <item><description>newLikeCount: total number of reactions after update</description></item>
        /// </list>
        /// </returns>
        [HttpPost("react")]
        public IActionResult ReactToPost([FromBody] Like request)
        {
            int postId = request.post_id.Value;
            int cusId = request.cus_id.Value;

            if (request.like_emotion_type <= 0)
            {
                _likeRepository.RemoveReaction(postId, cusId);
            }
            else
            {
                _likeRepository.AddOrUpdateReaction(postId, cusId, request.like_emotion_type.Value);
            }

            int newLikeCount = _likeRepository.CountLikes(postId);

            return Ok(new
            {
                reacted = request.like_emotion_type > 0,
                newLikeCount = newLikeCount
            });
        }

        /// <summary>
        /// Retrieves all reactions for a specific post.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve reactions for.</param>
        /// <returns>List of reactions grouped by emotion type for the specified post.</returns>
        [HttpGet("GetReactions/{postId}")]
        public IActionResult GetReactions(int postId)
        {
            var reactions = _likeRepository.GetReactionsByPost(postId);
            return Ok(reactions);
        }
    }
}
