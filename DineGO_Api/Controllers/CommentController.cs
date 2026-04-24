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
    /// <summary>
    /// Controller for managing comment in post
    /// </summary>
    /// <author>thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;

        /// <summary>
        /// Constructor that injects the comment repository for data access.
        /// </summary>
        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// Retrieves the list of all comments.
        /// </summary>
        /// <returns>List of comments.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_commentRepository.GetComments());
        }

        /// <summary>
        /// Retrieves a comment entry by its ID.
        /// </summary>
        /// <param name="ID">The ID of the comment.</param>
        /// <returns>Comment entry that matches the provided ID.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int ID)
        {
            return Ok(_commentRepository.FindCommentById(ID));
        }

        /// <summary>
        /// Adds a new comment entry.
        /// </summary>
        /// <param name="p">The comment object to add.</param>
        /// <returns>Updated list of comments.</returns>
        [HttpPost]
        public IActionResult AddComment([FromBody] Comment p)
        {
            _commentRepository.SaveComment(p);
            return Ok(_commentRepository.GetComments());
        }

        /// <summary>
        /// Updates an existing comment entry.
        /// </summary>
        /// <param name="p">The comment object with updated data.</param>
        /// <returns>Updated list of comments.</returns>
        [HttpPut]
        public IActionResult UpdateComment([FromBody] Comment p)
        {
            _commentRepository.UpdateComment(p);
            return Ok();
        }
        /// <summary>
        /// Deletes a comment entry by its ID.
        /// </summary>
        /// <param name="Id">The ID of the comment to delete.</param>
        /// <returns>Updated list of comments after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteComment(int Id)
        {
            _commentRepository.DeleteComment(Id);
            return Ok(_commentRepository.GetComments());
        }
    }
}