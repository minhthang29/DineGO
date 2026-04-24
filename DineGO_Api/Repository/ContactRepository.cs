using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Data;

namespace DineGO_Api.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly ContactDAO _contactDao;
        public ContactRepository(ContactDAO contactDao) => _contactDao = contactDao;

        public Task<List<Contact>> GetAllAsync() => _contactDao.GetAllAsync();
        public Task<Contact?> GetByIdAsync(int id) => _contactDao.GetByIdAsync(id);
        public Task<Contact> CreateAsync(Contact contact) => _contactDao.CreateAsync(contact);
        public Task<bool> UpdateAsync(Contact contact) => _contactDao.UpdateAsync(contact);
        public Task<bool> DeleteAsync(int id) => _contactDao.DeleteAsync(id);
    }
}