using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IVoucherRepository
    {
        Task<Voucher> AddVoucherAsync(Voucher voucher);
        Task<List<Voucher>> GetAllVouchersAsync();
        Task<Voucher> GetVoucherByIdAsync(int id);
        Task<bool> UpdateVoucherAsync(Voucher voucher);
        Task<bool> DeleteVoucherAsync(int id);
        Task<Voucher> GetVoucherByCodeAsync(string code);
    }
}