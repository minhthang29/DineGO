using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class ContactDAO
    {
        private readonly ApplicationDbContext _context;
        public ContactDAO(ApplicationDbContext context) => _context = context;

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts
                .Where(c => !c.contact_is_deleted)
                .ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts
                .FirstOrDefaultAsync(c => c.contact_id == id && !c.contact_is_deleted);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            contact.contact_created_at = DateTime.Now;
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<bool> UpdateAsync(Contact contact)
        {
            var existing = await _context.Contacts.FindAsync(contact.contact_id);
            if (existing == null || existing.contact_is_deleted) return false;

            _context.Entry(existing).CurrentValues.SetValues(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null || contact.contact_is_deleted) return false;

            contact.contact_is_deleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}