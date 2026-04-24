using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class PriorityDAO
    {
        private readonly ApplicationDbContext _context;

        public PriorityDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ tag ưu tiên của khách
        public List<Priority> GetByCustomerId(int cusId)
        {
            return _context.Priorities
                .Where(p => p.cus_id == cusId)
                .OrderByDescending(p => p.score)
                .ToList();
        }

        // Thêm mới hoặc tăng count nếu đã có (khi AI phân tích từ text)
        public void AddOrIncrement(string tag, int cusId)
        {
            var p = _context.Priorities.FirstOrDefault(x => x.cus_id == cusId && x.tag == tag);

            if (p != null)
            {
                p.count++;
                p.last_used = DateTime.Now;
                p.score = CalculatePriorityScore(p);
                _context.Priorities.Update(p);
            }
            else
            {
                var newP = new Priority
                {
                    cus_id = cusId,
                    tag = tag,
                    count = 1,
                    click_count = 0,
                    last_used = DateTime.Now,
                    weight_manual = 0,
                };
                newP.score = CalculatePriorityScore(newP);
                _context.Priorities.Add(newP);
            }

            _context.SaveChanges();
        }

        // ✅ Tăng click khi người dùng chọn món có tag
        public void AddClick(string tag, int cusId)
        {
            var p = _context.Priorities.FirstOrDefault(x => x.cus_id == cusId && x.tag == tag);
            if (p != null)
            {
                p.click_count++;
                p.last_used = DateTime.Now;
                p.score = CalculatePriorityScore(p);
                _context.Priorities.Update(p);
            }
            else
            {
                var newP = new Priority
                {
                    cus_id = cusId,
                    tag = tag,
                    count = 0,
                    click_count = 1,
                    last_used = DateTime.Now,
                    weight_manual = 0
                };
                newP.score = CalculatePriorityScore(newP);
                _context.Priorities.Add(newP);
            }

            _context.SaveChanges();
        }

        // ✅ Tùy chỉnh manual weight (từ giao diện người dùng)
        public void SetManualWeight(string tag, int cusId, double weight)
        {
            var p = _context.Priorities.FirstOrDefault(x => x.cus_id == cusId && x.tag == tag);
            if (p != null)
            {
                p.weight_manual = weight;
                p.score = CalculatePriorityScore(p);
                _context.Priorities.Update(p);
                _context.SaveChanges();
            }
        }

        // ✅ Tính điểm tổng hợp
        private double CalculatePriorityScore(Priority p)
        {
            double decay = 1.0 / (1 + (DateTime.Now - (p.last_used ?? DateTime.Now)).TotalDays);
            double weight = p.weight_manual ?? 0;

            return (p.count * 0.2) + (p.click_count * 0.6) + decay + weight;
        }

        // Trả về top tag ưu tiên
        public List<string> GetTopTagsByCustomer(int cusId, int limit = 5)
        {
            return _context.Priorities
                .Where(p => p.cus_id == cusId)
                .OrderByDescending(p => p.score)
                .Take(limit)
                .Select(p => p.tag)
                .ToList();
        }
    }
}
