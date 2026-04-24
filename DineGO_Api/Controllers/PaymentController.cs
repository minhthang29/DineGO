using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for handling payment operations such as creation, retrieval, update, and deletion.
    /// </summary>
    /// <author>Sieuhdd</author>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepositoy;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor that injects the payment repository for business logic operations.
        /// </summary>
        public PaymentController(IPaymentRepository paymentRepositoy, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _paymentRepositoy = paymentRepositoy;
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Retrieves all payment records.
        /// </summary>
        /// <returns>List of all payments.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_paymentRepositoy.GetPayments());
        }

        /// <summary>
        /// Retrieves a specific payment by its ID.
        /// </summary>
        /// <param name="ID">The ID of the payment.</param>
        /// <returns>The payment that matches the specified ID.</returns>
        [HttpGet("id")]
        public IActionResult GetOne(int ID)
        {
            return Ok(_paymentRepositoy.FindPaymentById(ID));
        }

        /// <summary>
        /// Adds a new payment record.
        /// </summary>
        /// <param name="p">The payment object to add.</param>
        /// <returns>List of all payments after insertion.</returns>
        [HttpPost]
        public IActionResult AddPayment(Payment p)
        {
            _paymentRepositoy.SavePayment(p);
            return Ok(_paymentRepositoy.GetPayments());
        }

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        /// <param name="p">The updated payment object.</param>
        /// <returns>List of all payments after update.</returns>
        [HttpPut]
        public IActionResult UpdatePayment(Payment p)
        {
            _paymentRepositoy.UpdatePayment(p);
            return Ok(_paymentRepositoy.GetPayments());
        }

        /// <summary>
        /// Deletes a payment by its ID.
        /// </summary>
        /// <param name="Id">The ID of the payment to delete.</param>
        /// <returns>List of all payments after deletion.</returns>
        [HttpDelete]
        public IActionResult DeletePayment(int Id)
        {
            _paymentRepositoy.DeletePayment(Id);
            return Ok(_paymentRepositoy.GetPayments());
        }

        /// <summary>
        /// Retrieves a list of payments made by a specific customer.
        /// </summary>
        /// <param name="cus_id">The ID of the customer.</param>
        /// <returns>List of payments for the given customer ID.</returns>
        [HttpGet("cus_id")]
        public IActionResult GetPaymentsByCustomer(int cus_id)
        {
            var payments = _paymentRepositoy.GetByCusId(cus_id);
            return Ok(payments ?? new List<Payment>());
        }

        [HttpGet("GetTransactions")]
        public async Task<IActionResult> GetTransactions()
        {
            string apiKey = _config["Casso:ApiKey"]; 
            var request = new HttpRequestMessage(HttpMethod.Get, "https://oauth.casso.vn/v2/transactions?sort=DESC");
            request.Headers.Add("Authorization", "apikey " + apiKey);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            return Content(content, "application/json");
        }
    }
}
