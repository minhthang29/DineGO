using Core.Common;
using Core.Constant;
using Core.Models;
using Core.Services;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace Core.Services
{
    /// <summary>
    /// Handles cart-related logic for the client side,
    /// including add, update, delete, view, and checkout cart items.
    /// </summary>
    /// <author>KhoiNV,SieuHdd</author>
    public class CartService
    {
        private readonly ApiService _apiService;

        /// <summary>
        /// Constructor for injecting API service dependency.
        /// </summary>
        /// <param name="apiService">The API service used for HTTP requests.</param>
        public CartService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Adds a specific food item to the customer's cart.
        /// </summary>
        /// <param name="cusId">Customer ID.</param>
        /// <param name="foodId">Food ID to be added.</param>
        /// <param name="quantity">Quantity of the food item.</param>
        /// <returns>Returns true if added successfully; otherwise false.</returns>
        public async Task<bool> AddFoodToCartAsync(int cusId, int foodId, int quantity)
        {
            var url = $"{ApiEndpoints.CART}/add?cusId={cusId}&foodId={foodId}&quantity={quantity}";
            var response = await _apiService.PostAsync<object, object>(url, null);
            return response != null;
        }

        /// <summary>
        /// Retrieves the list of cart items grouped by restaurant for a customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>List of cart items grouped by restaurant.</returns>
        public async Task<List<CartItemViewModel>> GetCartItems(int customerId)
        {
            var url = $"{ApiEndpoints.CART_BY_CUSID}{customerId}";
            return await _apiService.GetAsync<List<CartItemViewModel>>(url);
        }

        /// <summary>
        /// Deletes a specific item from the cart using its CartFood ID.
        /// </summary>
        /// <param name="cartFoodId">The ID of the cart food item to be removed.</param>
        public async Task DeleteCartItem(int cartFoodId)
        {
            var url = $"{ApiEndpoints.CART_DELETE}{cartFoodId}";
            await _apiService.DeleteAsync<dynamic>(url);
        }

        /// <summary>
        /// Updates the quantity of a specific cart item.
        /// </summary>
        /// <param name="cartFoodId">The ID of the cart food item.</param>
        /// <param name="quantity">The new quantity to be set.</param>
        public async Task UpdateQuantity(int cartFoodId, int quantity)
        {
            var url = ApiEndpoints.CART_UPDATE_QUANTITY;
            var payload = new { CartFoodId = cartFoodId, Quantity = quantity };
            await _apiService.PutAsync<object, dynamic>(url, payload);
        }

        /// <summary>
        /// Counts the total quantity of all food items in a customer's cart.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>Total quantity of all items in the cart.</returns>
        public async Task<int> CountCartItemsAsync(int customerId)
        {
            var url = $"{ApiEndpoints.CART_BY_CUSID}{customerId}";
            var cartItems = await _apiService.GetAsync<List<CartItemViewModel>>(url);

            // Total quantity of all items in the cart
            return cartItems.SelectMany(g => g.Items).Sum(i => i.Quantity);
        }

        /// <summary>
        /// Marks a list of CartFood items as "bought" (is_buy = true) before checkout.
        /// </summary>
        /// <param name="cartFoodIds">List of CartFood IDs to be marked as bought.</param>
        /// <returns>True if marked successfully; otherwise false.</returns>
        public async Task<bool> MarkAsBoughtAsync(List<int> cartFoodIds, string SelectedVoucherCode)
        {
            var payload = new
            {
                CartFoodIds = cartFoodIds,
                VoucherCode = SelectedVoucherCode
            };
            var response = await _apiService.PutAsync<HttpResponseMessage, object>(ApiEndpoints.CART_MARK, payload);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets checkout information including customer details and order information.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>CheckOutViewModel containing customer and order information.</returns>
        public async Task<CheckOutViewModel> GetCheckOutInfoAsync(int customerId, List<int> SelectedCartFoodIds)
        {
            return await _apiService.GetAsync<CheckOutViewModel>($"{ApiEndpoints.CART_CHECKOUT}/{customerId}?selectedIds={string.Join(",", SelectedCartFoodIds)}");
        }
        /// <summary>
        /// Tạo đơn hàng với chi tiết và thông tin cố định
        /// </summary>
        public async Task<bool> CreateOrderWithDetailsAsync(object orderData)
        {
            try
            {
                var response = await _apiService.PostAsync<object, object>(ApiEndpoints.ORDER_CREATE_WITH_DETAILS, orderData);
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xóa các item đã checkout khỏi giỏ hàng
        /// </summary>
        public async Task ClearSelectedCartItemsAsync(List<int> cartFoodIds)
        {
            var payload = new { CartFoodIds = cartFoodIds };
            await _apiService.PostAsync<object, object>(ApiEndpoints.CART_CLEAR_SELECTED, payload);
        }
    }
}
