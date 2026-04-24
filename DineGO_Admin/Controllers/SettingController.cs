using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services;

namespace DineGO_Admin.Controllers
{
    [Route("[controller]")]
    public class SettingController : Controller
    {
        private readonly ApiService _apiService;
        public SettingController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost("UpdateTagsAI")]
        public async Task<IActionResult> UpdateTagsAI()
        {
            var result = await _apiService.PostAsync<object, dynamic>("AI/update-tags", null);
            return Json(result);
        }
        public IActionResult Index()
        {
            return View();
        }

    }
}