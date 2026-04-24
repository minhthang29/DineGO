using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface ICustomerRepository
    {
        public Customer IsMailExist(string email);

        public Customer ChangPassword(string email, string newpassword);

        List<Customer> GetCustomers();

        Customer FindCustomerById(int ID);

        void SaveCustomer(Customer customer);

        void UpdateCustomer(Customer customer);

        void DeleteCustomer(int customerId);
        void BlockCustomer(int customerId);

    }
}