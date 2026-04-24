using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Client.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApiService _apiService;
        public ContactController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return View(contact);
            }
            contact.contact_created_at = DateTime.Now;
            await _apiService.PostAsync<object, dynamic>("Contact", contact);
            TempData["SuccessMessage"] = "Liên hệ đã được gửi thành công!";
            return RedirectToAction("Index");
        }
    }
}