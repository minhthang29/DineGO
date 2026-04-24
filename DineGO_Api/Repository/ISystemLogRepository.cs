using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface ISystemLogRepository
    {
        Task<List<SystemLog>> GetAllAsync();
        Task<SystemLog> GetByIdAsync(int id);
        Task<SystemLog> AddAsync(SystemLog log);
        Task<bool> DeleteAsync(int id);
        Task<int> DeleteOldLogsAsync(DateTime cutoffDate);
        Task<List<SystemLog>> GetFilteredAsync(DateTime? fromDate, DateTime? toDate, int? adminId, string action, bool? isSuccess, int page, int pageSize);
    }
}