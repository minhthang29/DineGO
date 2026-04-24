using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DineGO_Api.Repository;
using Core.Models;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemLogController : ControllerBase
    {
        private readonly ISystemLogRepository _systemLogRepository;

        public SystemLogController(ISystemLogRepository systemLogRepository)
        {
            _systemLogRepository = systemLogRepository;
        }

        // GET: api/systemlog
        [HttpGet]
        public async Task<ActionResult<List<SystemLog>>> GetAll()
        {
            try
            {
                var logs = await _systemLogRepository.GetAllAsync();
                
                // Ensure we never return null
                if (logs == null)
                {
                    logs = new List<SystemLog>();
                }

                // Load admin information for each log
                foreach (var log in logs.Where(l => l.ad_id.HasValue))
                {
                    try
                    {
                        // You might need to inject IAdminRepository or use a service here
                        // For now, we'll leave it as is since admin data might be loaded elsewhere
                    }
                    catch
                    {
                        // Continue if admin loading fails
                    }
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving system logs", error = ex.Message });
            }
        }

        // GET: api/systemlog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemLog>> GetById(int id)
        {
            try
            {
                var log = await _systemLogRepository.GetByIdAsync(id);
                if (log == null) 
                {
                    return NotFound(new { message = $"System log with ID {id} not found" });
                }
                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving system log", error = ex.Message });
            }
        }

        // POST: api/systemlog
        [HttpPost]
        public async Task<ActionResult<SystemLog>> Create([FromBody] SystemLog log)
        {
            try
            {
                if (log == null)
                {
                    return BadRequest(new { message = "System log data is required" });
                }

                // Set creation time if not provided
                if (log.log_time == null)
                {
                    log.log_time = DateTime.Now;
                }

                var created = await _systemLogRepository.AddAsync(log);
                return CreatedAtAction(nameof(GetById), new { id = created.sys_log_id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating system log", error = ex.Message });
            }
        }

        // DELETE: api/systemlog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _systemLogRepository.DeleteAsync(id);
                if (!result) 
                {
                    return NotFound(new { message = $"System log with ID {id} not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting system log", error = ex.Message });
            }
        }

        // DELETE: api/systemlog/cleanup - Clean up logs older than 6 months
        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupOldLogs()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddMonths(-6);
                var deletedCount = await _systemLogRepository.DeleteOldLogsAsync(cutoffDate);
                
                return Ok(new { 
                    message = "Cleanup completed successfully", 
                    deletedCount = deletedCount,
                    cutoffDate = cutoffDate.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error during cleanup", error = ex.Message });
            }
        }

        // GET: api/systemlog/stats - Get statistics
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            try
            {
                var logs = await _systemLogRepository.GetAllAsync();
                
                if (logs == null)
                {
                    logs = new List<SystemLog>();
                }

                var stats = new
                {
                    Total = logs.Count,
                    Last24Hours = logs.Count(l => l.log_time >= DateTime.Now.AddDays(-1)),
                    LastWeek = logs.Count(l => l.log_time >= DateTime.Now.AddDays(-7)),
                    LastMonth = logs.Count(l => l.log_time >= DateTime.Now.AddMonths(-1)),
                    SuccessRate = logs.Count > 0 ? (double)logs.Count(l => l.is_success == true) / logs.Count * 100 : 0,
                    SuccessCount = logs.Count(l => l.is_success == true),
                    FailureCount = logs.Count(l => l.is_success == false),
                    TopActions = logs.Where(l => !string.IsNullOrEmpty(l.action))
                        .GroupBy(l => l.action)
                        .Select(g => new { Action = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(5)
                        .ToList()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving statistics", error = ex.Message });
            }
        }

        // GET: api/systemlog/filter - Advanced filtering
        [HttpGet("filter")]
        public async Task<ActionResult<List<SystemLog>>> GetFiltered(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? adminId = null,
            [FromQuery] string action = null,
            [FromQuery] bool? isSuccess = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var logs = await _systemLogRepository.GetFilteredAsync(fromDate, toDate, adminId, action, isSuccess, page, pageSize);
                
                if (logs == null)
                {
                    logs = new List<SystemLog>();
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error filtering system logs", error = ex.Message });
            }
        }
    }
}