using FrontEnd.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace FrontEnd.Services
{
    public class ArticleService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ArticleService(HttpClient httpClient, IAuthService authService)
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

        public async Task<List<Article>> GetArticles()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Article>>("api/Article") ?? new List<Article>();
        }

        public async Task<Article> GetArticleById(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<Article>($"api/Article/{id}");
        }

        public async Task<bool> CreateArticle(Article article)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Article", article);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateArticle(Article article)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync("api/Article", article);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteArticle(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Article/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
