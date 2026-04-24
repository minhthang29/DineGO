using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models.Client.Custom;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles actions related to tables, such as listing, viewing details
    /// </summary>
    ///  <author>Thangtm</author>
    [Route("Table")]
    public class TableController : BaseController
    {
        private readonly TableService _tableService;

        public TableController(TableService tableService)
        {
            _tableService = tableService;
        }

        /// <summary>
        /// Displays a list of all tables.
        /// </summary>
        /// <returns>Returns the view with a list of tables.</returns>
        /// <author>Thangtm</author>
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            // int? res_id = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID);
            // if (res_id == null) return RedirectToAction("ProfileRestaurant", "Restaurant");

            // Gọi TableService để lấy danh sách khu vực
            var areas = await _tableService.GetAreasByRestaurantId(resId.Value);

            var viewModel = new CustomTableViewModel
            {
                Areas = areas ?? new List<TableArea>(),
                SelectedAreaId = areas?.FirstOrDefault()?.area_id ?? 0
            };

            return View(viewModel);
        }

        /// <summary>
        /// Displays a list of all tables.
        /// </summary>
        /// <returns>Returns the view with a list of tables.</returns>
        /// <author>Thangtm</author>
        [HttpGet("ViewTableStatus")]
        public async Task<IActionResult> ViewTableStatus()
        {
            // int? res_id = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID);
            // if (res_id == null) return RedirectToAction("ProfileRestaurant", "Restaurant");

            // Gọi TableService để lấy danh sách khu vực
            var areas = await _tableService.GetAreasByRestaurantId(resId.Value);

            var viewModel = new CustomTableViewModel
            {
                Areas = areas ?? new List<TableArea>(),
                SelectedAreaId = areas?.FirstOrDefault()?.area_id ?? 0
            };

            return View(viewModel);
        }

        [HttpGet("GetTables")]
        public async Task<IActionResult> GetTables(int area_id, DateTime date)
        {
            // Lấy danh sách bàn theo khu vực
            var tables = await _tableService.GetTablesByAreaId(area_id);
            // Lấy tất cả reservation theo ngày (rồi lọc theo table_id)
            var reservations = await _tableService.GetReservationsByDate(date);

            var result = tables.Select(t =>
            {
                // tìm reservation ứng với bàn này
                var reser = reservations.FirstOrDefault(r => r.table_id == t.table_id);
                int status;
                if (reser != null)
                {
                    switch (reser.reser_status)
                    {
                        case 0: // reservation mới (pending) -> bàn hiển thị là 1
                            status = 1;
                            break;
                        case 1: // reservation xác nhận -> bàn hiển thị là 2
                            status = 2;
                            break;
                        default:
                            status = t.table_status; // fallback nếu có thêm trạng thái khác
                            break;
                    }
                }
                else
                {
                    status = t.table_status;
                }
                return new
                {
                    id = t.table_id,
                    type = t.table_seat,
                    label = t.table_name,
                    left = "auto",
                    top = "auto",
                    images = t.table_image_json,
                    status = status
                };
            });
            return Ok(result);
        }

        [HttpGet("ReservationHistory")]
        public async Task<IActionResult> ReservationHistory(int? status /* -1: all, 0..3: lọc */)
        {
            var reservations = await _tableService.GetReservationsByRestaurantId(resId.Value);

            if (status.HasValue)
            {
                if (status.Value >= 0)
                {
                    // Lọc theo status cụ thể
                    reservations = reservations
                        .Where(r => r.reser_status == status.Value)
                        .ToList();
                }
                else if (status.Value == -1)
                {
                    // Chỉ lấy các trạng thái 1,3,4
                    reservations = reservations
                        .Where(r => r.reser_status == 1
                                 || r.reser_status == 4
                                 || r.reser_status == 5)
                        .ToList();
                }
            }

            reservations = reservations
                .OrderByDescending(r => r.reser_date)
                .ToList();

            ViewBag.SelectedStatus = status ?? -1;
            return View(reservations);
        }

        [HttpPost("CreateTable")]
        public async Task<IActionResult> CreateTable(string label, int seat, int area_id, List<IFormFile> images)
        {
            int? resId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID);
            if (resId == null) return Unauthorized();

            var success = await _tableService.CreateTable(resId.Value, area_id, label, seat, images);
            if (success)
                return Ok(new { success = true, message = NotificationConstants.CREATE_POST_SUCCESS });
            return StatusCode(500, "Lưu bàn thất bại.");
        }

        [HttpPost("UpdateTable")]
        public async Task<IActionResult> UpdateTable([FromForm] int table_id, [FromForm] string table_name, [FromForm] int table_seat, [FromForm] string old_images, List<IFormFile> images)
        {
            var oldList = string.IsNullOrEmpty(old_images) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(old_images);

            var table = new Table
            {
                table_id = table_id,
                table_name = table_name,
                table_seat = table_seat
            };

            var success = await _tableService.UpdateTableAsync(table, images, oldList);
            return Ok(new { success });
        }

        [HttpPost("DeleteTable")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            // 1️⃣ Kiểm tra bàn có reservation từ hôm nay trở đi
            var reservations = await _tableService.GetReservationsByDate(DateTime.Now);

            var tableIsUse = reservations.Any(r =>
                !r.reser_is_deleted &&
                r.reser_status != 2 &&
                r.table_id == id
            );

            if (tableIsUse)
            {
                return Ok(new 
                { 
                        success = false, 
                    message = "Bàn này đang có đơn đặt bàn từ hôm nay, không thể xóa." 
                });
            }

            // 2️⃣ Xóa bàn (chỉ khi bàn không đang được dùng)
            var deleted = await _tableService.DeleteTable(id); // DeleteTable chỉ cần trả bool thành công/không
            if (deleted)
            {
                return Ok(new { success = true, message = "Xóa bàn thành công." });
            }
            else
            {
                return Ok(new { success = false, message = "Xóa bàn thất bại." });
            }
        }   

        [HttpGet("ViewArea")]
        public async Task<IActionResult> ViewArea()
        {
            var areas = await _tableService.GetAreasByRestaurantId(resId.Value);

            // Lọc bỏ các khu vực bị đánh dấu đã xoá
            var activeAreas = areas?.Where(a => a.is_deleted == false).ToList();

            return View(activeAreas);
        }



        [HttpPost("CreateArea")]
        public async Task<IActionResult> CreateArea(string Name)
        {
            var newAreaId = await _tableService.CreateAreaAsync(resId.Value, Name);
            return Ok(new { success = true, area_id = newAreaId });
        }

        [HttpPost("EditArea")]
        public async Task<IActionResult> EditArea(int id, string Name)
        {
            var (success, message) = await _tableService.EditAreaAsyncWithCheck(id, resId.Value, Name);
            return Ok(new { success, message });
        }

        [HttpPost("DeleteArea")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var (success, message) = await _tableService.DeleteAreaAsyncWithCheck(id);
            return Ok(new { success, message });
        }

        [HttpPost]
        public async Task<IActionResult> EditReservationStatus(int reserId, int newStatus)
        {
            await _tableService.UpdateReservationStatus(reserId, newStatus);
            return RedirectToAction("ReservationHistory");
        }
    }
}