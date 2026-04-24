using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Models.Client.Custom;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for managing customer-related operations such as retrieving, creating, updating, and deleting customer data.
    /// </summary>
    /// <author>thangtm</author>
    [Authorize]// Requires JWT token to access the APIs in this controller
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Constructor that injects the customer repository for handling customer data.
        /// </summary>
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Retrieves the list of all customers.
        /// </summary>
        /// <returns>List of customers.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_customerRepository.GetCustomers());
        }

        /// <summary>
        /// Retrieves a specific customer by ID.
        /// </summary>
        /// <param name="id">Customer ID.</param>
        /// <returns>Customer data if found; otherwise 404 Not Found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var customer = _customerRepository.FindCustomerById(id);
            if (customer == null)
                return NotFound(string.Format(NotificationConstants.CUSTOMER_WITH_ID_NOT_FOUND, id));

            return Ok(customer);
        }

        /// <summary>
        /// Adds a new customer.
        /// </summary>
        /// <param name="customer">Customer object to be created.</param>
        /// <returns>201 Created with location of the new customer.</returns>
        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            // Check duplicate email
            var existingEmail = _customerRepository.GetCustomers().Any(c => c.cus_email == customer.cus_email);
            if (existingEmail)
                return BadRequest(NotificationConstants.EMAIL_ALREADY_EXISTS);

            // Check duplicate username
            var existingUsername = _customerRepository.GetCustomers().Any(c => c.cus_username == customer.cus_username);
            if (existingUsername)
                return BadRequest(NotificationConstants.USERNAME_ALREADY_EXISTS);

            _customerRepository.SaveCustomer(customer);
            return CreatedAtAction(nameof(GetOne), new { id = customer.cus_id }, customer);
        }

        /// <summary>
        /// Updates an existing customer's information.
        /// </summary>
        /// <param name="id">ID of the customer to update.</param>
        /// <param name="customer">Updated customer data.</param>
        /// <returns>200 OK if update is successful; 400 if ID mismatch.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] CustomerUpdateProfileViewModel customer)
        {
            if (id != customer.cus_id)
                return BadRequest(NotificationConstants.CUSTOMER_ID_MISMATCH);
            Customer checkCustomer = _customerRepository.FindCustomerById(id);
            Customer newCustomer = new Customer
            {
                cus_id = customer.cus_id,
                cus_name = customer.cus_name,
                cus_phone = customer.cus_phone,
                cus_email = customer.cus_email,
                cus_address = customer.cus_address,
                cus_gender = customer.cus_gender,
                cus_image = customer.cus_image,
                cus_birthday = customer.cus_birthday,
                cus_is_use = checkCustomer.cus_is_use,
                cus_latitude = customer.cus_latitude,
                cus_longitude = customer.cus_longitude,
                cus_password = !string.IsNullOrWhiteSpace(customer.cus_password) 
                    ? customer.cus_password  // Có password mới (đã được hash từ client)
                    : checkCustomer.cus_password // Giữ nguyên password cũ
            };
            _customerRepository.UpdateCustomer(newCustomer);
            return Ok(new { message = NotificationConstants.CUSTOMER_UPDATE_SUCCESS });
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// </summary>
        /// <param name="id">ID of the customer to delete.</param>
        /// <returns>204 No Content if deletion is successful.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _customerRepository.DeleteCustomer(id);
            return Ok(_customerRepository.GetCustomers());
        }

        /// <summary>
        /// Block (ngừng hoạt động) một khách hàng theo ID.
        /// </summary>
        /// <param name="id">ID của khách hàng cần block.</param>
        /// <returns>200 OK nếu thành công.</returns>
        [HttpPut("block/{id}")]
        public IActionResult BlockCustomer(int id)
        {
            var customer = _customerRepository.FindCustomerById(id);
            if (customer == null)
                return NotFound(string.Format(NotificationConstants.CUSTOMER_WITH_ID_NOT_FOUND, id));

            _customerRepository.BlockCustomer(id);
            return Ok(new { message = "Khách hàng đã bị ngừng hoạt động." });
        }
        /// <summary>
        /// Kích hoạt (active) một khách hàng theo ID.
        /// </summary>
        /// <param name="id">ID của khách hàng cần kích hoạt.</param>
        /// <returns>200 OK nếu thành công.</returns>
        [HttpPut("activate/{id}")]
        public IActionResult ActivateCustomer(int id)
        {
            var customer = _customerRepository.FindCustomerById(id);
            if (customer == null)
                return NotFound(string.Format(NotificationConstants.CUSTOMER_WITH_ID_NOT_FOUND, id));

            customer.cus_is_use = true;
            _customerRepository.UpdateCustomer(customer);
            return Ok(new { message = "Khách hàng đã được kích hoạt." });
        }
    }
}
