using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Common;

namespace DineGO_Admin.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApiService _apiService;
        public ContactController(ApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {
            var contacts = await _apiService.GetAsync<List<Contact>>("Contact");
            return View(contacts);
        }

    }
}