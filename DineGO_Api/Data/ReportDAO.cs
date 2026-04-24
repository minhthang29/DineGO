using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class ReportDAO
    {
        private readonly ApplicationDbContext _context;
        public ReportDAO(ApplicationDbContext context) => _context = context;

        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports
                .Where(r => !r.report_is_deleted)
                .Include(r => r.customer)
                .Include(r => r.admin)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.customer)
                .Include(r => r.admin)
                .FirstOrDefaultAsync(r => r.report_id == id && !r.report_is_deleted);
        }

        public async Task<Report> CreateAsync(Report report)
        {
            report.report_created_at = DateTime.Now;
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<bool> UpdateAsync(Report report)
        {
            var existing = await _context.Reports.FindAsync(report.report_id);
            if (existing == null || existing.report_is_deleted) return false;

            _context.Entry(existing).CurrentValues.SetValues(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null || report.report_is_deleted) return false;

            report.report_is_deleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}