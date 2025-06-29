using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        Task<ServiceResult<TaskResponseDto>> CreateTaskAsync(CreateTaskDto createTaskDto, string createdBy);
        Task<ServiceResult<TaskResponseDto>> GetTaskByIdAsync(int id);
        Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetAllTasksAsync();
        Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetTasksByUserIdAsync(int userId);
        Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetTasksByStatusAsync(TaskStatus status);
        Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetOverdueTasksAsync();
        Task<ServiceResult<TaskResponseDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, string updatedBy);
        Task<ServiceResult<TaskResponseDto>> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto updateStatusDto, string updatedBy);
        Task<ServiceResult<TaskResponseDto>> AssignTaskAsync(int taskId, AssignTaskDto assignTaskDto, string updatedBy);
        Task<ServiceResult<TaskResponseDto>> UnassignTaskAsync(int taskId, string updatedBy);
        Task<ServiceResult> DeleteTaskAsync(int id);
    }
}
