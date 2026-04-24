using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Controller responsible for handling user location-related operations.
    /// </summary>
    /// <author>Khoinv</author>
    public class LocationController : Controller
    {
        /// <summary>
        /// Stores the user's location in session.
        /// </summary>
        /// <param name="address">The user's selected address (from map or input).</param>
        /// <returns>A JSON result indicating success and the saved location.</returns>
        [HttpPost]
        public IActionResult SetUserLocation([FromBody] string address)
        {
            HttpContext.Session.SetString("USER_LOCATION", address);
            return Json(new { success = true, location = address });
        }
    }
}