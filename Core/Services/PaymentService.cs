using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Constant;

namespace Core.Services
{
    public class PaymentService
    {
        private readonly ApiService _apiService;

        public PaymentService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _apiService.GetAsync<List<Payment>>(ApiEndpoints.PAYMENT);
        }
        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return await _apiService.GetAsync<Payment>($"{ApiEndpoints.PAYMENT}/id?ID={id}");
        }
    }
}