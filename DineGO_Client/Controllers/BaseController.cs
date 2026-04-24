using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core.Services;
using Core.Models;
using Core.Constant;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace DineGO_Client.Controllers
{
    public class BaseController : Controller
    {
        protected RestaurantService _restaurantService;
        protected int? resId;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var actionName = context.ActionDescriptor.RouteValues["action"];
            if (actionName == "GetTables")
                return; // Bỏ qua kiểm tra session cho GetTables

            // Lấy từ service provider
            _restaurantService = HttpContext.RequestServices.GetService<RestaurantService>();

            resId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID);

            if (!resId.HasValue)
            {
                // Nếu chưa có resId -> chuyển hướng về trang Login
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }
            // Lấy restaurant từ service (đồng bộ hóa để gán ViewBag)
            var restaurant = _restaurantService.GetRestaurantByID(resId.Value).Result;
            ViewBag.Restaurant = restaurant;
        }
    }
}
