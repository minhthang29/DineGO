using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class TableDAO
    {
        private readonly ApplicationDbContext _context;

        public TableDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Tables
        public List<Table> GetTables()
        {
            try
            {
                return _context.Tables.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Tables: {e.Message}");
            }
        }

        public Table FindTableById(int id)
        {
            try
            {
                 return _context.Tables
            .Include(t => t.Restaurant) // Include Restaurant
            .SingleOrDefault(x => x.table_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding table: {e.Message}");
            }
        }

        // Get table by ID
        public List<Table> FindTableByResId(int resId)
        {
            try
            {

                return _context.Tables.Where(x => x.res_id == resId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding table: {e.Message}");
            }
        }

        // Save a new table
        public void SaveTable(Table table)
        {
            try
            {
                _context.Tables.Add(table);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving table: {e.Message}");
            }
        }

        // Update table details
        public void UpdateTable(Table table)
        {
            try
            {
                // Tìm bản ghi hiện có trong DB
                var existing = _context.Tables.FirstOrDefault(t => t.table_id == table.table_id);
                if (existing == null) throw new Exception("Table not found");

                // Chỉ cập nhật 3 field
                existing.table_name = table.table_name;
                existing.table_image = table.table_image;
                existing.table_seat= table.table_seat;
                existing.table_update_at = DateTime.Now;

                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating table: {e.Message}");
            }
        }


        // Delete table by ID
        public void DeleteTable(int id)
        {
            try
            {
                var table = _context.Tables.SingleOrDefault(x => x.table_id == id);
                if (table != null)
                {
                    table.table_is_deleted = true; // Soft delete: mark as inactive
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting table: {e.Message}");
            }
        }

    }
}
