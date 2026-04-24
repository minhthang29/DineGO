using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Client.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApiService _apiService;
        public ReportController(ApiService apiService)
        {
            _apiService = apiService;
        }
        public IActionResult Index()
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null)
            {
                throw new UnauthorizedAccessException();
            }
            return View();
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Report report)
        {
            if (!ModelState.IsValid)
                return View(report);
            report.cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID) ?? 1;
            report.report_created_at = DateTime.Now;
            await _apiService.PostAsync<object, dynamic>("Report", report);
            TempData[KeyConstants.SUCCESS_MESSAGE] = "Báo cáo đã được gửi thành công!";
            return RedirectToAction("Index");
        }
    }
}