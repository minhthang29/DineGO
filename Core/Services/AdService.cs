using Core.Models.Client.Custom;
using Core.Constant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public class AdService
    {
        private readonly ApiService _api;

        public AdService(ApiService api)
        {
            _api = api;
        }

        // ===== SLOT =====
        public async Task<List<AdSlotDto>> GetAllSlotsAsync()
        {
            return await _api.GetAsync<List<AdSlotDto>>(ApiEndpoints.AD_SLOTS);
        }

        public async Task<string> SaveSlotAsync(AdSlotDto dto)
        {
            var result = await _api.PostAsync<Dictionary<string, string>, AdSlotDto>(
                ApiEndpoints.AD_SLOT_CREATE_UPDATE, dto);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }

        public async Task<string> DeleteSlotAsync(int slotId)
        {
            var url = string.Format(ApiEndpoints.AD_SLOT_DELETE, slotId);
            var result = await _api.DeleteAsync<Dictionary<string, string>>(url);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }

        public async Task<bool> IsSlotOccupiedAsync(int slotId)
        {
            return await _api.GetAsync<bool>(string.Format(ApiEndpoints.AD_SLOT_OCCUPIED, slotId));
        }

        // ===== REGISTRATION =====
        public async Task<string> RegisterAdAsync(AdRegistrationRequestDto dto)
        {
            var result = await _api.PostAsync<Dictionary<string, string>, AdRegistrationRequestDto>(
                ApiEndpoints.AD_REGISTER, dto);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }

        public async Task<List<AdRegistrationResponseDto>> GetAdsByStatusAsync(bool isActive)
        {
            return await _api.GetAsync<List<AdRegistrationResponseDto>>(
                string.Format(ApiEndpoints.AD_GET_BY_STATUS, isActive));
        }

        // ===== HISTORY =====
        public async Task<List<AdHistoryDto>> GetHistoryAsync(int? resOwnerId = null)
        {
            string endpoint = ApiEndpoints.AD_HISTORY;
            if (resOwnerId.HasValue)
                endpoint += $"?resOwnerId={resOwnerId.Value}";

            return await _api.GetAsync<List<AdHistoryDto>>(endpoint);
        }

        // ===== UTILITIES =====
        public async Task<string> DeactivateExpiredAsync()
        {
            var result = await _api.PostAsync<Dictionary<string, string>, object>(
                ApiEndpoints.AD_DEACTIVATE_EXPIRED, new { });

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }
        public async Task<string> LogAdAsync(int adId)
        {
            var url = $"{ApiEndpoints.AD_LOG}/{adId}";
            var result = await _api.PostAsync<Dictionary<string, string>, object>(url, new { });

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }
    }
}
