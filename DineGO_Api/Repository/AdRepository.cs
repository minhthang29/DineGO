using Core.Models;
using DineGO_Api.Data;
using System.Collections.Generic;

namespace DineGO_Api.Repository
{
    public class AdRepository : IAdRepository
    {
        private readonly AdDAO _dao;

        public AdRepository(ApplicationDbContext context)
        {
            _dao = new AdDAO(context);
        }

        // CRUD Slot
        public List<AdSlot> GetAllSlots() => _dao.GetAllSlots();
        public AdSlot? FindSlotById(int slotId) => _dao.FindSlotById(slotId);
        public void SaveSlot(AdSlot slot) => _dao.SaveSlot(slot);
        public void DeleteSlot(int slotId) => _dao.DeleteSlot(slotId);

        // History
        public List<AdHistory> GetHistory(int? resOwnerId = null) => _dao.GetHistory(resOwnerId);
        public void SaveHistory(AdHistory history) => _dao.SaveHistory(history);

        // Đăng ký / sửa quảng cáo
        public void SaveRegistration(AdRegistration ad) => _dao.SaveRegistration(ad);

        // Xem quảng cáo theo trạng thái
        public List<AdRegistration> GetAdsByStatus(bool isActive) => _dao.GetAdsByStatus(isActive);

        // Tự động unactive quảng cáo hết hạn
        public void DeactivateExpiredAds() => _dao.DeactivateExpiredAds();

        // Kiểm tra slot có đang được đăng ký không
        public bool IsSlotOccupied(int slotId) => _dao.IsSlotOccupied(slotId);
        public void LogAd(int adId) => _dao.LogAd(adId);

    }
}
