using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Core.Models.Client;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _ordersRepository;

        /// <summary>
        /// Constructor that injects the Order repository for data operations.
        /// </summary>
        public OrderController(IOrderRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        /// <summary>
        /// Retrieves a list of all Orders.
        /// </summary>
        /// <returns>List of Orders.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_ordersRepository.GetOrders());
        }

        /// <summary>
        /// Retrieves a specific Order by its ID.
        /// </summary>
        /// <param name="ID">The ID of the Order.</param>
        /// <returns>Order matching the specified ID.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int ID)
        {
            return Ok(_ordersRepository.GetOrderById(ID));
        }

        /// <summary>
        /// Retrieves orders by customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>List of Orders for the customer.</returns>
        [HttpGet("customer/{customerId}")]
        public IActionResult GetByCustomer(int customerId)
        {
            return Ok(_ordersRepository.GetOrdersByCustomerId(customerId));
        }

        [HttpGet("details/{orderId}")]
        public IActionResult GetOrderDetails(int orderId)
        {
            return Ok(_ordersRepository.GetOrderDetailsByOrderId(orderId));
        }

        /// <summary>
        /// Updates an existing Order.
        /// </summary>
        /// <param name="p">The updated Order object.</param>
        /// <returns>Updated list of Orders.</returns>
        [HttpPut]
        public IActionResult UpdateOrders(Order p)
        {
            _ordersRepository.UpdateOrder(p);
            return Ok(_ordersRepository.GetOrders());
        }

        /// <summary>
        /// Deletes a Order by its ID.
        /// </summary>
        /// <param name="Id">The ID of the Order to be deleted.</param>
        /// <returns>Updated list of Orders after deletion.</returns>
        [HttpDelete]
        public IActionResult DeleteOrders(int Id)
        {
            _ordersRepository.DeleteOrder(Id);
            return Ok(_ordersRepository.GetOrders());
        }

        [HttpGet("restaurant/{resId}")]
        public IActionResult GetByRestaurant(int resId)
        {
            return Ok(_ordersRepository.GetOrdersByRestaurantId(resId));
        }

        /// <summary>
        /// Tạo đơn hàng với thông tin chi tiết và cố định
        /// </summary>
        /// <param name="orderData">Dữ liệu đơn hàng bao gồm thông tin cố định</param>
        /// <returns>Thông tin đơn hàng đã tạo</returns>
        [HttpPost("create-with-details")]
        public IActionResult CreateOrderWithDetails([FromBody] CreateOrderWithDetailsRequest orderData)
        {
            try
            {
                var createdOrder = _ordersRepository.CreateOrderWithDetails(orderData);
                if (createdOrder == null)
                    return BadRequest(new { error = "Không thể tạo đơn hàng." });

                return Ok(new
                {
                    success = true,
                    orderId = createdOrder.order_id,
                    message = "Đơn hàng đã được tạo thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}