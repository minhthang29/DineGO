using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IVerificationRepository
    {
        Task<Verification> AddVerificationAsync(Verification verification);
        Task<List<Verification>> GetAllVerificationsAsync();
        Task<Verification> GetVerificationByIdAsync(int id);
        Task<bool> UpdateVerificationAsync(Verification verification);
        Task<bool> DeleteVerificationAsync(int id);
        Task<List<Verification>> GetVerificationsByRestaurantIdAsync(int res_id);
    }
}