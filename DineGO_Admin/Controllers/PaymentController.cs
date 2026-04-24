using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using Core.Services;
using Core.Constant;

namespace DineGO_Admin.Controllers
{
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly PaymentService _paymentService;

        public PaymentController(ILogger<PaymentController> logger, PaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments);
        }

        [HttpGet]
        [Route("PrintInvoice/{id}")]
        public async Task<IActionResult> PrintInvoice(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound();

            // Truyền payment sang view in hóa đơn
            return View("Invoice", payment);
        }
    }
}