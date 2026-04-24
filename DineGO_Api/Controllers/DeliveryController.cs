using Microsoft.AspNetCore.Mvc;
using DineGO_Api.Repository;
using Core.Models;
using Core.Models.Client.Custom;
using System.Linq;
using System;
using System.Collections.Generic;

namespace DineGO_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryController(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        [HttpGet("tracking/{customerId}")]
        public IActionResult GetDeliveryTracking(int customerId)
        {
            try
            {
                var deliveries = _deliveryRepository.GetDeliveriesByCustomerId(customerId);
                var trackingData = deliveries.Select(d => new DeliveryTrackingViewModel
                {
                    OrderId = d.order_id,
                    RestaurantName = d.order?.restaurant?.res_name ?? "Unknown Restaurant",
                    OrderDate = d.order?.order_date ?? DateTime.Now,
                    OrderTotal = d.order?.order_total ?? 0,
                    DeliveryStatus = d.de_status,
                    StatusText = GetStatusText(d.de_status),
                    StatusClass = GetStatusClass(d.de_status),
                    OrderItems = d.order?.orderDetails?.Select(od => 
                    {
                        var cartFood = od.cart?.cartFoods?.FirstOrDefault();
                        return new OrderItemViewModel
                        {
                            FoodName = cartFood?.food?.food_name ?? "Unknown Food",
                            FoodImage = cartFood?.food?.food_image ?? "",
                            Quantity = od.order_quantity,
                            Price = od.order_price,
                            TotalPrice = od.order_quantity * od.order_price
                        };
                    }).ToList() ?? new List<OrderItemViewModel>()
                }).ToList();

                return Ok(trackingData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Chờ xác nhận",
                1 => "Đang chế biến",
                2 => "Đang giao",
                3 => "Đã giao",
                4 => "Đã hủy",
                _ => "Không xác định"
            };
        }

        private string GetStatusClass(int status)
        {
            return status switch
            {
                0 => "status-pending",
                1 => "status-preparing",
                2 => "status-delivering",
                3 => "status-delivered",
                4 => "status-cancelled",
                _ => "status-unknown"
            };
        }

      
    }
} 