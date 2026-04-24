using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.Models.Client.Custom;

namespace DineGO_Admin.Controllers
{
    public class AdController : Controller
    {
        private readonly AdService _adService;

        public AdController(AdService adService)
        {
            _adService = adService;
        }

        // ===== SLOT CRUD =====

        // Danh sách slot
        public async Task<IActionResult> Index()
        {
            var slots = await _adService.GetAllSlotsAsync();
            return View(slots);
        }

        // Form thêm/sửa slot
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id.HasValue)
            {
                var slots = await _adService.GetAllSlotsAsync();
                var slot = slots.FirstOrDefault(s => s.slot_id == id.Value);
                if (slot != null)
                    return View(slot);
            }
            return View(new AdSlotDto());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdSlotDto dto)
        {
            if (ModelState.IsValid)
            {
                await _adService.SaveSlotAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _adService.DeleteSlotAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ===== HISTORY LOG =====

        public async Task<IActionResult> History()
        {
            var history = await _adService.GetHistoryAsync(); // lấy tất cả, không cần id
            return View(history);
        }
    }
}