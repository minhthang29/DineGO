using System.Collections.Generic;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly VerificationDAO _verificationDAO;

        public VerificationRepository(VerificationDAO verificationDAO)
        {
            _verificationDAO = verificationDAO;
        }
        public Task<Verification> AddVerificationAsync(Verification verification)
            => _verificationDAO.AddVerificationAsync(verification);

        public Task<List<Verification>> GetAllVerificationsAsync()
            => _verificationDAO.GetAllVerificationsAsync();

        public Task<Verification> GetVerificationByIdAsync(int id)
            => _verificationDAO.GetVerificationByIdAsync(id);

        public Task<bool> UpdateVerificationAsync(Verification verification)
            => _verificationDAO.UpdateVerificationAsync(verification);

        public Task<bool> DeleteVerificationAsync(int id)
            => _verificationDAO.DeleteVerificationAsync(id);
        public Task<List<Verification>> GetVerificationsByRestaurantIdAsync(int res_id)
    => _verificationDAO.GetVerificationsByRestaurantIdAsync(res_id);
    }
}