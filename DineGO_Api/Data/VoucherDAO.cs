using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class VoucherDAO
    {
        private readonly ApplicationDbContext _context;

        public VoucherDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create
        public async Task<Voucher> AddVoucherAsync(Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        // Read all
        public async Task<List<Voucher>> GetAllVouchersAsync()
        {
            return await _context.Vouchers
                .Where(v => !v.voucher_is_deleted)
                .ToListAsync();
        }
        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers
                .FirstOrDefaultAsync(v => v.voucher_code == code);
        }
        // Read by id
        public async Task<Voucher> GetVoucherByIdAsync(int id)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.voucher_id == id);
        }

        // Update
        public async Task<bool> UpdateVoucherAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            return await _context.SaveChangesAsync() > 0;
        }

        // Delete
        public async Task<bool> DeleteVoucherAsync(int id)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.voucher_id == id);
            if (voucher == null) return false;
            voucher.voucher_is_deleted = true;
            _context.Vouchers.Update(voucher);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}