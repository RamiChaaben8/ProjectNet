using FrontEnd.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace FrontEnd.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserById(string id);
        Task<bool> CreateUser(SignupDTO user);
        Task<bool> UpdateUser(string id, User user);
        Task<bool> DeleteUser(string id);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public UserService(HttpClient httpClient, IAuthService authService)
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

        public async Task<List<User>> GetUsers()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<User>>("api/User");
        }

        public async Task<User> GetUserById(string id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<User>($"api/User/{id}");
        }

        public async Task<bool> CreateUser(SignupDTO user)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/User", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUser(string id, User user)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/User/{id}", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUser(string id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/User/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
