using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Constant;

namespace Core.Services
{
    public class CategoryService
    {
        private readonly ApiService _apiService;

        public CategoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _apiService.GetAsync<List<Category>>(ApiEndpoints.CATEGORY);
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _apiService.GetAsync<Category>($"{ApiEndpoints.CATEGORY}/id?ID={id}");
        }

        public async Task AddAsync(Category category)
        {
            var addData = new
            {
                cate_id = category.cate_id,
                cate_type = category.cate_type,
                cate_description = category.cate_description
            };
            await _apiService.PostAsync<object, dynamic>($"{ApiEndpoints.CATEGORY}", addData);
        }

        public async Task UpdateAsync(Category category)
        {
            var updateData = new
            {
                cate_id = category.cate_id,
                cate_type = category.cate_type,
                cate_description = category.cate_description
            };
            await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.CATEGORY}", updateData);
        }

        public async Task DeleteAsync(int id)
        {
            await _apiService.DeleteAsync<dynamic>($"{ApiEndpoints.CATEGORY}?Id={id}");
        }
    }
}