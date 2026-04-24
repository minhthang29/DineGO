using Core.Constant;
using Core.Services;
using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    /// <summary>
    /// Handles blog-related operations by communicating with the API.
    /// </summary>
    /// <author>KhoiNV</author>
    public class BlogService
    {
        private readonly ApiService _apiService;

        public BlogService(ApiService apiService)
        {
            _apiService = apiService;
        }



        /// <summary>
        /// Retrieves the list of all blogs from the API.
        /// </summary>
        /// <returns>List of blogs.</returns>
        /// <author>KhoiNV</author>
        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            var blogs = await _apiService.GetAsync<List<Blog>>(ApiEndpoints.BLOG);
            return blogs;
        }

        public async Task<Blog> GetByIdAsync(int id)
        {
            return await _apiService.GetAsync<Blog>($"{ApiEndpoints.BLOG_BY_ID}{id}");
        }

        public async Task AddAsync(Blog blog)
        {
            await _apiService.PostAsync<object, dynamic>(ApiEndpoints.BLOG, blog);
        }

        public async Task UpdateAsync(Blog blog)
        {
            
            await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.BLOG}", blog);
        }

        public async Task DeleteAsync(int id)
        {
            await _apiService.DeleteAsync<object>($"{ApiEndpoints.BLOG_DELETE_BY_ID}{id}");
        }
    }
}
