using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Controllers
{
    public class TasksController : BaseApiController
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <returns>List of all tasks</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            _logger.LogInformation("Getting all tasks");
            var result = await _taskService.GetAllTasksAsync();
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            _logger.LogInformation("Getting task with ID: {TaskId}", id);
            var result = await _taskService.GetTaskByIdAsync(id);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="createTaskDto">Task creation data</param>
        /// <returns>Created task</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new task: {Title}", createTaskDto.Title);
            var result = await _taskService.CreateTaskAsync(createTaskDto, GetCurrentUser());

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetTaskById), new { id = result.Data!.Id }, result.Data);
            }

            return HandleServiceResult(result);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="updateTaskDto">Task update data</param>
        /// <returns>Updated task</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating task with ID: {TaskId}", id);
            var result = await _taskService.UpdateTaskAsync(id, updateTaskDto, GetCurrentUser());
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="updateStatusDto">Status update data</param>
        /// <returns>Updated task</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto updateStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating status for task with ID: {TaskId} to {Status}", id, updateStatusDto.Status);
            var result = await _taskService.UpdateTaskStatusAsync(id, updateStatusDto, GetCurrentUser());
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Assign task to a user
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="assignTaskDto">Assignment data</param>
        /// <returns>Updated task</returns>
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTask(int id, [FromBody] AssignTaskDto assignTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Assigning task {TaskId} to user {UserId}", id, assignTaskDto.UserId);
            var result = await _taskService.AssignTaskAsync(id, assignTaskDto, GetCurrentUser());
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Unassign task from user
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Updated task</returns>
        [HttpPatch("{id}/unassign")]
        public async Task<IActionResult> UnassignTask(int id)
        {
            _logger.LogInformation("Unassigning task with ID: {TaskId}", id);
            var result = await _taskService.UnassignTaskAsync(id, GetCurrentUser());
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", id);
            var result = await _taskService.DeleteTaskAsync(id);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get tasks by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user's tasks</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            _logger.LogInformation("Getting tasks for user: {UserId}", userId);
            var result = await _taskService.GetTasksByUserIdAsync(userId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get tasks by status
        /// </summary>
        /// <param name="status">Task status</param>
        /// <returns>List of tasks with specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTasksByStatus(TaskStatus status)
        {
            _logger.LogInformation("Getting tasks with status: {Status}", status);
            var result = await _taskService.GetTasksByStatusAsync(status);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        /// <returns>List of overdue tasks</returns>
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            _logger.LogInformation("Getting overdue tasks");
            var result = await _taskService.GetOverdueTasksAsync();
            return HandleServiceResult(result);
        }
    }
}
