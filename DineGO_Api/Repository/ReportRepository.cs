using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Data;

namespace DineGO_Api.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ReportDAO _reportDao;
        public ReportRepository(ReportDAO reportDao) => _reportDao = reportDao;

        public Task<List<Report>> GetAllAsync() => _reportDao.GetAllAsync();
        public Task<Report?> GetByIdAsync(int id) => _reportDao.GetByIdAsync(id);
        public Task<Report> CreateAsync(Report report) => _reportDao.CreateAsync(report);
        public Task<bool> UpdateAsync(Report report) => _reportDao.UpdateAsync(report);
        public Task<bool> DeleteAsync(int id) => _reportDao.DeleteAsync(id);
    }
}