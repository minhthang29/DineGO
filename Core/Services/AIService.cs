using Core.Constant;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class AIService
    {
        private readonly ApiService _api;
        private readonly ILogger<AIService> _logger;

        public AIService(ApiService api, ILogger<AIService> logger)
        {
            _api = api;
            _logger = logger;
        }

        // 🔍 Gợi ý tag từ text
        public async Task<List<string>> SuggestTagsFromTextAsync(string text)
        {
            var url = ApiEndpoints.AI_SUGGEST_TAGS + text;
            System.Diagnostics.Debug.WriteLine("🧪 GỌI TỚI URL: " + url);
            return await _api.GetAsync<List<string>>(url);
        }

        // ✅ Cập nhật tag từ model vào Category
        public async Task<string> UpdateTagsToCategoryAsync()
        {
            var result = await _api.PostAsync<object, object>(ApiEndpoints.AI_UPDATE_CATEGORY_TAGS, null);
            if (result is Dictionary<string, object> dict && dict.ContainsKey("message"))
                return dict["message"]?.ToString() ?? "Không có phản hồi";
            return "Không có phản hồi";
        }

        // 📊 Ghi nhận ưu tiên tag từ text cho customer
        public async Task<string> UpdatePriorityAsync(int cusId, string text)
        {
            var url = string.Format(ApiEndpoints.AI_UPDATE_PRIORITY, cusId, text);
            var result = await _api.PostAsync<object, object>(url, null);
            if (result is Dictionary<string, object> dict && dict.ContainsKey("message"))
                return dict["message"]?.ToString() ?? "Không có phản hồi";
            return "Không có phản hồi";
        }
        // ✅ Ghi nhận khi người dùng click món có tag
        public async Task<string> AddClickToTagAsync(int cusId, string tag)
        {
            var url = string.Format(ApiEndpoints.AI_ADD_CLICK, cusId, tag);
            var result = await _api.PostAsync<object, object>(url, null);
            if (result is Dictionary<string, object> dict && dict.ContainsKey("message"))
                return dict["message"]?.ToString() ?? "Không có phản hồi";
            return "Không có phản hồi";
        }

        // ✅ Cập nhật ưu tiên thủ công
        public async Task<string> SetManualWeightAsync(int cusId, string tag, double weight)
        {
            var url = string.Format(ApiEndpoints.AI_SET_WEIGHT, cusId, tag, weight);
            var result = await _api.PostAsync<object, object>(url, null);
            if (result is Dictionary<string, object> dict && dict.ContainsKey("message"))
                return dict["message"]?.ToString() ?? "Không có phản hồi";
            return "Không có phản hồi";
        }
        public async Task<object> GetFullSuggestionAsync(string input)
        {
            // GỌI đúng kiểu: cả kết quả và dữ liệu là string
            var result = await _api.PostAsync<object, string>(ApiEndpoints.AI_SUGGEST_FULL, input);
            return result!;
        }
    }
}
