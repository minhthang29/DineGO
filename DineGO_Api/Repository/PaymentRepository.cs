using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;
        public PaymentRepository(PaymentDAO paymentDAO)
        {
            _paymentDAO = paymentDAO;
        }

        public void DeletePayment(int Payment) => _paymentDAO.DeletePayment(Payment);
        public Payment FindPaymentById(int ID) => _paymentDAO.FindPaymentById(ID);

        public List<Payment> GetByCusId(int ID)
        {
            return GetPayments().Where(p => p.cus_id == ID).ToList();
        }
        public List<Payment> GetPayments() => _paymentDAO.GetPayments();
        public void SavePayment(Payment Payment) => _paymentDAO.SavePayment(Payment);
        public void UpdatePayment(Payment Payment) => _paymentDAO.UpdatePayment(Payment);
    }
}