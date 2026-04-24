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
using Core.Models.Client.Custom;
using Microsoft.Extensions.Logging;


namespace DineGO_Admin.Controllers
{
    public class VoucherController : Controller
    {
        private readonly ApiService _apiService;
        private readonly CustomerPointService _pointService;

        public VoucherController(ApiService apiService, ILogger<CustomerPointService> logger)
        {
            _apiService = apiService;
            _pointService = new CustomerPointService(apiService, logger);
        }

        public async Task<IActionResult> Index()
        {
            var vouchers = await _apiService.GetAsync<List<Voucher>>("Voucher");
            return View(vouchers);
        }
        public async Task<IActionResult> UpdatePoints()
        {
            // Lấy danh sách customer để hiển thị trong dropdown
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            ViewBag.Customers = customers;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePoints(int cusId, int changeAmount, string description)
        {
            var request = new CustomerPointRequest
            {
                CusId = cusId,
                ChangeAmount = changeAmount,
                Description = description
            };

            await _pointService.UpdatePointsAsync(request);

            TempData["SuccessMessage"] = $"Đã cộng {changeAmount} điểm cho khách hàng ID={cusId}";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Create()
        {
            // lấy danh sách customer
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);

            // đẩy vào ViewBag để view hiển thị dropdown
            ViewBag.Customers = customers;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Voucher voucher, int? cusId)
        {
            // ✅ Check giới hạn %
            if (voucher.voucher_type == 0 && voucher.voucher_discount > 50)
            {
                TempData["ErrorMessage"] = "Voucher giảm theo % chỉ được phép tối đa 50%";
                // nạp lại customer list để view render
                var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
                ViewBag.Customers = customers;
                return View(voucher);
            }

            if (!ModelState.IsValid)
            {
                var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
                ViewBag.Customers = customers;
                return View(voucher);
            }

            var created = await _apiService.PostAsync<Voucher, Voucher>(ApiEndpoints.VOUCHER, voucher);
            if (voucher.voucher_apply_type == 1 && cusId.HasValue)
            {
                await _pointService.TransferVoucherStockToCustomerAsync(cusId.Value, created.voucher_id);
                var logRequest = new CustomerPointRequest
                {
                    CusId = cusId.Value,
                    ChangeAmount = 0,
                    Description = $"Admin đã tặng voucher {created.voucher_code}"
                };
                await _pointService.UpdatePointsAsync(logRequest);
            }

            TempData["SuccessMessage"] = "Thêm voucher thành công!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var voucher = await _apiService.GetAsync<Voucher>($"Voucher/{id}");
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Voucher voucher)
        {
            // ✅ Check giới hạn %
            if (voucher.voucher_type == 0 && voucher.voucher_discount > 50)
            {
                TempData["ErrorMessage"] = "Voucher giảm theo % chỉ được phép tối đa 50%";
                return View(voucher);
            }

            if (!ModelState.IsValid)
                return View(voucher);

            await _apiService.PutAsync<Voucher, object>($"Voucher/{voucher.voucher_id}", voucher);
            TempData["SuccessMessage"] = "Cập nhật voucher thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync<object>($"Voucher/{id}");
            TempData["SuccessMessage"] = "Xóa voucher thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Histories()
        {
            var histories = await _pointService.GetAllHistoriesWithCustomerNameAsync();
            return View(histories ?? new List<CustomerPointHistoryWithName>());
        }
    }
}