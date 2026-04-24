using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for managing blog data such as creating, updating, deleting, and retrieving blog entries.
    /// </summary>
    /// <author>Sieuhdd</author>
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogRepositoy _blogRepositoy;

        /// <summary>
        /// Constructor that injects the blog repository for data access.
        /// </summary>
        public BlogController(IBlogRepositoy blogRepositoy)
        {
            _blogRepositoy = blogRepositoy;
        }

        /// <summary>
        /// Retrieves the list of all blogs.
        /// </summary>
        /// <returns>List of blogs.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_blogRepositoy.GetBlogs());
        }

        /// <summary>
        /// Retrieves a blog entry by its ID.
        /// </summary>
        /// <param name="ID">The ID of the blog.</param>
        /// <returns>Blog entry that matches the provided ID.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int ID)
        {
            return Ok(_blogRepositoy.FindBlogById(ID));
        }

        /// <summary>
        /// Adds a new blog entry.
        /// </summary>
        /// <param name="p">The blog object to add.</param>
        /// <returns>Updated list of blogs.</returns>
        [HttpPost]
        public IActionResult Addblog(Blog p)
        {
            _blogRepositoy.SaveBlog(p);
            return Ok(_blogRepositoy.GetBlogs());
        }

        /// <summary>
        /// Updates an existing blog entry.
        /// </summary>
        /// <param name="p">The blog object with updated data.</param>
        /// <returns>Updated list of blogs.</returns>
        [HttpPut]
        public IActionResult Updateblog(Blog p)
        {
            _blogRepositoy.UpdateBlog(p);
            return Ok(_blogRepositoy.GetBlogs());
        }

        /// <summary>
        /// Deletes a blog entry by its ID.
        /// </summary>
        /// <param name="Id">The ID of the blog to delete.</param>
        /// <returns>Updated list of blogs after deletion.</returns>
        [HttpDelete]
        public IActionResult Deleteblog(int ID)
        {
            _blogRepositoy.DeleteBlog(ID);
            return Ok(_blogRepositoy.GetBlogs());
        }
    }
}
