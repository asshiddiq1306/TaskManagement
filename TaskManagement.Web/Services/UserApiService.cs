using System.Text;
using System.Text.Json;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Web.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TaskManagementAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UserResponseDto>>(json, _jsonOptions) ?? new List<UserResponseDto>();
                }
                return new List<UserResponseDto>();
            }
            catch
            {
                return new List<UserResponseDto>();
            }
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserResponseDto>(json, _jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createUserDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/users", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "User created successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to create user: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "User deleted successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to delete user: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}
