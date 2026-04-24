using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for handling operations related to tables such as retrieval, creation, update, deletion
    /// </summary>
    /// <author>Thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tablesRepository;

        /// <summary>
        /// Constructor that injects the table repository for handling table data.
        /// </summary>
        public TableController(ITableRepository tablesRepository)
        {
            _tablesRepository = tablesRepository;
        }

        /// <summary>
        /// Retrieves a list of all tables.
        /// </summary>
        /// <returns>List of tables.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_tablesRepository.GetTables());
        }

        /// <summary>
        /// Retrieves a table by its ID.
        /// </summary>
        /// <param name="id">The ID of the table.</param>
        /// <returns>Table details or 404 if not found.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int id)
        {
            var table = _tablesRepository.FindTableById(id);
            if (table == null)
            {
                return NotFound();
            }
            return Ok(table);
        }

        /// <summary>
        /// Retrieves a table by table owner ID.
        /// </summary>
        /// <param name="id">The ID of the table.</param>
        /// <returns>Table details or 404 if not found.</returns>
        [HttpGet("res_id")]
        public IActionResult GetOneByTableOwner(int res_id)
        {
            var table = _tablesRepository.FindTableByResId(res_id);
            if (table == null)
            {
                return NotFound();
            }
            return Ok(table);
        }

        /// <summary>
        /// Adds a new table.
        /// </summary>
        /// <param name="p">The table object to add.</param>
        /// <returns>Object containing the new table ID.</returns>
        [HttpPost]
        public IActionResult AddTables(Table p)
        {
            _tablesRepository.SaveTable(p);
            return Ok(new { res_id = p.res_id });
        }

        /// <summary>
        /// Updates table information.
        /// </summary>
        /// <param name="p">The table object with updated data.</param>
        /// <returns>List of all tables after update.</returns>
        [HttpPut]
        public IActionResult UpdateTables(Table p)
        {
            _tablesRepository.UpdateTable(p);
            return Ok(p);
        }

        /// <summary>
        /// Deletes a table by its ID.
        /// </summary>
        /// <param name="Id">The ID of the table to delete.</param>
        /// <returns>List of tables after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteTables(int Id)
        {
            _tablesRepository.DeleteTable(Id);
            return Ok(_tablesRepository.GetTables());
        }

        /// <summary>
        /// Updates only the status of a table.
        /// </summary>
        /// <param name="id">ID of the table.</param>
        /// <param name="table_status">New status value (int).</param>
        /// <returns>Updated table or error.</returns>
        [HttpPut("status/{id}")]
        public IActionResult UpdateTableStatus(int id, [FromQuery] int table_status)
        {
            var table = _tablesRepository.FindTableById(id);
            if (table == null)
                return NotFound();

            table.table_status = table_status;
            _tablesRepository.UpdateTable(table);

            return Ok(new { message = "Table status updated", new_status = table_status });
        }
    }
}
