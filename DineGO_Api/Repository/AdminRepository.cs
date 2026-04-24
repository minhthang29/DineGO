using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace DineGO_Api.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AdminDAO _adminDAO;

        public AdminRepository(AdminDAO adminDAO)
        {
            _adminDAO = adminDAO;
        }
        public Admin GetAdminById(int id) => _adminDAO.GetAdminById(id);
        public List<Admin> GetAdmins() => _adminDAO.GetAdmins();
        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            return await _adminDAO.UpdateAdminAsync(admin);
        }
    }
}