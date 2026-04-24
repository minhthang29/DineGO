using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.AdminModel;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminReository;
        public AdminController(IAdminRepository adminReository)
        {
            _adminReository = adminReository;
        }
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAvatarRequest admin)
        {
            Admin admin1 = _adminReository.GetAdminById(admin.ad_id);
            admin1.ad_image = admin.ad_image;
            var result = await _adminReository.UpdateAdminAsync(admin1);
            if (result)
                return Ok(_adminReository.GetAdminById(admin.ad_id));
            return BadRequest();
        }
    }
}