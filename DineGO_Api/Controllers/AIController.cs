using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIPredictRepository _aiRepo;

        public AIController(IAIPredictRepository aiRepo)
        {
            _aiRepo = aiRepo;
        }
        [HttpPost("update-tags")]
        public async Task<IActionResult> UpdateTagsToCategory()
        {
            var count = await _aiRepo.UpdateTagsToCategoryAsync();
            return Ok(new { message = $"Đã thêm {count} tag mới vào Category." });
        }
        [HttpGet("suggest-tags")]
        public async Task<IActionResult> SuggestTagsFromText([FromQuery] string text)
        {
            var tags = await _aiRepo.SuggestValidTagsAsync(text);
            return Ok(tags);
        }
        [HttpPost("priority/update")]
        public async Task<IActionResult> UpdatePriority([FromQuery] int cusId, [FromQuery] string text)
        {
            var updated = await _aiRepo.UpdatePriorityFromTextAsync(cusId, text);
            return Ok(new { message = $"Đã ghi nhận {updated} tag ưu tiên cho khách hàng {cusId}." });
        }
        [HttpPost("priority/click")]
        public IActionResult RecordClick([FromQuery] int cusId, [FromQuery] string tag)
        {
            _aiRepo.AddClickToTag(tag, cusId);
            return Ok(new { message = "Đã ghi nhận click cho tag: " + tag });
        }

        [HttpPost("priority/set-weight")]
        public IActionResult SetManualWeight([FromQuery] int cusId, [FromQuery] string tag, [FromQuery] double weight)
        {
            _aiRepo.SetManualPriorityWeight(tag, cusId, weight);
            return Ok(new { message = $"Đã cập nhật ưu tiên thủ công tag {tag} thành {weight}" });
        }

        [HttpGet("suggest-food")]
        public async Task<IActionResult> SuggestFood([FromQuery] string text)
        {
            try
            {
                var response = await _aiRepo.GenerateFoodSuggestionAsync(text);
                return Ok(new { suggestion = response });
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết ra nếu cần
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("suggest-full")]
        public async Task<IActionResult> SuggestFullFlow([FromBody] string text)
        {
            var result = await _aiRepo.GetSuggestionWithFoodsAsync(text);
            return Ok(result);
        }
    }
}