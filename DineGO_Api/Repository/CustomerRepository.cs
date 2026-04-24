using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace DineGO_Api.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDAO _customerDAO;

        public CustomerRepository(CustomerDAO customerDAO)
        {
            _customerDAO = customerDAO;
        }

        public Customer ChangPassword(string email, string newPassword)
        {
            return _customerDAO.ChangePassword(email, newPassword);
        }

        public Customer IsMailExist(string email)
        {
            return _customerDAO.GetCustomers().FirstOrDefault(c => c.cus_email == email);


        }
        public List<Customer> GetCustomers() => _customerDAO.GetCustomers();
        public Customer FindCustomerById(int id) => _customerDAO.FindCustomerById(id);
        public void SaveCustomer(Customer c) => _customerDAO.SaveCustomer(c);
        public void UpdateCustomer(Customer c) => _customerDAO.UpdateCustomer(c);
        public void DeleteCustomer(int id) => _customerDAO.DeleteCustomer(id);
        public void BlockCustomer(int id) => _customerDAO.BlockCustomer(id);
    }
}