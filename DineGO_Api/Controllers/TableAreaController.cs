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
    /// Controller for handling operations related to tableareas such as retrieval, creation, update, deletion
    /// </summary>
    /// <author>Thangtm</author>
    [ApiController]
    [Route("api/[controller]")]
    public class TableAreaController : ControllerBase
    {
        private readonly ITableAreaRepository _tableareasRepository;

        /// <summary>
        /// Constructor that injects the tablearea repository for handling tablearea data.
        /// </summary>
        public TableAreaController(ITableAreaRepository tableareasRepository)
        {
            _tableareasRepository = tableareasRepository;
        }

        /// <summary>
        /// Retrieves a list of all tableareas.
        /// </summary>
        /// <returns>List of tableareas.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_tableareasRepository.GetTableAreas());
        }

        /// <summary>
        /// Retrieves a tablearea by its ID.
        /// </summary>
        /// <param name="id">The ID of the tablearea.</param>
        /// <returns>TableArea details or 404 if not found.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int id)
        {
            var tablearea = _tableareasRepository.FindTableAreaById(id);
            if (tablearea == null)
            {
                return NotFound();
            }
            return Ok(tablearea);
        }

        /// <summary>
        /// Retrieves a tablearea by tablearea owner ID.
        /// </summary>
        /// <param name="id">The ID of the tablearea.</param>
        /// <returns>TableArea details or 404 if not found.</returns>
        [HttpGet("res_id")]
        public IActionResult GetOneByTableAreaOwner(int res_id)
        {
            var tablearea = _tableareasRepository.FindTableAreaByResId(res_id);
            if (tablearea == null)
            {
                return NotFound();
            }
            return Ok(tablearea);
        }

        /// <summary>
        /// Adds a new tablearea.
        /// </summary>
        /// <param name="p">The tablearea object to add.</param>
        /// <returns>Object containing the new tablearea ID.</returns>
        [HttpPost]
        public IActionResult AddTableAreas(TableArea p)
        {
            _tableareasRepository.SaveTableArea(p);
            return Ok(new { res_id = p.res_id });
        }

        /// <summary>
        /// Updates tablearea information.
        /// </summary>
        /// <param name="p">The tablearea object with updated data.</param>
        /// <returns>List of all tableareas after update.</returns>
        [HttpPut]
        public IActionResult UpdateTableAreas(TableArea p)
        {
            _tableareasRepository.UpdateTableArea(p);
            return Ok(p);
        }

        /// <summary>
        /// Deletes a tablearea by its ID.
        /// </summary>
        /// <param name="Id">The ID of the tablearea to delete.</param>
        /// <returns>List of tableareas after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteTableAreas(int Id)
        {
            _tableareasRepository.DeleteTableArea(Id);
            return Ok(_tableareasRepository.GetTableAreas());
        }
    }
}
