using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class SystemLogDAO
    {
        private readonly ApplicationDbContext _context;

        public SystemLogDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thêm mới log
        public async Task<SystemLog> AddAsync(SystemLog log)
        {
            try
            {
                if (log.log_time == null)
                    log.log_time = DateTime.Now;

                _context.SystemLogs.Add(log);
                await _context.SaveChangesAsync();
                return log;
            }
            catch
            {
                return null;
            }
        }

        // Lấy tất cả log
        public async Task<List<SystemLog>> GetAllAsync()
        {
            try
            {
                return await _context.SystemLogs
                    .Include(l => l.admin)
                    .OrderByDescending(l => l.log_time)
                    .ToListAsync();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }

        // Lấy log theo id
        public async Task<SystemLog> GetByIdAsync(int id)
        {
            try
            {
                return await _context.SystemLogs
                    .Include(l => l.admin)
                    .FirstOrDefaultAsync(l => l.sys_log_id == id);
            }
            catch
            {
                return null;
            }
        }

        // Xóa log
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var log = await _context.SystemLogs.FindAsync(id);
                if (log == null) return false;
                _context.SystemLogs.Remove(log);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Xóa log cũ (cleanup)
        public async Task<int> DeleteOldLogsAsync(DateTime cutoffDate)
        {
            try
            {
                var oldLogs = await _context.SystemLogs
                    .Where(l => l.log_time < cutoffDate)
                    .ToListAsync();

                if (oldLogs.Count > 0)
                {
                    _context.SystemLogs.RemoveRange(oldLogs);
                    await _context.SaveChangesAsync();
                }

                return oldLogs.Count;
            }
            catch
            {
                return 0;
            }
        }

        // Lấy log theo bộ lọc
        public async Task<List<SystemLog>> GetFilteredAsync(DateTime? fromDate, DateTime? toDate, 
            int? adminId, string action, bool? isSuccess, int page, int pageSize)
        {
            try
            {
                var query = _context.SystemLogs
                    .Include(s => s.admin)
                    .AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.log_time >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.log_time <= toDate.Value.AddDays(1));

                if (adminId.HasValue)
                    query = query.Where(l => l.ad_id == adminId.Value);

                if (!string.IsNullOrEmpty(action))
                    query = query.Where(l => l.action.Contains(action));

                if (isSuccess.HasValue)
                    query = query.Where(l => l.is_success == isSuccess.Value);

                return await query
                    .OrderByDescending(l => l.log_time)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }

        // Lấy thống kê
        public async Task<Dictionary<string, object>> GetStatsAsync()
        {
            try
            {
                var logs = await _context.SystemLogs.ToListAsync();
                
                var stats = new Dictionary<string, object>
                {
                    ["Total"] = logs.Count,
                    ["Last24Hours"] = logs.Count(l => l.log_time >= DateTime.Now.AddDays(-1)),
                    ["LastWeek"] = logs.Count(l => l.log_time >= DateTime.Now.AddDays(-7)),
                    ["LastMonth"] = logs.Count(l => l.log_time >= DateTime.Now.AddMonths(-1)),
                    ["SuccessRate"] = logs.Count > 0 ? (double)logs.Count(l => l.is_success == true) / logs.Count * 100 : 0,
                    ["SuccessCount"] = logs.Count(l => l.is_success == true),
                    ["FailureCount"] = logs.Count(l => l.is_success == false),
                    ["TopActions"] = logs.Where(l => !string.IsNullOrEmpty(l.action))
                        .GroupBy(l => l.action)
                        .Select(g => new { Action = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(5)
                        .ToList()
                };

                return stats;
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        // Lấy log theo khoảng thời gian
        public async Task<List<SystemLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await _context.SystemLogs
                    .Include(l => l.admin)
                    .Where(l => l.log_time >= fromDate && l.log_time <= toDate)
                    .OrderByDescending(l => l.log_time)
                    .ToListAsync();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }

        // Lấy log theo admin
        public async Task<List<SystemLog>> GetByAdminIdAsync(int adminId)
        {
            try
            {
                return await _context.SystemLogs
                    .Include(l => l.admin)
                    .Where(l => l.ad_id == adminId)
                    .OrderByDescending(l => l.log_time)
                    .ToListAsync();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }

        // Lấy log theo hành động
        public async Task<List<SystemLog>> GetByActionAsync(string action)
        {
            try
            {
                return await _context.SystemLogs
                    .Include(l => l.admin)
                    .Where(l => l.action.Contains(action))
                    .OrderByDescending(l => l.log_time)
                    .ToListAsync();
            }
            catch
            {
                return new List<SystemLog>();
            }
        }
    }
}