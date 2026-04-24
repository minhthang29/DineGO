using Core.Common;
using Core.Constant;
using Core.Models;
using Core.Services;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace Core.Services
{
    public class TableService
    {
        private readonly ApiService _apiService;
        private readonly ImageHelper _imageHelper;
        private readonly S3BucketAWS _S3;

        public TableService(ApiService apiService, ImageHelper imageHelper, S3BucketAWS S3)
        {
            _apiService = apiService;
            _imageHelper = imageHelper;
            _S3 = S3;
        }

        public async Task<Table> GetTablesById(int table_id)
        {
            return await _apiService.GetAsync<Table>($"{ApiEndpoints.TABLE_BY_ID}{table_id}");
        }

        public async Task<List<TableArea>> GetAreasByRestaurantId(int res_id)
        {
            return await _apiService.GetAsync<List<TableArea>>($"{ApiEndpoints.AREA_BY_RESID}{res_id}");
        }

        public async Task<List<Reservation>> GetReservationsByRestaurantId(int res_id)
        {
            return await _apiService.GetAsync<List<Reservation>>($"{ApiEndpoints.RESERVATION_BY_RESID}{res_id}");
        }

        public async Task<List<Table>> GetTablesByAreaId(int area_id)
        {
            var all = await _apiService.GetAsync<List<Table>>(ApiEndpoints.TABLE);
            return all?.Where(t => t.area_id == area_id && !t.table_is_deleted).ToList();
        }
        public async Task<List<Reservation>> GetReservationsByDate(DateTime date)
        {
            var all = await _apiService.GetAsync<List<Reservation>>(ApiEndpoints.RESERVATION);

            return all
                .Where(r => r.reser_date.Date == date.Date && r.reser_status != 2) // loại hủy
                .GroupBy(r => r.table_id) // gom theo table
                .Select(g => g
                    .OrderByDescending(r => r.reser_date) 
                    .First()
                )
                .ToList();
        }

        public async Task<bool> UpdateTableStatus(int tableId, int newStatus)
        {
            var url = $"{ApiEndpoints.TABLE}/status/{tableId}?table_status={newStatus}";
            var result = await _apiService.PutAsync<object, object>(url, null);
            return result != null;
        }

        public async Task<bool> UpdateReservationStatus(int reserId, int newStatus)
        {
            var url = $"{ApiEndpoints.RESERVATION}/status/{reserId}?reser_status={newStatus}";
            var result = await _apiService.PutAsync<object, object>(url, null);
            return result != null;
        }

        public async Task<bool> CreateTable(int res_id, int area_id, string label, int seat, List<IFormFile> images)
        {
            var fileNames = new List<string>();

            foreach (var image in images)
            {
                var fileName = await _imageHelper.UploadImageWithThumbnailAsync(image, "tables", thumbWidth: 600);
                fileNames.Add(fileName);
            }

            var table = new Table
            {
                res_id = res_id,
                area_id = area_id,
                table_name = label,
                table_seat = seat,
                table_position_x = 0,
                table_position_y = 0,
                table_created_at = DateTime.Now,
                table_is_deleted = false,
                table_image = JsonSerializer.Serialize(fileNames)
            };

            var result = await _apiService.PostAsync<Table, object>(ApiEndpoints.TABLE, table);
            return result != null;
        }

        public async Task<bool> UpdateTableAsync(Table table, List<IFormFile> newImages, List<string> oldImages)
        {
            try
            {
                var existing = await _apiService.GetAsync<Table>($"{ApiEndpoints.TABLE}/id?id={table.table_id}");
                if (existing == null)
                {
                    Console.WriteLine("❌ Table not found");
                    return false;
                }

                var imagesToKeep = oldImages ?? new List<string>();

                // ✅ Dùng table_image_json
                var oldList = existing.table_image_json ?? new List<string>();

                foreach (var oldImg in oldList)
                {
                    if (!imagesToKeep.Contains(oldImg))
                        await _S3.DeleteFileAsync("tables", oldImg);
                }

                // ✅ Upload ảnh mới
                if (newImages != null && newImages.Count > 0)
                {
                    foreach (var img in newImages)
                    {
                        var newName = await _imageHelper.UploadImageWithThumbnailAsync(img, "tables", 600);
                        imagesToKeep.Add(newName);
                    }
                }

                // ✅ Cập nhật lại thông tin bàn
                var payload = new Table
                {
                    table_id = table.table_id,
                    table_name = table.table_name,
                    table_seat = table.table_seat,
                    table_image = JsonSerializer.Serialize(imagesToKeep),
                    table_update_at = DateTime.Now
                };

                var result = await _apiService.PutAsync<Table, Table>(ApiEndpoints.TABLE, payload);
                return result != null;

            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Exception: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteTable(int table_id)
        {
            var list = await _apiService.DeleteAsync<List<Table>>($"{ApiEndpoints.TABLE}?Id={table_id}");
            return list != null;
        }

        public async Task<int?> CreateAreaAsync(int res_id, string name)
        {
            var area = new TableArea
            {
                res_id = res_id,
                area_name = name,
                created_at = DateTime.Now,
                is_deleted = false
            };

            var result = await _apiService.PostAsync<TableArea, TableArea>(ApiEndpoints.TABLE_AREA, area);
            return result?.area_id;
        }


        public async Task<(bool success, string message)> EditAreaAsyncWithCheck(int id, int resId, string name)
        {
            var tables = await GetTablesByAreaId(id);
            if (tables != null && tables.Any(t => t.table_status != 1))
                return (false, "Khu vực có bàn đang được sử dụng, không thể cập nhật.");

            var payload = new TableArea { area_id = id, res_id = resId, area_name = name };
            var result = await _apiService.PutAsync<TableArea, TableArea>(ApiEndpoints.TABLE_AREA, payload);
            return (result != null, result != null ? "Cập nhật khu vực thành công." : "Cập nhật thất bại.");
        }

        public async Task<(bool success, string message)> DeleteAreaAsyncWithCheck(int id)
        {
            var tables = await GetTablesByAreaId(id);
            if (tables != null && tables.Any(t => t.table_status != 1))
                return (false, "Khu vực có bàn đang được sử dụng, không thể xoá.");

            var result = await _apiService.DeleteAsync<List<TableArea>>($"{ApiEndpoints.TABLE_AREA}?Id={id}");
            return (result != null, result != null ? "Xoá khu vực thành công." : "Xoá thất bại.");
        }

    }
}