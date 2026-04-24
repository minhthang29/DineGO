using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services;
using Core.Models;
using Core.Constant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Admin.Controllers
{
    /// <summary>
    /// Controller for managing restaurant categories (CRUD operations).
    /// </summary>
    /// <author>Phuonghh</author>
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly CategoryService _categoryService;

        /// <summary>
        /// Constructor for CategoryController.
        /// </summary>
        public CategoryController(ILogger<CategoryController> logger, CategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Displays the list of all categories.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        /// <summary>
        /// Displays the add category page.
        /// </summary>
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        /// <summary>
        /// Handles add category POST request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.AddAsync(category);
                    TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.CATEGORY_CREATE_SUCCESS;
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CATEGORY_CREATE_FAILED;
                }
            }
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.DATA_INVALID;
            }
            return View(category);
        }

        /// <summary>
        /// Displays the update category page.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CATEGORY_NOT_FOUND;
                return RedirectToAction("Index");
            }
            return View(category);
        }

        /// <summary>
        /// Handles update category POST request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            if (category == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CATEGORY_UPDATE_FAILED;
                return View(category);
            }

            try
            {
                await _categoryService.UpdateAsync(category);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.CATEGORY_UPDATE_SUCCESS;
                return RedirectToAction("Index");
            }
            catch
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CATEGORY_UPDATE_FAILED;
                return View(category);
            }
        }

        /// <summary>
        /// Displays the delete category confirmation page.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CATEGORY_NOT_FOUND;
                return RedirectToAction("Index");
            }
            return View(category);
        }

        /// <summary>
        /// Handles delete category POST request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Category category)
        {
            try
            {
                await _categoryService.DeleteAsync(category.cate_id);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.CATEGORY_DELETE_SUCCESS;
            }
            catch
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CATEGORY_DELETE_FAILED;
            }
            return RedirectToAction("Index");
        }
    }
}