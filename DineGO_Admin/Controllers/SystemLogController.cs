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
using System.Text;
using System.IO;

namespace DineGO_Admin.Controllers
{
    [Route("[controller]")]
    public class SystemLogController : Controller
    {
        private readonly ApiService _apiService;

        public SystemLogController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, 
            DateTime? fromDate = null, DateTime? toDate = null, 
            int? adminId = null, string action = null, bool? isSuccess = null)
        {
            try
            {
                var systemLogs = await _apiService.GetAsync<List<SystemLog>>("SystemLog");
                
                // Null safety check - most important!
                if (systemLogs == null)
                {
                    systemLogs = new List<SystemLog>();
                }

                // DEBUG: Log original count and sample data
                System.Console.WriteLine($"Original count: {systemLogs.Count}");
                if (systemLogs.Any())
                {
                    System.Console.WriteLine($"First log time: {systemLogs.First().log_time}");
                    System.Console.WriteLine($"Last log time: {systemLogs.Last().log_time}");
                    
                    // DEBUG: Show first 3 actions
                    var firstActions = systemLogs.Take(3).Select(l => $"ID:{l.sys_log_id}, Action:'{l.action}'").ToList();
                    System.Console.WriteLine($"Sample actions: {string.Join("; ", firstActions)}");
                }
                
                // DEBUG: Log filter parameters
                System.Console.WriteLine($"From Date: {fromDate}");
                System.Console.WriteLine($"To Date: {toDate}");
                System.Console.WriteLine($"Action Filter: '{action}'");
                System.Console.WriteLine($"AdminId: {adminId}");
                System.Console.WriteLine($"IsSuccess: {isSuccess}");

                if (adminId.HasValue)
                {
                    var beforeFilter = systemLogs.Count;
                    systemLogs = systemLogs.Where(l => l.ad_id == adminId.Value).ToList();
                    System.Console.WriteLine($"After admin filter {adminId}: {beforeFilter} -> {systemLogs.Count}");
                }

                if (isSuccess.HasValue)
                {
                    var beforeFilter = systemLogs.Count;
                    systemLogs = systemLogs.Where(l => l.is_success == isSuccess.Value).ToList();
                    System.Console.WriteLine($"After success filter {isSuccess}: {beforeFilter} -> {systemLogs.Count}");
                }

                if (fromDate.HasValue)
                {
                    var beforeFilter = systemLogs.Count;
                    var fromDateUtc = fromDate.Value.Date;
                    
                    // DEBUG: Check some dates before filtering
                    var sampleDates = systemLogs.Take(3).Select(l => new {
                        ID = l.sys_log_id,
                        LogTime = l.log_time,
                        DatePart = l.log_time?.Date,
                        CompareResult = l.log_time?.Date >= fromDateUtc
                    }).ToList();
                    
                    System.Console.WriteLine($"Sample date comparisons for fromDate {fromDateUtc}:");
                    foreach (var sample in sampleDates)
                    {
                        System.Console.WriteLine($"  ID:{sample.ID}, Time:{sample.LogTime}, Date:{sample.DatePart}, >= {fromDateUtc}: {sample.CompareResult}");
                    }
                    
                    systemLogs = systemLogs.Where(l => l.log_time.HasValue && 
                        l.log_time.Value.Date >= fromDateUtc).ToList();
                    System.Console.WriteLine($"After from date filter ({fromDateUtc}): {beforeFilter} -> {systemLogs.Count}");
                }

                if (toDate.HasValue)
                {
                    var beforeFilter = systemLogs.Count;
                    var toDateUtc = toDate.Value.Date.AddDays(1).AddTicks(-1);
                    systemLogs = systemLogs.Where(l => l.log_time.HasValue && 
                        l.log_time.Value <= toDateUtc).ToList();
                    System.Console.WriteLine($"After to date filter ({toDateUtc}): {beforeFilter} -> {systemLogs.Count}");
                }

                // Sort by latest first with null safety
                systemLogs = systemLogs.OrderByDescending(l => l.log_time ?? DateTime.MinValue).ToList();
                
                System.Console.WriteLine($"Final count after all filters: {systemLogs.Count}");

                var pagedList = PaginatedList<SystemLog>.Create(systemLogs, pageIndex, pageSize);

                // Pass filter values to view
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                ViewBag.AdminId = adminId;
                ViewBag.Action = action;
                ViewBag.IsSuccess = isSuccess;

                // Get admin list for filter dropdown with null safety
                try
                {
                    var admins = await _apiService.GetAsync<List<Admin>>("Admin");
                    ViewBag.Admins = admins ?? new List<Admin>();
                }
                catch
                {
                    ViewBag.Admins = new List<Admin>();
                }

                return View(pagedList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải nhật ký: " + ex.Message;
                System.Console.WriteLine($"Error in Index: {ex.Message}");
                return View(new PaginatedList<SystemLog>(new List<SystemLog>(), 0, 1, 10));
            }
        }

        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportCsv(DateTime? fromDate = null, DateTime? toDate = null, 
            int? adminId = null, string action = null, bool? isSuccess = null)
        {
            try
            {
                var systemLogs = await GetFilteredLogs(fromDate, toDate, adminId, action, isSuccess);
                
                // Null safety check
                if (systemLogs == null)
                {
                    systemLogs = new List<SystemLog>();
                }

                var csv = new StringBuilder();
                // Add UTF-8 BOM for Excel compatibility
                csv.Append('\uFEFF');
                csv.AppendLine("ID,Admin,Hành động,Mô tả,Thời gian,IP,Thiết bị,Mã trạng thái,Thành công");

                foreach (var log in systemLogs)
                {
                    // Null safety for each field
                    var id = log?.sys_log_id ?? 0;
                    var adminName = log?.admin?.ad_name ?? "N/A";
                    var actionValue = log?.action ?? "";
                    var description = log?.description ?? "";
                    var logTime = log?.log_time?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    var ipAddress = log?.ip_address ?? "";
                    var deviceInfo = log?.device_info ?? "";
                    var statusCode = log?.status_code ?? 0;
                    var isSuccessValue = log?.is_success == true ? "Có" : "Không";

                    csv.AppendLine($"{id}," +
                                  $"\"{adminName.Replace("\"", "\"\"")}\"," +
                                  $"\"{actionValue.Replace("\"", "\"\"")}\"," +
                                  $"\"{description.Replace("\"", "\"\"")}\"," +
                                  $"\"{logTime}\"," +
                                  $"\"{ipAddress.Replace("\"", "\"\"")}\"," +
                                  $"\"{deviceInfo.Replace("\"", "\"\"")}\"," +
                                  $"{statusCode}," +
                                  $"{isSuccessValue}");
                }

                var fileName = $"SystemLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất CSV: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel(DateTime? fromDate = null, DateTime? toDate = null, 
            int? adminId = null, string action = null, bool? isSuccess = null)
        {
            try
            {
                var systemLogs = await GetFilteredLogs(fromDate, toDate, adminId, action, isSuccess);
                
                // Null safety check
                if (systemLogs == null)
                {
                    systemLogs = new List<SystemLog>();
                }

                // Create HTML table for Excel
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset='utf-8'>");
                html.AppendLine("<title>Nhật ký hệ thống</title>");
                html.AppendLine("<style>");
                html.AppendLine("table { border-collapse: collapse; width: 100%; }");
                html.AppendLine("th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                html.AppendLine("<h1>NHẬT KÝ HỆ THỐNG</h1>");
                html.AppendLine($"<p>Xuất ngày: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
                html.AppendLine($"<p>Tổng số bản ghi: {systemLogs.Count}</p>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>ID</th><th>Admin</th><th>Hành động</th><th>Mô tả</th><th>Thời gian</th><th>IP</th><th>Thiết bị</th><th>Status</th><th>Thành công</th>");
                html.AppendLine("</tr>");

                foreach (var log in systemLogs)
                {
                    // Null safety for each field
                    var id = log?.sys_log_id ?? 0;
                    var adminName = log?.admin?.ad_name ?? "N/A";
                    var actionValue = log?.action ?? "";
                    var description = log?.description ?? "";
                    var logTime = log?.log_time?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    var ipAddress = log?.ip_address ?? "";
                    var deviceInfo = TruncateString(log?.device_info ?? "", 30);
                    var statusCode = log?.status_code ?? 0;
                    var isSuccessValue = log?.is_success == true ? "Có" : "Không";

                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{id}</td>");
                    html.AppendLine($"<td>{adminName}</td>");
                    html.AppendLine($"<td>{actionValue}</td>");
                    html.AppendLine($"<td>{description}</td>");
                    html.AppendLine($"<td>{logTime}</td>");
                    html.AppendLine($"<td>{ipAddress}</td>");
                    html.AppendLine($"<td>{deviceInfo}</td>");
                    html.AppendLine($"<td>{statusCode}</td>");
                    html.AppendLine($"<td>{isSuccessValue}</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");

                var fileName = $"SystemLog_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                var bytes = Encoding.UTF8.GetBytes(html.ToString());
                
                return File(bytes, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất Excel: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("cleanup")]
        public async Task<IActionResult> CleanupOldLogs()
        {
            try
            {
                var result = await _apiService.DeleteAsync<object>("SystemLog/cleanup");
                TempData["SuccessMessage"] = "Dọn dẹp nhật ký cũ thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi dọn dẹp: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var systemLogs = await _apiService.GetAsync<List<SystemLog>>("SystemLog");
                
                // Null safety check
                if (systemLogs == null)
                {
                    systemLogs = new List<SystemLog>();
                }

                var stats = new
                {
                    Total = systemLogs.Count,
                    Last24Hours = systemLogs.Count(l => l.log_time.HasValue && l.log_time >= DateTime.Now.AddDays(-1)),
                    LastWeek = systemLogs.Count(l => l.log_time.HasValue && l.log_time >= DateTime.Now.AddDays(-7)),
                    LastMonth = systemLogs.Count(l => l.log_time.HasValue && l.log_time >= DateTime.Now.AddMonths(-1)),
                    SuccessRate = systemLogs.Count > 0 ? (double)systemLogs.Count(l => l.is_success == true) / systemLogs.Count * 100 : 0,
                    TopActions = systemLogs
                        .Where(l => !string.IsNullOrEmpty(l.action))
                        .GroupBy(l => l.action)
                        .Select(g => new { Action = g.Key ?? "Unknown", Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(5)
                        .ToList(),
                    TopAdmins = systemLogs
                        .Where(l => l.admin != null && !string.IsNullOrEmpty(l.admin.ad_name))
                        .GroupBy(l => l.admin.ad_name)
                        .Select(g => new { Admin = g.Key ?? "Unknown", Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(5)
                        .ToList()
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task<List<SystemLog>> GetFilteredLogs(DateTime? fromDate, DateTime? toDate, 
            int? adminId, string action, bool? isSuccess)
        {
            try
            {
                var systemLogs = await _apiService.GetAsync<List<SystemLog>>("SystemLog");

                // Null safety check
                if (systemLogs == null)
                {
                    return new List<SystemLog>();
                }

                if (adminId.HasValue)
                {
                    systemLogs = systemLogs.Where(l => l.ad_id == adminId.Value).ToList();
                }

                if (isSuccess.HasValue)
                {
                    systemLogs = systemLogs.Where(l => l.is_success == isSuccess.Value).ToList();
                }

                if (fromDate.HasValue)
                {
                    systemLogs = systemLogs.Where(l => l.log_time.HasValue && l.log_time >= fromDate.Value).ToList();
                }

                if (toDate.HasValue)
                {
                    systemLogs = systemLogs.Where(l => l.log_time.HasValue && l.log_time <= toDate.Value.AddDays(1)).ToList();
                }

                return systemLogs.OrderByDescending(l => l.log_time ?? DateTime.MinValue).ToList();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }

        private string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}