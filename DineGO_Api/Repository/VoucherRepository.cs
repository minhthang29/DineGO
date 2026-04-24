using System.Collections.Generic;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly VoucherDAO _voucherDAO;

        public VoucherRepository(VoucherDAO voucherDAO)
        {
            _voucherDAO = voucherDAO;
        }

        public Task<Voucher> AddVoucherAsync(Voucher voucher)
            => _voucherDAO.AddVoucherAsync(voucher);

        public Task<List<Voucher>> GetAllVouchersAsync()
            => _voucherDAO.GetAllVouchersAsync();

        public Task<Voucher> GetVoucherByIdAsync(int id)
            => _voucherDAO.GetVoucherByIdAsync(id);

        public Task<bool> UpdateVoucherAsync(Voucher voucher)
            => _voucherDAO.UpdateVoucherAsync(voucher);

        public Task<bool> DeleteVoucherAsync(int id)
            => _voucherDAO.DeleteVoucherAsync(id);
        public Task<Voucher> GetVoucherByCodeAsync(string code)
            => _voucherDAO.GetVoucherByCodeAsync(code);
    }
}