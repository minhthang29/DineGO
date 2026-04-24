using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class SystemLogRepository : ISystemLogRepository
    {
        private readonly SystemLogDAO _systemLogDAO;

        public SystemLogRepository(SystemLogDAO systemLogDAO)
        {
            _systemLogDAO = systemLogDAO;
        }

        public Task<SystemLog> AddAsync(SystemLog log)
            => _systemLogDAO.AddAsync(log);

        public Task<List<SystemLog>> GetAllAsync()
            => _systemLogDAO.GetAllAsync();

        public Task<SystemLog> GetByIdAsync(int id)
            => _systemLogDAO.GetByIdAsync(id);

        public Task<bool> DeleteAsync(int id)
            => _systemLogDAO.DeleteAsync(id);

        public Task<int> DeleteOldLogsAsync(DateTime cutoffDate)
            => _systemLogDAO.DeleteOldLogsAsync(cutoffDate);

        public Task<List<SystemLog>> GetFilteredAsync(DateTime? fromDate, DateTime? toDate, 
            int? adminId, string action, bool? isSuccess, int page, int pageSize)
            => _systemLogDAO.GetFilteredAsync(fromDate, toDate, adminId, action, isSuccess, page, pageSize);

        // Các method bổ sung
        public Task<Dictionary<string, object>> GetStatsAsync()
            => _systemLogDAO.GetStatsAsync();

        public Task<List<SystemLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
            => _systemLogDAO.GetByDateRangeAsync(fromDate, toDate);

        public Task<List<SystemLog>> GetByAdminIdAsync(int adminId)
            => _systemLogDAO.GetByAdminIdAsync(adminId);

        public Task<List<SystemLog>> GetByActionAsync(string action)
            => _systemLogDAO.GetByActionAsync(action);
    }
}