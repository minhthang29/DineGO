using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IAdRepository
    {
        // CRUD Slot
        List<AdSlot> GetAllSlots();
        AdSlot? FindSlotById(int slotId);
        void SaveSlot(AdSlot slot);
        void DeleteSlot(int slotId);

        // History
        List<AdHistory> GetHistory(int? resOwnerId = null);
        void SaveHistory(AdHistory history);

        // Đăng ký / sửa quảng cáo
        void SaveRegistration(AdRegistration ad);

        // Xem quảng cáo theo trạng thái
        List<AdRegistration> GetAdsByStatus(bool isActive);

        // Tự động unactive quảng cáo hết hạn
        void DeactivateExpiredAds();

        // Kiểm tra slot có đang được đăng ký không
        bool IsSlotOccupied(int slotId);
        void LogAd(int adId);

    }
}