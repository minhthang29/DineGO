using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DineGO_Api.Repository;
using Core.Models;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        // GET: api/voucher
        [HttpGet]
        public async Task<ActionResult<List<Voucher>>> GetAll()
        {
            var vouchers = await _voucherRepository.GetAllVouchersAsync();
            return Ok(vouchers);
        }

        // GET: api/voucher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Voucher>> GetById(int id)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(id);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }
        // GET: api/voucher/code/{code}
        [HttpGet("code/{code}")]
        public async Task<ActionResult<Voucher>> GetByCode(string code)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(code);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }

        // POST: api/voucher
        [HttpPost]
        public async Task<ActionResult<Voucher>> Create([FromBody] Voucher voucher)
        {
            var created = await _voucherRepository.AddVoucherAsync(voucher);
            return CreatedAtAction(nameof(GetById), new { id = created.voucher_id }, created);
        }

        // PUT: api/voucher/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Voucher voucher)
        {
            if (id != voucher.voucher_id) return BadRequest();
            var result = await _voucherRepository.UpdateVoucherAsync(voucher);
            if (!result) return NotFound();
            return Ok(new { voucher_id = id });
        }

        // DELETE: api/voucher/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _voucherRepository.DeleteVoucherAsync(id);
            if (!result) return NotFound();
            return Ok(new { voucher_id = id });
        }
    }
}