using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Core.Models;
using Core.Services;

namespace Core.Middlewares
{
    public class SystemLogMiddleware
    {
        private readonly RequestDelegate _next;

        public SystemLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApiService apiService)
        {
            var log = new SystemLog
            {
                ad_id = context.Session.GetInt32("ad_id"),
                action = DetectHttpMethodFromPath(context),
                description = GetActionDescription(context),
                log_time = DateTime.Now,
                ip_address = context.Connection.RemoteIpAddress?.ToString(),
                device_info = context.Request.Headers["User-Agent"],
                status_code = 0,
                is_success = true
            };

            try
            {
                await _next(context);
                log.status_code = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                log.is_success = false;
                log.status_code = 500;
                log.description = ex.Message;
                throw;
            }
            finally
            {
                // Chỉ log các request không phải static file và không phải GET
                if (!context.Request.Path.Value.Contains(".")
                    && !context.Request.Path.StartsWithSegments("/favicon.ico")
                    && !string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        await apiService.PostAsync<SystemLog, SystemLog>("SystemLog", log);
                    }
                    catch { /* Không làm gián đoạn request nếu log lỗi */ }
                }
            }
        }
        private string DetectHttpMethodFromPath(HttpContext context)
        {
            var path = context.Request.Path.Value?.Trim('/').Split('/');
            if (path == null || path.Length < 2)
                return context.Request.Method;

            var action = path[1].ToLower();

            if (action.Contains("delete") || action.Contains("remove"))
                return "DELETE";
            if (action.Contains("update") || action.Contains("edit"))
                return "PUT";
            // Trường hợp đặc biệt: Block cũng có thể coi là PUT
            if (action.Contains("block") || action.Contains("unblock"))
                return "PUT";

            // Mặc định là POST cho các trường hợp còn lại (Add, Create, ...)
            return "POST";
        }
        // Thêm phương thức helper vào trong class SystemLogMiddleware:
        private string GetActionDescription(HttpContext context)
        {
            var path = context.Request.Path.Value?.Trim('/').Split('/');
            if (path != null && path.Length >= 2 && path[0] != "Auth")
            {
                var controller = path[0];
                var action = path[1];
                string id = path.Length > 2 ? path[2] : null;

                string verb = action.ToLower() switch
                {
                    var a when a.Contains("delete") => "Xóa",
                    var a when a.Contains("update") => "Cập nhật",
                    var a when a.Contains("add") || a.Contains("create") => "Thêm",
                    _ => context.Request.Method switch
                    {
                        "POST" => "Thêm",
                        "PUT" => "Cập nhật",
                        "DELETE" => "Xóa",
                        _ => "Thực hiện"
                    }
                };

                if (!string.IsNullOrEmpty(id))
                    return $"{verb} {controller} có ID là {id}";
                else
                    return $"{verb} {controller}";
            }
            return $"Thực hiện {context.Request.Path}";
        }
    }

}