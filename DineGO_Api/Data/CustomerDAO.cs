using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class CustomerDAO
    {
        private readonly ApplicationDbContext _context;

        public CustomerDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Customers
        public List<Customer> GetCustomers()
        {
            try
            {
                return _context.Customers.Where(x => !x.cus_is_deleted).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Customers: {e.Message}");
            }
        }

        // Get Customer by ID
        public Customer FindCustomerById(int id)
        {
            try
            {
                return _context.Customers.SingleOrDefault(x => x.cus_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Customer: {e.Message}");
            }
        }

        // Save a new Customer
        public void SaveCustomer(Customer Customer)
        {
            try
            {
                _context.Customers.Add(Customer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Customer: {e.Message}");
            }
        }

        // Update Customer details
        public void UpdateCustomer(Customer customer)
        {
            var existing = _context.Customers.FirstOrDefault(c => c.cus_id == customer.cus_id);
            if (existing == null)
                throw new Exception("Customer not found.");

            existing.cus_name = customer.cus_name;
            existing.cus_password = customer.cus_password;
            existing.cus_email = customer.cus_email;
            existing.cus_phone = customer.cus_phone;
            existing.cus_address = customer.cus_address;
            existing.cus_birthday = customer.cus_birthday;
            existing.cus_gender = customer.cus_gender;
            existing.cus_image = customer.cus_image;
            existing.cus_is_kyc = customer.cus_is_kyc;
            existing.google_id = customer.google_id;
            existing.login_provider = customer.login_provider;
            existing.cus_is_use = customer.cus_is_use;
            //add latitue and longitude
            existing.cus_latitude = customer.cus_latitude;
            existing.cus_longitude = customer.cus_longitude;
            _context.SaveChanges();
        }

        // Delete Customer by ID
        public void DeleteCustomer(int id)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(x => x.cus_id == id);
                if (customer != null)
                {
                    customer.cus_is_deleted = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Customer: {e.Message}");
            }
        }

        public Customer ChangePassword(string email, string newPassword)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.cus_email == email);
                if (customer != null)
                {
                    customer.cus_password = newPassword; // Nên hash mật khẩu trước khi lưu
                    _context.SaveChanges();
                    return customer;
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception($"Error changing password: {e.Message}");
            }
        }
        public void BlockCustomer(int id)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(x => x.cus_id == id);
                if (customer != null)
                {
                    customer.cus_is_use = false;
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Customer not found.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error blocking Customer: {e.Message}");
            }
        }
    }
}