using DineGO_Api.Data;
using Core.Models;
using System.Collections.Generic;

namespace DineGO_Api.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MenuDAO _menuDAO;

        public MenuRepository(MenuDAO menuDAO)
        {
            _menuDAO = menuDAO;
        }

        public List<Menu> GetMenus() => _menuDAO.GetMenus();

        public List<Menu> GetMenusByRestaurantId(int resId) => _menuDAO.GetMenusByRestaurantId(resId);

        public Menu FindMenuById(int id) => _menuDAO.FindMenuById(id);

        public void SaveMenu(Menu menu) => _menuDAO.SaveMenu(menu);

        public void UpdateMenu(Menu menu) => _menuDAO.UpdateMenu(menu);

        public void DeleteMenu(int id) => _menuDAO.DeleteMenu(id);
    }
}
