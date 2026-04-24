using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for managing categories such as retrieving, adding, updating, and deleting category entries.
    /// </summary>
    /// <author>Thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categorysRepository;

        /// <summary>
        /// Constructor that injects the category repository for data operations.
        /// </summary>
        public CategoryController(ICategoryRepository categorysRepository)
        {
            _categorysRepository = categorysRepository;
        }

        /// <summary>
        /// Retrieves a list of all categories.
        /// </summary>
        /// <returns>List of categories.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_categorysRepository.GetCategories());
        }

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="ID">The ID of the category.</param>
        /// <returns>Category matching the specified ID.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int ID)
        {
            return Ok(_categorysRepository.FindCategoryById(ID));
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="p">The category object to be added.</param>
        /// <returns>Updated list of categories.</returns>
        [HttpPost]
        public IActionResult AddCategorys(Category p)
        {
            _categorysRepository.SaveCategory(p);
            return Ok(_categorysRepository.GetCategories());
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="p">The updated category object.</param>
        /// <returns>Updated list of categories.</returns>
        [HttpPut]
        public IActionResult UpdateCategorys(Category p)
        {
            _categorysRepository.UpdateCategory(p);
            return Ok(_categorysRepository.GetCategories());
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="Id">The ID of the category to be deleted.</param>
        /// <returns>Updated list of categories after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteCategorys(int Id)
        {
            _categorysRepository.DeleteCategory(Id);
            return Ok(_categorysRepository.GetCategories());
        }
    }
}
