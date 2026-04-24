using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class TableAreaDAO
    {
        private readonly ApplicationDbContext _context;

        public TableAreaDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all TableAreas
        public List<TableArea> GetTableAreas()
        {
            try
            {
                return _context.TableAreas.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching TableAreas: {e.Message}");
            }
        }

        public TableArea FindTableAreaById(int id)
        {
            try
            {
                return _context.TableAreas.SingleOrDefault(x => x.area_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding tablearea: {e.Message}");
            }
        }

        // Get tablearea by ID
        public List<TableArea> FindTableAreaByResId(int resId)
        {
            try
            {

                return _context.TableAreas.Where(x => x.res_id == resId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding tablearea: {e.Message}");
            }
        }

        // Save a new tablearea
        public void SaveTableArea(TableArea tablearea)
        {
            try
            {
                _context.TableAreas.Add(tablearea);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving tablearea: {e.Message}");
            }
        }

        // Update tablearea details
        public void UpdateTableArea(TableArea tablearea)
        {
            try
            {
                tablearea.updated_at = DateTime.Now; // ✅ Gán thời gian cập nhật mới
                _context.Entry(tablearea).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating tablearea: {e.Message}");
            }
        }

        // Delete tablearea by ID
        public void DeleteTableArea(int id)
        {
            try
            {
                var tablearea = _context.TableAreas.SingleOrDefault(x => x.area_id == id);
                if (tablearea != null)
                {
                    tablearea.is_deleted = true; // Soft delete: mark as inactive
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting tablearea: {e.Message}");
            }
        }

    }
}
