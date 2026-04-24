using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
namespace DineGO_Api.Data
{
    public class AdDAO
    {
        private readonly ApplicationDbContext _context;

        public AdDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================
        // CRUD SLOT
        // =====================
        public List<AdSlot> GetAllSlots() => _context.AdSlots.ToList();

        public AdSlot? FindSlotById(int slotId) =>
            _context.AdSlots.SingleOrDefault(s => s.slot_id == slotId);

        public void SaveSlot(AdSlot slot)
        {
            if (slot.slot_id == 0)
                _context.AdSlots.Add(slot);
            else
                _context.AdSlots.Update(slot);
            _context.SaveChanges();
        }

        public void DeleteSlot(int slotId)
        {
            var slot = FindSlotById(slotId);
            if (slot != null)
            {
                _context.AdSlots.Remove(slot);
                _context.SaveChanges();
            }
        }

        // =====================
        // HISTORY
        // =====================
        public List<AdHistory> GetHistory(int? resOwnerId = null)
        {
            if (resOwnerId.HasValue)
            {
                return _context.AdHistories
                    .Where(h => h.res_owner_id == resOwnerId.Value)
                    .ToList();
            }
            return _context.AdHistories.ToList();
        }

        public void SaveHistory(AdHistory history)
        {
            _context.AdHistories.Add(history);
            _context.SaveChanges();
        }

        // =====================
        // ĐĂNG KÝ / SỬA QUẢNG CÁO
        // =====================
        public void SaveRegistration(AdRegistration ad)
        {
            if (ad.ad_id == 0)
                _context.AdRegistrations.Add(ad);
            else
                _context.AdRegistrations.Update(ad);

            _context.SaveChanges();
        }

        // =====================
        // XEM QUẢNG CÁO THEO TRẠNG THÁI
        // =====================
        // trạng thái: 1 = active, 0 = inactive
        public List<AdRegistration> GetAdsByStatus(bool isActive)
        {
            return _context.AdRegistrations
                .Include(r => r.slot)
                .Include(r => r.restaurantOwner)
                .Where(r => r.is_active == isActive)
                .ToList();
        }

        // =====================
        // TỰ ĐỘNG UNACTIVE QUẢNG CÁO HẾT HẠN
        // =====================
        public void DeactivateExpiredAds()
        {
            var now = DateTime.Now;
            var expired = _context.AdRegistrations
                .Where(r => r.end_date < now && r.is_active)
                .ToList();

            foreach (var ad in expired)
            {
                ad.is_active = false;

                // Ghi log
                var history = new AdHistory
                {
                    ad_id = ad.ad_id,
                    slot_id = ad.slot_id,
                    res_owner_id = ad.res_owner_id,
                    start_date = ad.start_date,
                    end_date = ad.end_date,
                    archived_date = now
                };
                _context.AdHistories.Add(history);
            }

            _context.SaveChanges();
        }

        // =====================
        // KIỂM TRA SLOT CÓ ĐANG ĐƯỢC ĐĂNG KÝ HAY KHÔNG
        // =====================
        public bool IsSlotOccupied(int slotId)
        {
            var now = DateTime.Now;
            return _context.AdRegistrations.Any(r =>
                r.slot_id == slotId &&
                r.is_active &&
                r.start_date <= now &&
                r.end_date >= now
            );
        }

        // =====================
        // GHI LOG QUẢNG CÁO THỦ CÔNG (chỉ cần adId, tự lấy slotId & resOwnerId)
        // =====================
        public void LogAd(int adId)
        {
            var ad = _context.AdRegistrations
                .FirstOrDefault(a => a.ad_id == adId);

            if (ad == null) return;

            var history = new AdHistory
            {
                ad_id = ad.ad_id,
                slot_id = ad.slot_id,          
                res_owner_id = ad.res_owner_id, 
                start_date = ad.start_date,
                end_date = ad.end_date,
                archived_date = DateTime.Now
            };

            _context.AdHistories.Add(history);
            _context.SaveChanges();
        }

    }
}