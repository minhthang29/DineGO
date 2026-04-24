using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class MenuDAO
    {
        private readonly ApplicationDbContext _context;

        public MenuDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Menu> GetMenus()
        {
            return _context.Menus
                .Where(m => !m.menu_is_deleted)
                .ToList();
        }

        public List<Menu> GetMenusByRestaurantId(int resId)
        {
            return _context.Menus
                .Where(m => m.res_id == resId && !m.menu_is_deleted)
                .ToList();
        }

        public Menu FindMenuById(int id)
        {
            return _context.Menus.SingleOrDefault(m => m.menu_id == id);
        }

        public void SaveMenu(Menu menu)
        {
            _context.Menus.Add(menu);
            _context.SaveChanges();
        }

        public void UpdateMenu(Menu menu)
        {
            _context.Entry(menu).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteMenu(int id)
        {
            var menu = _context.Menus.SingleOrDefault(m => m.menu_id == id);
            if (menu != null)
            {
                menu.menu_is_deleted = true;
                _context.SaveChanges();
            }
        }
    }
}
