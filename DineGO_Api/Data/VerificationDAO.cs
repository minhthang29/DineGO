using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class VerificationDAO
    {
        private readonly ApplicationDbContext _context;

        public VerificationDAO(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Verification> AddVerificationAsync(Verification verification)
        {
            _context.Verifications.Add(verification);
            await _context.SaveChangesAsync();
            return verification;
        }

        public async Task<List<Verification>> GetAllVerificationsAsync()
        {
            return await _context.Verifications.ToListAsync();
        }

        public async Task<Verification> GetVerificationByIdAsync(int id)
        {
            return await _context.Verifications.FirstOrDefaultAsync(v => v.ver_id == id);
        }

        public async Task<bool> UpdateVerificationAsync(Verification verification)
        {
            _context.Verifications.Update(verification);
            // Nếu verification được duyệt (status = 1), cập nhật authorization cho restaurant
            if (verification.ver_status == 1)
            {
                var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.res_id == verification.res_id);
                if (restaurant != null)
                {
                    restaurant.res_is_authorized = true;
                    _context.Restaurants.Update(restaurant);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVerificationAsync(int id)
        {
            var verification = await _context.Verifications.FirstOrDefaultAsync(v => v.ver_id == id);
            if (verification == null) return false;
            _context.Verifications.Remove(verification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Verification>> GetVerificationsByRestaurantIdAsync(int res_id)
        {
            return await _context.Verifications
                .Where(v => v.res_id == res_id)
                .ToListAsync();
        }

        
    }
}