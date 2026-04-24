using Core.Models;
using Core.Constant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    /// <summary>
    /// Service class that manages operations related to Menu by communicating with API endpoints.
    /// </summary>
    /// <author>KhoiNV</author>
    public class MenuService
    {
        private readonly ApiService _apiService;

        public MenuService(ApiService apiService)
        {
            _apiService = apiService;
        }
        /// <summary>
        /// Retrieves all menus from the system.
        /// </summary>
        /// <returns>List of all menus.</returns>
        public async Task<List<Menu>> GetMenusAsync()
        {
            return await _apiService.GetAsync<List<Menu>>(ApiEndpoints.MENU);
        }
        /// <summary>
        /// Retrieves menus by a specific restaurant ID.
        /// </summary>
        /// <param name="resId">The ID of the restaurant.</param>
        /// <returns>List of menus belonging to the restaurant.</returns>
        public async Task<List<Menu>> GetMenusByRestaurantAsync(int resId)
        {
            return await _apiService.GetAsync<List<Menu>>($"{ApiEndpoints.MENU}/restaurant/{resId}");
        }
        /// <summary>
        /// Retrieves a menu by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the menu.</param>
        /// <returns>Menu object with the specified ID.</returns>
        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            return await _apiService.GetAsync<Menu>($"{ApiEndpoints.MENU}/{id}");
        }
        /// <summary>
        /// Creates a new menu by sending a POST request to the API.
        /// </summary>
        /// <param name="menu">The menu object to be created.</param>
        /// <returns>The response from the API, usually the created menu ID or status.</returns>
        public async Task<object> CreateMenuAsync(Menu menu)
        {
            return await _apiService.PostAsync<object, Menu>(ApiEndpoints.MENU, menu);
        }
        /// <summary>
        /// Updates an existing menu by sending a PUT request to the API.
        /// </summary>
        /// <param name="menu">The menu object containing updated data.</param>
        /// <returns>The API response after update.</returns>
        public async Task<object> UpdateMenuAsync(Menu menu)
        {
            return await _apiService.PutAsync<object, Menu>($"{ApiEndpoints.MENU}/{menu.menu_id}", menu);
        }
        /// <summary>
        /// Performs a soft delete on a menu by setting menu_is_deleted to true.
        /// </summary>
        /// <param name="menu">The menu object to be deleted.</param>
        /// <returns>The API response after deletion.</returns>
        public async Task<object> DeleteMenuAsync(Menu menu)
        {
            menu.menu_is_deleted = true;
            return await _apiService.PutAsync<object, Menu>($"{ApiEndpoints.MENU}/{menu.menu_id}", menu);
        }

    }
}
