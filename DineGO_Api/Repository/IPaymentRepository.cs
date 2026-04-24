using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IPaymentRepository
    {
        List<Payment> GetPayments();

        Payment FindPaymentById(int ID);

        void SavePayment(Payment Payment);

        void UpdatePayment(Payment Payment);

        void DeletePayment(int Payment);

        List<Payment> GetByCusId(int ID);
    }

}