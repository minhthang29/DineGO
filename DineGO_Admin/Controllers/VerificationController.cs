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
    public class VerificationController : Controller
    {
        private readonly ApiService _apiService;

        public VerificationController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 6)
        {
            var verifications = await _apiService.GetAsync<List<Verification>>("Verification");
            foreach (var verification in verifications)
            {
                verification.restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{verification.res_id}");
            
            }
            var pagedList = PaginatedList<Verification>.Create(verifications, pageIndex, pageSize);
            return View(pagedList);
        }
        
        [HttpPost]
        [ActionName("Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var verification = await _apiService.GetAsync<Verification>($"Verification/{id}");
            if (verification == null)
                return RedirectToAction("Index");

            verification.ver_status = 1; // Đã duyệt
            verification.ver_date_verified = DateTime.Now;

            await _apiService.PutAsync<object, dynamic>($"Verification/{id}", verification);
            TempData["SuccessMessage"] = "Duyệt giấy phép thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Reject")]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var verification = await _apiService.GetAsync<Verification>($"Verification/{id}");
            if (verification == null)
                return RedirectToAction("Index");

            verification.ver_status = 2; // Từ chối
            verification.ver_content_responded = reason;
            verification.ver_date_verified = DateTime.Now;

            await _apiService.PutAsync<object, dynamic>($"Verification/{id}", verification);
            TempData["ErrorMessage"] = "Đã bác bỏ giấy phép và gửi phản hồi!";
            return RedirectToAction("Index");
        }
    }
}