using System;
using System.Threading.Tasks;
using Core.Models.Client.Custom; // nơi bạn để CustomerPointRequest
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerPointController : ControllerBase
    {
        private readonly ICustomerPointRepository _repo;

        public CustomerPointController(ICustomerPointRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Cộng hoặc trừ điểm cho customer (nếu chưa có ví thì tạo mới).
        /// </summary>
        [HttpPost("update")]
        public IActionResult UpdatePoints([FromBody] CustomerPointRequest request)
        {
            try
            {
                _repo.UpdatePoints(request.CusId, request.ChangeAmount, request.Description);
                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (InvalidOperationException ex) // lỗi nghiệp vụ
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex) // lỗi hệ thống
            {
                return StatusCode(500, new { error = "Lỗi server", detail = ex.Message });
            }
        }


        /// <summary>
        /// Lấy ví điểm theo cusId.
        /// </summary>
        [HttpGet("{cusId}")]
        public IActionResult GetPoint(int cusId)
        {
            var point = _repo.GetPointByCustomerId(cusId);
            if (point == null) return NotFound("Customer point not found");

            return Ok(point);
        }

        /// <summary>
        /// Lấy lịch sử giao dịch điểm theo cusId.
        /// </summary>
        [HttpGet("{cusId}/history")]
        public IActionResult GetHistory(int cusId)
        {
            var history = _repo.GetHistoryByCustomerId(cusId);
            return Ok(history);
        }

        [HttpGet("{cusId}/vouchers/available")]
        public IActionResult GetAvailableVouchers(int cusId)
        {
            var result = _repo.GetAvailableVouchersWithCustomerPoints(cusId);
            return Ok(result);
        }


        [HttpGet("{cusId}/vouchers/owned")]
        public IActionResult GetOwnedVouchers(int cusId)
        {
            var result = _repo.GetOwnedVouchers(cusId);
            return Ok(result);
        }

        [HttpPost("redeem")]
        public IActionResult RedeemVoucher([FromBody] RedeemVoucherRequest request)
        {
            try
            {
                _repo.RedeemVoucher(request.CusId, request.VoucherId);
                return Ok(new { message = "Đổi voucher thành công" });
            }
            catch (InvalidOperationException ex)
            {
                // ✅ Lấy thông báo từ DAO
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("gift")]
        public IActionResult GiftVoucher([FromBody] GiftVoucherRequest request)
        {
            try
            {
                _repo.GiftVoucher(request.SenderId, request.ReceiverId, request.VoucherId);
                return Ok(new { message = "Tặng voucher thành công" });
            }
            catch (InvalidOperationException ex)
            {
                // ✅ Lấy thông báo lỗi từ DAO
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpDelete("{cusId}/voucher/{voucherId}")]
        public IActionResult DeleteCustomerVoucher(int cusId, int voucherId)
        {
            try
            {
                _repo.DeleteCustomerVoucher(cusId, voucherId);
                return Ok(new { message = "Xóa voucher thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
        [HttpPost("transfer")]
        public IActionResult TransferVoucherStock([FromBody] AssignVoucherRequest request)
        {
            try
            {
                _repo.TransferVoucherStockToCustomer(request.CusId, request.VoucherId);
                return Ok(new { message = "Đã chuyển toàn bộ số lượng voucher sang cho customer." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("histories/all")]
        public IActionResult GetAllHistoriesWithCustomerName()
        {
            var histories = _repo.GetAllHistoriesWithCustomerName();
            return Ok(histories);
        }
        [HttpPost("use-voucher")]
        public IActionResult UseVoucher([FromBody] UseVoucherRequest req)
        {
            if (string.IsNullOrEmpty(req.VoucherCode) || req.CustomerId <= 0)
                return BadRequest(new { error = "Dữ liệu không hợp lệ" });

            try
            {
                var msg = _repo.UseVoucher(req.CustomerId, req.VoucherCode);
                return Ok(new { message = msg });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
