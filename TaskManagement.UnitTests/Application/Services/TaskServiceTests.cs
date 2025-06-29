using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;
using TaskManagement.UnitTests.TestHelpers;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Application.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<TaskService>> _mockLogger;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<TaskService>>();

            _mockUnitOfWork.Setup(x => x.Tasks).Returns(_mockTaskRepository.Object);
            _mockUnitOfWork.Setup(x => x.Users).Returns(_mockUserRepository.Object);

            _taskService = new TaskService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_WithValidData_ShouldReturnSuccessResult()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = TaskPriority.High,
                AssignedUserId = null
            };

            var createdBy = "test-user";
            var expectedTask = TaskTestDataBuilder.NewTask()
                .WithTitle(createTaskDto.Title)
                .WithDescription(createTaskDto.Description)
                .WithDueDate(createTaskDto.DueDate)
                .WithPriority(createTaskDto.Priority)
                .WithCreatedBy(createdBy)
                .Build();

            _mockTaskRepository.Setup(x => x.AddAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(expectedTask);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _taskService.CreateTaskAsync(createTaskDto, createdBy);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(createTaskDto.Title);
            result.Data.Description.Should().Be(createTaskDto.Description);
            result.Data.Priority.Should().Be(createTaskDto.Priority);

            _mockTaskRepository.Verify(x => x.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_WithAssignedUserThatDoesNotExist_ShouldReturnFailureResult()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = TaskPriority.High,
                AssignedUserId = 999 // Non-existent user
            };
            var createdBy = "test-user";

            _mockUserRepository.Setup(x => x.ExistsAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _taskService.CreateTaskAsync(createTaskDto, createdBy);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Assigned user does not exist");

            _mockUserRepository.Verify(x => x.ExistsAsync(999), Times.Once);
            _mockTaskRepository.Verify(x => x.AddAsync(It.IsAny<TaskItem>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithExistingId_ShouldReturnSuccessResult()
        {
            // Arrange
            var taskId = 1;
            var existingTask = TaskTestDataBuilder.NewTask()
                .WithTitle("Test Task")
                .WithDescription("Description")
                .Build();

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be("Test Task");

            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithNonExistentId_ShouldReturnFailureResult()
        {
            // Arrange
            var taskId = 999;

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Task not found");

            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_WithValidData_ShouldReturnSuccessResult()
        {
            // Arrange
            var taskId = 1;
            var updateStatusDto = new UpdateTaskStatusDto { Status = TaskStatus.InProgress };
            var updatedBy = "test-user";

            var existingTask = TaskTestDataBuilder.NewTask()
                .WithTitle("Test Task")
                .WithPriority(TaskPriority.Medium)
                .Build();

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, updateStatusDto, updatedBy);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Status.Should().Be(TaskStatus.InProgress);

            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.UpdateAsync(existingTask), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_WithTaskThatCanBeDeleted_ShouldReturnSuccessResult()
        {
            // Arrange
            var taskId = 1;
            var existingTask = TaskTestDataBuilder.NewTask()
                .WithTitle("Test Task")
                .Build();
            existingTask.UpdateStatus(TaskStatus.Pending, "user"); // Pending tasks can be deleted

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.DeleteAsync(existingTask), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_WithTaskInProgress_ShouldReturnFailureResult()
        {
            // Arrange
            var taskId = 1;
            var existingTask = TaskTestDataBuilder.NewTask()
                .WithTitle("In Progress Task")
                .WithPriority(TaskPriority.High)
                .Build();
            existingTask.UpdateStatus(TaskStatus.InProgress, "user");// InProgress tasks cannot be deleted

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Cannot delete task that is in progress");

            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.DeleteAsync(It.IsAny<TaskItem>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignTaskAsync_WithValidData_ShouldReturnSuccessResult()
        {
            // Arrange
            var taskId = 1;
            var assignTaskDto = new AssignTaskDto { UserId = 2 };
            var updatedBy = "test-user";

            var existingTask = TaskTestDataBuilder.NewTask()
                .WithTitle("Assignable Task")
                .Build();

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            _mockUserRepository.Setup(x => x.ExistsAsync(assignTaskDto.UserId))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _taskService.AssignTaskAsync(taskId, assignTaskDto, updatedBy);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AssignedUserId.Should().Be(assignTaskDto.UserId);

            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockUserRepository.Verify(x => x.ExistsAsync(assignTaskDto.UserId), Times.Once);
            _mockTaskRepository.Verify(x => x.UpdateAsync(existingTask), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
