using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class PaymentDAO
    {
         private readonly ApplicationDbContext _context;

        public PaymentDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Payments
        public List<Payment> GetPayments()
        {
            try
            {
                return _context.Payments.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Payments: {e.Message}");
            }
        }

        // Get Payment by ID
        public Payment FindPaymentById(int id)
        {
            try
            {
                return _context.Payments.SingleOrDefault(x => x.pay_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Payment: {e.Message}");
            }
        }

        // Save a new Payment
        public void SavePayment(Payment Payment)
        {
            try
            {
                _context.Payments.Add(Payment);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Payment: {e.Message}");
            }
        }

        // Update Payment details
        public void UpdatePayment(Payment Payment)
        {
            try
            {
                _context.Entry(Payment).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating Payment: {e.Message}");
            }
        }

        // Delete Payment by ID
        public void DeletePayment(int id)
        {
            try
            {
                var Payment = _context.Payments.SingleOrDefault(x => x.pay_id == id);
                if (Payment != null)
                {
                    _context.Payments.Remove(Payment);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Payment: {e.Message}");
            }
        }
    }
}