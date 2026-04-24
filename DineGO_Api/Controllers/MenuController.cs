using Microsoft.AspNetCore.Mvc;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepo;

        public MenuController(IMenuRepository menuRepo)
        {
            _menuRepo = menuRepo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_menuRepo.GetMenus());
        }

        [HttpGet("restaurant/{resId}")]
        public IActionResult GetByRestaurant(int resId)
        {
            return Ok(_menuRepo.GetMenusByRestaurantId(resId));
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var menu = _menuRepo.FindMenuById(id);
            if (menu == null) return NotFound();
            return Ok(menu);
        }

        [HttpPost]
        public IActionResult Create(Menu menu)
        {
            _menuRepo.SaveMenu(menu);
            return Ok(new { menu_id = menu.menu_id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Menu menu)
        {
            if (id != menu.menu_id) return BadRequest("ID không khớp.");
            _menuRepo.UpdateMenu(menu);
            return Ok(menu);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _menuRepo.DeleteMenu(id);
            return Ok();
        }
    }
}
