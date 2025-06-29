using System.Text;
using System.Text.Json;
using TaskManagement.Application.DTOs;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Web.Services
{
    public class TaskApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TaskApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TaskManagementAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<TaskResponseDto>> GetAllTasksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/tasks");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TaskResponseDto>>(json, _jsonOptions) ?? new List<TaskResponseDto>();
                }
                return new List<TaskResponseDto>();
            }
            catch
            {
                return new List<TaskResponseDto>();
            }
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/tasks/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TaskResponseDto>(json, _jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TaskResponseDto>> GetOverdueTasksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/tasks/overdue");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TaskResponseDto>>(json, _jsonOptions) ?? new List<TaskResponseDto>();
                }
                return new List<TaskResponseDto>();
            }
            catch
            {
                return new List<TaskResponseDto>();
            }
        }

        public async Task<List<TaskResponseDto>> GetTasksByUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/tasks/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TaskResponseDto>>(json, _jsonOptions) ?? new List<TaskResponseDto>();
                }
                return new List<TaskResponseDto>();
            }
            catch
            {
                return new List<TaskResponseDto>();
            }
        }

        public async Task<(bool Success, string Message)> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createTaskDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/tasks", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Task created successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to create task: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateTaskDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/tasks/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Task updated successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to update task: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateTaskStatusAsync(int id, TaskStatus status)
        {
            try
            {
                var updateStatusDto = new UpdateTaskStatusDto { Status = status };
                var json = JsonSerializer.Serialize(updateStatusDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"api/tasks/{id}/status", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Task status updated successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to update status: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> AssignTaskAsync(int taskId, int userId)
        {
            try
            {
                var assignTaskDto = new AssignTaskDto { UserId = userId };
                var json = JsonSerializer.Serialize(assignTaskDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"api/tasks/{taskId}/assign", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Task assigned successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to assign task: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UnassignTaskAsync(int taskId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/tasks/{taskId}/unassign", null);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Task unassigned successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to unassign task: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteTaskAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/tasks/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Task deleted successfully!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to delete task: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}
