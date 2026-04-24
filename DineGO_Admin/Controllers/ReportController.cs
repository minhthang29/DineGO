using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Services;

namespace DineGO_Admin.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApiService _apiService;

        public ReportController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _apiService.GetAsync<List<Report>>("Report");
            return View(reports);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report)
        {
            if (!ModelState.IsValid) return View(report);
            await _apiService.PostAsync<Report, Report>("Report", report);
            TempData["SuccessMessage"] = "Thêm báo cáo thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _apiService.GetAsync<Report>($"Report/{id}");
            if (report == null) return NotFound();
            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Report report)
        {
            if (!ModelState.IsValid) return View(report);
            await _apiService.PutAsync<object, dynamic>($"Report/{report.report_id}", report);
            TempData["SuccessMessage"] = "Cập nhật báo cáo thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync<object>($"Report/{id}");
            TempData["SuccessMessage"] = "Xóa báo cáo thành công!";
            return RedirectToAction("Index");
        }
    }
}