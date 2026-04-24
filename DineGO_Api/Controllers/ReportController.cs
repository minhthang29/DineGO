using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using DineGO_Api.Repository;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repo;
        public ReportController(IReportRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<List<Report>>> GetAll()
        {
            var reports = await _repo.GetAllAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetById(int id)
        {
            var report = await _repo.GetByIdAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        [HttpPost]
        public async Task<ActionResult<Report>> Create([FromBody] Report report)
        {
            var created = await _repo.CreateAsync(report);
            return CreatedAtAction(nameof(GetById), new { id = created.report_id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Report report)
        {
            if (id != report.report_id) return BadRequest();
            var success = await _repo.UpdateAsync(report);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}