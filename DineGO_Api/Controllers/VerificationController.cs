using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DineGO_Api.Repository;
using Core.Models;
using Microsoft.AspNetCore.Http;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationRepository _verificationRepository;

        public VerificationController(IVerificationRepository verificationRepository)
        {
            _verificationRepository = verificationRepository;
        }

        // GET: api/verification
        [HttpGet]
        public async Task<ActionResult<List<Verification>>> GetAll()
        {
            var verifications = await _verificationRepository.GetAllVerificationsAsync();
            return Ok(verifications);
        }

        // GET: api/verification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Verification>> GetById(int id)
        {
            var verification = await _verificationRepository.GetVerificationByIdAsync(id);
            if (verification == null) return NotFound();
            return Ok(verification);
        }

        // POST: api/verification
        [HttpPost]
        public async Task<ActionResult<Verification>> Create([FromBody] Verification verification)
        {
            // Xử lý file ở đây
            var created = await _verificationRepository.AddVerificationAsync(verification);
            return CreatedAtAction(nameof(GetById), new { id = created.ver_id }, created);
        }

        // PUT: api/verification/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Verification verification)
        {
            if (id != verification.ver_id) return BadRequest();
            var result = await _verificationRepository.UpdateVerificationAsync(verification);
            if (!result) return NotFound();
            return Ok(new { ver_id = verification.ver_id });
        }

        // DELETE: api/verification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _verificationRepository.DeleteVerificationAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // GET: api/verification/res_id
        [HttpGet("res_id")]
        public async Task<ActionResult<List<Verification>>> GetByResId([FromQuery] int res_id)
        {
            var verifications = await _verificationRepository.GetVerificationsByRestaurantIdAsync(res_id);
            return Ok(verifications);
        }
    }
}