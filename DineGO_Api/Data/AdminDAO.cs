using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class AdminDAO
    {
        private readonly ApplicationDbContext _context;

        public AdminDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Admins
        public List<Admin> GetAdmins()
        {
            try
            {
                return _context.Admins.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Customers: {e.Message}");
            }
        }
        public Admin GetAdminById(int id)
        {
            try
            {
                return _context.Admins.FirstOrDefault(a => a.ad_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Admin by ID: {e.Message}");
            }
        }

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            try
            {
                _context.Admins.Update(admin);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}