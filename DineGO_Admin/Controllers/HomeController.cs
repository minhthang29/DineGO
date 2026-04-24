using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Constant;
using Core.Models;
using Core.Models.AdminModel;
using Core.Services;
using System.Diagnostics;
using System.Threading.Tasks;
namespace DineGO_Admin.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;
        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("api/admin/update-avatar")]
        [HttpPost]
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarRequest admin)
        {

            admin.ad_id = HttpContext.Session.GetInt32("ad_id") ?? 0;
            var response = await _apiService.PutAsync<object, dynamic>("Admin/update", admin);
            HttpContext.Session.SetString("ad_image", admin.ad_image);
            return Ok();
        }
        /// <summary>
        /// Displays the error page with the error message.
        /// </summary>
        /// <returns>Returns the error view with the error details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorMessage = HttpContext.Session.GetString(KeyConstants.ERROR_MESSAGE);
            var model = new ErrorViewModel
            {
                ErrorMessage = errorMessage,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(model);
        }
    }
}