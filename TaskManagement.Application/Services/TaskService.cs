using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskService> _logger;

        public TaskService(IUnitOfWork unitOfWork, ILogger<TaskService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<TaskResponseDto>> CreateTaskAsync(CreateTaskDto createTaskDto, string createdBy)
        {
            using var scope = _logger.BeginScope("CreateTask {Title} by {CreatedBy}", createTaskDto.Title, createdBy);

            try
            {
                _logger.LogInformation("Starting task creation with title: {Title}, priority: {Priority}, dueDate: {DueDate}",
                    createTaskDto.Title, createTaskDto.Priority, createTaskDto.DueDate);

                // Validate assigned user exists if provided
                if (createTaskDto.AssignedUserId.HasValue)
                {
                    _logger.LogDebug("Validating assigned user with ID: {UserId}", createTaskDto.AssignedUserId.Value);

                    var userExists = await _unitOfWork.Users.ExistsAsync(createTaskDto.AssignedUserId.Value);
                    if (!userExists)
                    {
                        _logger.LogWarning("Task creation failed: Assigned user {UserId} does not exist", createTaskDto.AssignedUserId.Value);
                        return ServiceResult<TaskResponseDto>.Failure("Assigned user does not exist");
                    }

                    _logger.LogDebug("User validation successful for user ID: {UserId}", createTaskDto.AssignedUserId.Value);
                }

                // Create task using factory method
                var task = TaskItem.Create(
                    createTaskDto.Title,
                    createTaskDto.Description,
                    createTaskDto.DueDate,
                    createTaskDto.Priority,
                    createdBy
                );

                _logger.LogDebug("Task entity created with ID: {TaskId}", task.Id);

                // Assign user if provided
                if (createTaskDto.AssignedUserId.HasValue)
                {
                    task.AssignToUser(createTaskDto.AssignedUserId.Value, createdBy);
                    _logger.LogInformation("Task assigned to user {UserId}", createTaskDto.AssignedUserId.Value);
                }

                // Save to database
                var createdTask = await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task created successfully with ID: {TaskId}, Title: {Title}",
                    createdTask.Id, createdTask.Title);

                var response = await MapToResponseDto(createdTask);
                return ServiceResult<TaskResponseDto>.Success(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error creating task: {Message}", ex.Message);
                return ServiceResult<TaskResponseDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error occurred while creating task with title: {Title}", createTaskDto.Title);
                return ServiceResult<TaskResponseDto>.Failure("An error occurred while creating the task");
            }
        }

        public async Task<ServiceResult<TaskResponseDto>> GetTaskByIdAsync(int id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task == null)
                {
                    return ServiceResult<TaskResponseDto>.Failure("Task not found");
                }

                var response = await MapToResponseDto(task);
                return ServiceResult<TaskResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task with ID: {TaskId}", id);
                return ServiceResult<TaskResponseDto>.Failure("An error occurred while retrieving the task");
            }
        }

        public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetAllAsync();
                var responses = new List<TaskResponseDto>();

                foreach (var task in tasks)
                {
                    responses.Add(await MapToResponseDto(task));
                }

                return ServiceResult<IEnumerable<TaskResponseDto>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tasks");
                return ServiceResult<IEnumerable<TaskResponseDto>>.Failure("An error occurred while retrieving tasks");
            }
        }

        public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetTasksByUserIdAsync(int userId)
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId);
                var responses = new List<TaskResponseDto>();

                foreach (var task in tasks)
                {
                    responses.Add(await MapToResponseDto(task));
                }

                return ServiceResult<IEnumerable<TaskResponseDto>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for user: {UserId}", userId);
                return ServiceResult<IEnumerable<TaskResponseDto>>.Failure("An error occurred while retrieving user tasks");
            }
        }

        public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetTasksByStatusAsync(TaskStatus status)
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetTasksByStatusAsync(status);
                var responses = new List<TaskResponseDto>();

                foreach (var task in tasks)
                {
                    responses.Add(await MapToResponseDto(task));
                }

                return ServiceResult<IEnumerable<TaskResponseDto>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks by status: {Status}", status);
                return ServiceResult<IEnumerable<TaskResponseDto>>.Failure("An error occurred while retrieving tasks by status");
            }
        }

        public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetOverdueTasksAsync()
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetOverdueTasksAsync();
                var responses = new List<TaskResponseDto>();

                foreach (var task in tasks)
                {
                    responses.Add(await MapToResponseDto(task));
                }

                return ServiceResult<IEnumerable<TaskResponseDto>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue tasks");
                return ServiceResult<IEnumerable<TaskResponseDto>>.Failure("An error occurred while retrieving overdue tasks");
            }
        }

        public async Task<ServiceResult<TaskResponseDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, string updatedBy)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task == null)
                {
                    return ServiceResult<TaskResponseDto>.Failure("Task not found");
                }

                // Update task using domain method
                task.UpdateDetails(
                    updateTaskDto.Title,
                    updateTaskDto.Description,
                    updateTaskDto.DueDate,
                    updateTaskDto.Priority,
                    updatedBy
                );

                await _unitOfWork.Tasks.UpdateAsync(task);
                await _unitOfWork.SaveChangesAsync();

                var response = await MapToResponseDto(task);
                return ServiceResult<TaskResponseDto>.Success(response);
            }
            catch (ArgumentException ex)
            {
                return ServiceResult<TaskResponseDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", id);
                return ServiceResult<TaskResponseDto>.Failure("An error occurred while updating the task");
            }
        }

        public async Task<ServiceResult<TaskResponseDto>> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto updateStatusDto, string updatedBy)
        {
            using var scope = _logger.BeginScope("UpdateTaskStatus {TaskId} to {Status} by {UpdatedBy}", id, updateStatusDto.Status, updatedBy);

            try
            {
                _logger.LogInformation("Starting task status update for task ID: {TaskId} to status: {Status}", id, updateStatusDto.Status);

                var task = await _unitOfWork.Tasks.GetByIdAsync(id);         
                if (task == null)
                {
                    _logger.LogWarning("Task status update failed: Task with ID {TaskId} not found", id);
                    return ServiceResult<TaskResponseDto>.Failure("Task not found");
                }

                var oldStatus = task.Status;
                task.UpdateStatus(updateStatusDto.Status, updatedBy);
                await _unitOfWork.Tasks.UpdateAsync(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task status updated successfully: Task {TaskId} changed from {OldStatus} to {NewStatus} by {UpdatedBy}",
                    id, oldStatus, updateStatusDto.Status, updatedBy);

                var response = await MapToResponseDto(task);
                return ServiceResult<TaskResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status with ID: {TaskId}", id);
                return ServiceResult<TaskResponseDto>.Failure("An error occurred while updating task status");
            }
        }

        public async Task<ServiceResult<TaskResponseDto>> AssignTaskAsync(int taskId, AssignTaskDto assignTaskDto, string updatedBy)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                {
                    return ServiceResult<TaskResponseDto>.Failure("Task not found");
                }

                var userExists = await _unitOfWork.Users.ExistsAsync(assignTaskDto.UserId);
                if (!userExists)
                {
                    return ServiceResult<TaskResponseDto>.Failure("User not found");
                }

                task.AssignToUser(assignTaskDto.UserId, updatedBy);
                await _unitOfWork.Tasks.UpdateAsync(task);
                await _unitOfWork.SaveChangesAsync();

                var response = await MapToResponseDto(task);
                return ServiceResult<TaskResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task with ID: {TaskId}", taskId);
                return ServiceResult<TaskResponseDto>.Failure("An error occurred while assigning the task");
            }
        }

        public async Task<ServiceResult<TaskResponseDto>> UnassignTaskAsync(int taskId, string updatedBy)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                {
                    return ServiceResult<TaskResponseDto>.Failure("Task not found");
                }

                task.UnassignFromUser(updatedBy);
                await _unitOfWork.Tasks.UpdateAsync(task);
                await _unitOfWork.SaveChangesAsync();

                var response = await MapToResponseDto(task);
                return ServiceResult<TaskResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unassigning task with ID: {TaskId}", taskId);
                return ServiceResult<TaskResponseDto>.Failure("An error occurred while unassigning the task");
            }
        }

        public async Task<ServiceResult> DeleteTaskAsync(int id)
        {
            using var scope = _logger.BeginScope("DeleteTask {TaskId}", id);

            try
            {
                _logger.LogInformation("Starting task deletion for task ID: {TaskId}", id);

                var task = await _unitOfWork.Tasks.GetByIdAsync(id);                
                if (task == null)
                {
                    _logger.LogWarning("Task deletion failed: Task with ID {TaskId} not found", id);
                    return ServiceResult.Failure("Task not found");
                }
                
                if (!task.CanBeDeleted())
                {
                    _logger.LogWarning("Task deletion blocked: Task {TaskId} with status {Status} cannot be deleted",
                        id, task.Status);
                    return ServiceResult.Failure("Cannot delete task that is in progress");
                }

                await _unitOfWork.Tasks.DeleteAsync(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task deleted successfully: Task {TaskId} '{Title}' removed from system",
                    id, task.Title);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
                return ServiceResult.Failure("An error occurred while deleting the task");
            }
        }

        private async Task<TaskResponseDto> MapToResponseDto(TaskItem task)
        {
            string? assignedUserName = null;
            if (task.AssignedUserId.HasValue)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(task.AssignedUserId.Value);
                assignedUserName = user?.Name;
            }

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = assignedUserName,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CreatedBy = task.CreatedBy,
                UpdatedBy = task.UpdatedBy,
                IsOverdue = task.IsOverdue(),
                CanBeDeleted = task.CanBeDeleted()
            };
        }
    }
}
