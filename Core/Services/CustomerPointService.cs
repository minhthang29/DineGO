using Core.Models;
using Core.Models.Client.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.Constant; // nơi chứa ApiEndpoints

namespace Core.Services
{
    public class CustomerPointService
    {
        private readonly ApiService _api;
        private readonly ILogger<CustomerPointService> _logger;

        public CustomerPointService(ApiService api, ILogger<CustomerPointService> logger)
        {
            _api = api;
            _logger = logger;
        }

        // ✅ Cộng hoặc trừ điểm
        public async Task<string> UpdatePointsAsync(CustomerPointRequest request)
        {
            var result = await _api.PostAsync<Dictionary<string, string>, CustomerPointRequest>(
                ApiEndpoints.CUSTOMER_POINT_UPDATE, request);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];

            return "Không có phản hồi";
        }


        // ✅ Lấy ví điểm
        public async Task<CustomerPoint?> GetPointAsync(int cusId)
        {
            var url = string.Format(ApiEndpoints.CUSTOMER_POINT_GET, cusId);
            return await _api.GetAsync<CustomerPoint>(url);
        }

        // ✅ Lấy lịch sử điểm
        public async Task<List<CustomerPointHistory>> GetHistoryAsync(int cusId)
        {
            var url = string.Format(ApiEndpoints.CUSTOMER_POINT_HISTORY, cusId);
            return await _api.GetAsync<List<CustomerPointHistory>>(url);
        }
        // ✅ Lấy danh sách voucher có thể đổi (kèm số dư)
        public async Task<VoucherPointViewmodel.VoucherListResponse?> GetAvailableVouchersAsync(int cusId)
        {
            var url = string.Format(ApiEndpoints.CUSTOMER_POINT_AVAILABLE, cusId);
            return await _api.GetAsync<VoucherPointViewmodel.VoucherListResponse>(url);
        }

        // ✅ Lấy danh sách voucher đã sở hữu (kèm danh sách bạn bè)
        public async Task<VoucherOwnedListResponse?> GetOwnedVouchersAsync(int cusId)
        {
            var url = string.Format(ApiEndpoints.CUSTOMER_POINT_OWNED, cusId);
            return await _api.GetAsync<VoucherOwnedListResponse>(url);
        }

        // ✅ Đổi điểm lấy voucher
        public async Task<string> RedeemVoucherAsync(RedeemVoucherRequest request)
        {
            var result = await _api.PostAsync<Dictionary<string, string>, RedeemVoucherRequest>(
                ApiEndpoints.CUSTOMER_POINT_REDEEM, request);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }

        // ✅ Tặng voucher cho bạn bè
        public async Task<string> GiftVoucherAsync(GiftVoucherRequest request)
        {
            var result = await _api.PostAsync<Dictionary<string, string>, GiftVoucherRequest>(
                ApiEndpoints.CUSTOMER_POINT_GIFT, request);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];
            return "Không có phản hồi";
        }
        public async Task<string> DeleteCustomerVoucherAsync(int cusId, int voucherId)
        {
            var url = string.Format(ApiEndpoints.CUSTOMER_POINT_DELETE, cusId, voucherId);
            var result = await _api.DeleteAsync<Dictionary<string, string>>(url);

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];

            return "Không có phản hồi";
        }
        // Lấy toàn bộ lịch sử điểm (kèm tên customer)
        public async Task<List<CustomerPointHistoryWithName>?> GetAllHistoriesWithCustomerNameAsync()
        {
            return await _api.GetAsync<List<CustomerPointHistoryWithName>>(ApiEndpoints.CUSTOMER_POINT_ALL_HISTORY_WITH_NAME);
        }

        // Chuyển toàn bộ stock voucher sang cho customer
        public async Task<string> TransferVoucherStockToCustomerAsync(int cusId, int voucherId)
        {
            var request = new { CusId = cusId, VoucherId = voucherId };
            var result = await _api.PostAsync<Dictionary<string, string>, object>(
                ApiEndpoints.CUSTOMER_POINT_TRANSFER, request
            );

            if (result != null && result.ContainsKey("message"))
                return result["message"];
            if (result != null && result.ContainsKey("error"))
                return result["error"];

            return "Không có phản hồi";
        }
        public async Task<string> UseVoucherAsync(int customerId, string voucherCode)
        {
            var body = new UseVoucherRequest
            {
                CustomerId = customerId,
                VoucherCode = voucherCode
            };

            var result = await _api.PostAsync<object, UseVoucherRequest>(
                ApiEndpoints.CUSTOMER_POINT_USE_VOUCHER, body);

            if (result is Dictionary<string, object> dict && dict.ContainsKey("message"))
                return dict["message"]?.ToString() ?? "Không có phản hồi";
            if (result is Dictionary<string, object> dictErr && dictErr.ContainsKey("error"))
                return dictErr["error"]?.ToString() ?? "Có lỗi xảy ra";

            return "Không có phản hồi";
        }
    }
}