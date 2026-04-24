using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Models.CartItemModel;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Handles cart-related API endpoints such as viewing, modifying,
    /// deleting, and processing cart items into orders.
    /// </summary>
    /// <author>Sieuhdd,KhoiNV</author>
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        /// Constructor that injects cart and order repositories.
        /// </summary>
        public CartController(ICartRepository cartRepository, IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Retrieves all cart items for a specific customer,
        /// grouped by restaurant.
        /// </summary>
        /// <param name="cusId">Customer ID.</param>
        /// <returns>List of cart items grouped by restaurant.</returns>
        [HttpGet("cusId")]
        public IActionResult GetCart(int cusId)
        {
            var items = _cartRepository.GetGroupedCartByCustomer(cusId);
            return Ok(items);
        }

        /// <summary>
        /// Deletes a specific cart item based on its CartFood ID.
        /// </summary>
        /// <param name="cartFoodId">ID of the cart food item to delete.</param>
        /// <returns>Success status.</returns>
        [HttpDelete("cartFoodId")]
        public IActionResult DeleteCartItem(int cartFoodId)
        {
            _cartRepository.DeleteCartItem(cartFoodId);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Updates the quantity of a specific cart item.
        /// </summary>
        /// <param name="request">Object containing CartFoodId and new quantity.</param>
        /// <returns>Success status or 404 if item not found.</returns>
        [HttpPut]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var updated = _cartRepository.UpdateQuantity(request.CartFoodId, request.Quantity);
            if (!updated) return NotFound();
            return Ok(new { success = true });
        }

        /// <summary>
        /// Marks selected cart items as "bought" (is_buy = true)
        /// and creates or updates orders based on grouped cart data.
        /// </summary>
        /// <param name="cartFoodIds">List of selected cart food IDs.</param>
        /// <returns>Success if updated and order created successfully, otherwise error.</returns>
        [HttpPut("mark-bought")]
        public IActionResult MarkAsBought([FromBody] MarkBoughtRequest request)
        {
            var success = _cartRepository.UpdateIsBuy(request.CartFoodIds);
            if (!success) return NotFound(new { error = "CartFood items not found to update." });

            var orderResult = _orderRepository.CreateOrUpdateOrderFromCart(request.CartFoodIds, request.VoucherCode);
            if (!orderResult) return BadRequest(new { error = "Failed to process cart into order." });

            return Ok(new { success = true });
        }
        public class MarkBoughtRequest
        {
            public List<int> CartFoodIds { get; set; }
            public string VoucherCode { get; set; }
        }

        /// <summary>
        /// Adds a food item to the customer's cart.
        /// </summary>
        /// <param name="cusId">Customer ID.</param>
        /// <param name="foodId">Food ID to add.</param>
        /// <param name="quantity">Quantity of the food (default is 1).</param>
        /// <returns>A success message or an error response.</returns>
        /// <response code="200">Food added successfully.</response>
        /// <response code="400">An error occurred during the operation.</response>
        /// <author>KhoiNV</author>
        [HttpPost("add")]
        public IActionResult AddFoodToCart(int cusId, int foodId, int quantity = 1)
        {
            try
            {
                _cartRepository.AddFoodToCart(cusId, foodId, quantity);
                return Ok(new { message = "Added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets checkout information including customer details and latest order information.
        /// </summary>
        /// <param name="customerId">Customer ID.</param>
        /// <param name="quantity">Quantity of the food (default is 1).</param>
        /// <returns>A success message or an error response.</returns>
        /// <response code="200">Checkout info retrieved successfully.</response>
        /// <response code="400">An error occurred during the operation.</response>
        /// <author>ThanhDT</author>
        [HttpGet("checkout/{customerId}")]
        public IActionResult GetCheckOutInfo(int customerId, [FromQuery] string selectedIds)
        {
            try
            {
                var checkoutInfo = _cartRepository.GetCheckOutInfo(customerId, selectedIds);
                return Ok(checkoutInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        /// <summary>
        /// Xóa các cart items đã được chọn để checkout
        /// </summary>
        /// <param name="request">Danh sách CartFood IDs cần xóa</param>
        /// <returns>Success status</returns>
        [HttpPost("clear-selected")]
        public IActionResult ClearSelectedCartItems([FromBody] ClearSelectedRequest request)
        {
            try
            {
                var success = _cartRepository.ClearSelectedCartItems(request.CartFoodIds);
                if (!success) 
                    return NotFound(new { error = "Không tìm thấy cart items để xóa." });
                    
                return Ok(new { 
                    success = true, 
                    message = $"Đã xóa {request.CartFoodIds.Count} items khỏi giỏ hàng." 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public class ClearSelectedRequest
        {
            public List<int> CartFoodIds { get; set; } = new();
        }
    }
}
