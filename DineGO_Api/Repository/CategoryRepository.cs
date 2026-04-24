using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;
        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }
        public List<Category> GetCategories() => _categoryDAO.GetCategories();
        public Category FindCategoryById(int Id) => _categoryDAO.FindCategoryById(Id);
        public void SaveCategory(Category p) => _categoryDAO.SaveCategory(p);
        public void UpdateCategory(Category p) => _categoryDAO.UpdateCategory(p);
        public void DeleteCategory(int Id) => _categoryDAO.DeleteCategory(Id);

    }
}