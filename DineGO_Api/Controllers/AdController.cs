using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Core.Models.Client.Custom;
using Core.Models;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdController : ControllerBase
    {
        private readonly IAdRepository _repo;

        public AdController(IAdRepository repo)
        {
            _repo = repo;
        }

        // ========== SLOT CRUD ==========

        [HttpGet("slots")]
        public IActionResult GetAllSlots()
        {
            var slots = _repo.GetAllSlots()
                .Select(s => new AdSlotDto
                {
                    slot_id = s.slot_id,
                    slot_name = s.slot_name,
                    slot_type = s.slot_type,
                    slot_price = s.slot_price,
                    slot_is_active = s.slot_is_active,
                    occupied = _repo.IsSlotOccupied(s.slot_id)  // check 1 lần ở server
                }).ToList();
            return Ok(slots);
            ;
        }

        [HttpPost("slots")]
        public IActionResult CreateOrUpdateSlot([FromBody] AdSlotDto dto)
        {
            try
            {
                var slot = new AdSlot
                {
                    slot_id = dto.slot_id,
                    slot_name = dto.slot_name,
                    slot_type = dto.slot_type,
                    slot_price = dto.slot_price,
                    slot_is_active = dto.slot_is_active
                };
                _repo.SaveSlot(slot);
                return Ok(new { message = "Lưu slot thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Lỗi khi lưu slot: {ex.Message}" });
            }
        }

        [HttpDelete("slots/{id}")]
        public IActionResult DeleteSlot(int id)
        {
            try
            {
                _repo.DeleteSlot(id);
                return Ok(new { message = "Xóa slot thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Lỗi khi xóa slot: {ex.Message}" });
            }
        }

        [HttpGet("slots/{slotId}/occupied")]
        public IActionResult IsSlotOccupied(int slotId)
        {
            bool occupied = _repo.IsSlotOccupied(slotId);
            return Ok(new { occupied });
        }

        // ========== AD REGISTRATION ==========

        [HttpPost("register")]
        public IActionResult RegisterAd([FromBody] AdRegistrationRequestDto dto)
        {
            try
            {
                var ad = new AdRegistration
                {
                    slot_id = dto.slot_id,
                    res_owner_id = dto.res_owner_id,
                    ad_image_url = dto.ad_image_url,
                    ad_link_url = dto.ad_link_url,
                    start_date = dto.start_date,
                    end_date = dto.end_date,
                    is_active = true
                };
                _repo.SaveRegistration(ad);
                return Ok(new { message = "Đăng ký quảng cáo thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Lỗi khi đăng ký quảng cáo: {ex.Message}" });
            }
        }

        [HttpGet("ads")]
        public IActionResult GetAdsByStatus([FromQuery] bool isActive)
        {
            var ads = _repo.GetAdsByStatus(isActive)
                .Select(a => new AdRegistrationResponseDto
                {
                    ad_id = a.ad_id,
                    slot_id = a.slot_id,
                    slot_name = a.slot.slot_name,
                    slot_type = a.slot.slot_type,   // ✨ thêm dòng này
                    res_owner_id = a.res_owner_id,
                    ad_image_url = a.ad_image_url,
                    ad_link_url = a.ad_link_url,
                    start_date = a.start_date,
                    end_date = a.end_date,
                    is_active = a.is_active
                }).ToList();

            return Ok(ads);
        }


        // ========== HISTORY ==========

        [HttpGet("history")]
        public IActionResult GetHistory([FromQuery] int? resOwnerId)
        {
            var history = _repo.GetHistory(resOwnerId);
            return Ok(history);
        }

        // ========== UTILITIES ==========

        [HttpPost("deactivate-expired")]
        public IActionResult DeactivateExpired()
        {
            try
            {
                _repo.DeactivateExpiredAds();
                return Ok(new { message = "Expired ads deactivated and logged." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Lỗi khi deactivate ads: {ex.Message}" });
            }
        }

        [HttpPost("log/{adId}")]
        public IActionResult LogAd(int adId)
        {
            try
            {
                _repo.LogAd(adId);
                return Ok(new { message = $"Đã ghi log cho quảng cáo {adId}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Lỗi khi ghi log quảng cáo: {ex.Message}" });
            }
        }

    }
}