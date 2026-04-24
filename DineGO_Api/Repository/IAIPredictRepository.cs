using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IAIPredictRepository
    {
        Task<int> UpdateTagsToCategoryAsync();
        Task<List<string>> SuggestValidTagsAsync(string text);
        Task<int> UpdatePriorityFromTextAsync(int cusId, string text);
        void AddClickToTag(string tag, int cusId);
        void SetManualPriorityWeight(string tag, int cusId, double weight);
        Task<string> GenerateFoodSuggestionAsync(string userInput);
        Task<object> GetSuggestionWithFoodsAsync(string userInput);
    }
}
