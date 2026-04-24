using DineGO_Api.Data;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
namespace DineGO_Api.Repository
{
    public interface IMenuRepository
    {
        List<Menu> GetMenus();
        List<Menu> GetMenusByRestaurantId(int resId);
        Menu FindMenuById(int id);
        void SaveMenu(Menu menu);
        void UpdateMenu(Menu menu);
        void DeleteMenu(int id);
    }
}