using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IAdminRepository
    {
        Admin GetAdminById(int id);
        List<Admin> GetAdmins();
        Task<bool> UpdateAdminAsync(Admin admin);
    }
}