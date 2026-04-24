using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DineGO_Api.Controllers
{
    // <summary>
    /// Controller for managing post of customer
    /// </summary>
    /// <author>thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;

        /// <summary>
        /// Constructor that injects the post repository for data access.
        /// </summary>
        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        /// <summary>
        /// Retrieves the list of all posts.
        /// </summary>
        /// <returns>List of posts.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_postRepository.GetPosts());
        }

        /// <summary>
        /// Retrieves a post entry by its ID.
        /// </summary>
        /// <param name="ID">The ID of the post.</param>
        /// <returns>Post entry that matches the provided ID.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int ID)
        {
            return Ok(_postRepository.FindPostById(ID));
        }

        /// <summary>
        /// Adds a new post entry.
        /// </summary>
        /// <param name="p">The post object to add.</param>
        /// <returns>Updated list of posts.</returns>
        [HttpPost]
        public IActionResult AddPost(Post p)
        {
            _postRepository.SavePost(p);
            return Ok(_postRepository.GetPosts());
        }

        /// <summary>
        /// Updates an existing post entry.
        /// </summary>
        /// <param name="post">The post object with updated data.</param>
        /// <returns>Updated list of posts.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, [FromBody] Post post)
        {
            _postRepository.UpdatePost(post);
            return Ok(_postRepository.GetPosts());
        }

        /// <summary>
        /// Deletes a post entry by its ID.
        /// </summary>
        /// <param name="Id">The ID of the post to delete.</param>
        /// <returns>Updated list of posts after deletion.</returns>
        [HttpDelete]
        public IActionResult DeletePost(int Id)
        {
            _postRepository.DeletePost(Id);
            return Ok(_postRepository.GetPosts());
        }

         /// <summary>
        /// Approves a post (sets post_is_approve to true).
        /// </summary>
        /// <param name="id">The ID of the post to approve.</param>
        /// <returns>Success message.</returns>
        [HttpPut("{id}/approve")]
        public IActionResult ApprovePost(int id)
        {
            var post = _postRepository.FindPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            post.post_is_approve = true;
            post.post_updated_date = DateTime.Now;
            _postRepository.UpdatePost(post);

            return Ok(new { message = "Post approved successfully" });
        }

        /// <summary>
        /// Rejects a post (sets post_is_approve to false).
        /// </summary>
        /// <param name="id">The ID of the post to reject.</param>
        /// <returns>Success message.</returns>
        [HttpPut("{id}/reject")]
        public IActionResult RejectPost(int id)
        {
            var post = _postRepository.FindPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            post.post_is_approve = false;
            post.post_updated_date = DateTime.Now;
            _postRepository.UpdatePost(post);

            return Ok(new { message = "Post rejected successfully" });
        }

    }
}