using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IReportRepository
    {
        Task<List<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(int id);
        Task<Report> CreateAsync(Report report);
        Task<bool> UpdateAsync(Report report);
        Task<bool> DeleteAsync(int id);
    }
}