using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Admin.Controllers
{
   
    public class ReservationController : Controller
    {
        private readonly ApiService _apiService;
        public ReservationController(ApiService apiService){
            _apiService = apiService;
        }
       
        public async Task<IActionResult> Index()
        {
            try
            {
                var reservations = await _apiService.GetAsync<List<Reservation>>(ApiEndpoints.RESERVATION);
                foreach (var r in reservations)
                {
                    try
                    {
                        var restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{r.res_id}");
                        var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{r.cus_id}");
                        r.customer = customer;
                        r.restaurant = restaurant;
                    }
                    catch
                    {
                        // Continue if failed to load related data
                    }
                }
                return View(reservations);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách đặt bàn: " + ex.Message;
                return View(new List<Reservation>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int reserId, int newStatus)
        {
            try
            {
                // Gọi API để cập nhật trạng thái
                var response = await _apiService.PutAsync<object, dynamic>(
                    $"{ApiEndpoints.RESERVATION}/status/{reserId}?reser_status={newStatus}", 
                    null
                );

                TempData["SuccessMessage"] = "Cập nhật trạng thái đặt bàn thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult AddReservation()
        {
            return View();
        }

        public IActionResult UpdateReservation()
        {
            return View();
        }

        public IActionResult DeleteReservation()
        {
            return View();
        }
    }
}