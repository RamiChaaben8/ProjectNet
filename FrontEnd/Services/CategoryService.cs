using FrontEnd.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace FrontEnd.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public CategoryService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private void AddAuthorizationHeader()
        {
            var token = _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<Category>> GetCategories()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Category>>("api/Category");
        }

        public async Task<Category> GetCategoryById(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<Category>($"api/Category/id/{id}");
        }

        public async Task<bool> CreateCategory(AddCategoryDTO category)
        {
            AddAuthorizationHeader();
            var result = await _httpClient.PostAsJsonAsync("api/Category", category);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategory(int id, Category category)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync("api/Category", category);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Category/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
