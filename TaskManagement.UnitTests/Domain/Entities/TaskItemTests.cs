using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.UnitTests.TestHelpers;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.UnitTests.Domain.Entities
{
    public class TaskItemTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateTaskSuccessfully()
        {
            // Arrange
            var title = "Test Task";
            var description = "Test Description";
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.High;
            var createdBy = "test-user";

            // Act
            var task = TaskItem.Create(title, description, dueDate, priority, createdBy);

            // Assert
            task.Should().NotBeNull();
            task.Title.Should().Be(title);
            task.Description.Should().Be(description);
            task.DueDate.Should().Be(dueDate);
            task.Priority.Should().Be(priority);
            task.Status.Should().Be(TaskStatus.Pending);
            task.CreatedBy.Should().Be(createdBy);
            task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            task.AssignedUserId.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var emptyTitle = "";
            var description = "Test Description";
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.Medium;
            var createdBy = "test-user";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                TaskItem.Create(emptyTitle, description, dueDate, priority, createdBy));

            exception.Message.Should().Contain("Title cannot be empty");
        }

        [Fact]
        public void Create_WithWhitespaceTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var whitespaceTitle = "   ";
            var description = "Test Description";
            var dueDate = DateTime.UtcNow.AddDays(7);
            var priority = TaskPriority.Medium;
            var createdBy = "test-user";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                TaskItem.Create(whitespaceTitle, description, dueDate, priority, createdBy));

            exception.Message.Should().Contain("Title cannot be empty");
        }

        [Fact]
        public void Create_WithPastDueDate_ShouldThrowArgumentException()
        {
            // Arrange
            var title = "Test Task";
            var description = "Test Description";
            var pastDueDate = DateTime.UtcNow.AddDays(-1);
            var priority = TaskPriority.Medium;
            var createdBy = "test-user";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                TaskItem.Create(title, description, pastDueDate, priority, createdBy));

            exception.Message.Should().Contain("Due date cannot be in the past");
        }

        [Fact]
        public void Create_WithNullDueDate_ShouldCreateTaskSuccessfully()
        {
            // Arrange
            var title = "Test Task";
            var description = "Test Description";
            DateTime? nullDueDate = null;
            var priority = TaskPriority.Low;
            var createdBy = "test-user";

            // Act
            var task = TaskItem.Create(title, description, nullDueDate, priority, createdBy);

            // Assert
            task.Should().NotBeNull();
            task.DueDate.Should().BeNull();
        }

        [Fact]
        public void UpdateDetails_WithValidData_ShouldUpdateTaskSuccessfully()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask()
                .WithTitle("Original Task")
                .WithPriority(TaskPriority.Low)
                .Build();

            var newTitle = "Updated Task";
            var newDescription = "Updated Description";
            var newDueDate = DateTime.UtcNow.AddDays(14);
            var newPriority = TaskPriority.Critical;
            var updatedBy = "updater-user";

            // Act
            task.UpdateDetails(newTitle, newDescription, newDueDate, newPriority, updatedBy);

            // Assert
            task.Title.Should().Be(newTitle);
            task.Description.Should().Be(newDescription);
            task.DueDate.Should().Be(newDueDate);
            task.Priority.Should().Be(newPriority);
            task.UpdatedBy.Should().Be(updatedBy);
            task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UpdateStatus_WithValidStatus_ShouldUpdateStatusSuccessfully()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask()
                .WithTitle("Test Task")
                .WithPriority(TaskPriority.Medium)
                .Build();

            var newStatus = TaskStatus.InProgress;
            var updatedBy = "updater-user";

            // Act
            task.UpdateStatus(newStatus, updatedBy);

            // Assert
            task.Status.Should().Be(newStatus);
            task.UpdatedBy.Should().Be(updatedBy);
            task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void AssignToUser_WithValidUserId_ShouldAssignUserSuccessfully()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask().Build();
            var userId = 123;
            var updatedBy = "assigner-user";

            // Act
            task.AssignToUser(userId, updatedBy);

            // Assert
            task.AssignedUserId.Should().Be(userId);
            task.UpdatedBy.Should().Be(updatedBy);
            task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UnassignFromUser_ShouldRemoveUserAssignment()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask().Build();
            task.AssignToUser(123, "assigner");
            var updatedBy = "unassigner-user";

            // Act
            task.UnassignFromUser(updatedBy);

            // Assert
            task.AssignedUserId.Should().BeNull();
            task.UpdatedBy.Should().Be(updatedBy);
            task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void CanBeDeleted_WhenStatusIsPending_ShouldReturnTrue()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask().Build();
            task.UpdateStatus(TaskStatus.Pending, "user");

            // Act
            var canBeDeleted = task.CanBeDeleted();

            // Assert
            canBeDeleted.Should().BeTrue();
        }

        [Fact]
        public void CanBeDeleted_WhenStatusIsInProgress_ShouldReturnFalse()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask().Build();
            task.UpdateStatus(TaskStatus.InProgress, "user");

            // Act
            var canBeDeleted = task.CanBeDeleted();

            // Assert
            canBeDeleted.Should().BeFalse();
        }

        [Fact]
        public void CanBeDeleted_WhenStatusIsCompleted_ShouldReturnTrue()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask().Build();
            task.UpdateStatus(TaskStatus.Completed, "user");

            // Act
            var canBeDeleted = task.CanBeDeleted();

            // Assert
            canBeDeleted.Should().BeTrue();
        }

        [Fact]
        public void IsOverdue_WhenDueDateIsNull_ShouldReturnFalse()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask()
                .WithDueDate(null)
                .Build();

            // Act
            var isOverdue = task.IsOverdue();

            // Assert
            isOverdue.Should().BeFalse();
        }

        [Fact]
        public void IsOverdue_WhenDueDateIsFuture_ShouldReturnFalse()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask()
                .WithFutureDueDate()
                .Build();

            // Act
            var isOverdue = task.IsOverdue();

            // Assert
            isOverdue.Should().BeFalse();
        }

        [Fact]
        public void IsOverdue_WhenDueDateIsFutureAndNotCompleted_ShouldReturnFalse()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask()
                .WithFutureDueDate()
                .Build();
            task.UpdateStatus(TaskStatus.Pending, "user");

            // Act
            var isOverdue = task.IsOverdue();

            // Assert
            isOverdue.Should().BeFalse();
        }

        [Fact]
        public void IsOverdue_WhenDueDateIsFutureButCompleted_ShouldReturnFalse()
        {
            // Arrange
            var task = TaskTestDataBuilder.NewTask()
                .WithFutureDueDate()
                .Build();
            task.UpdateStatus(TaskStatus.Completed, "user");

            // Act
            var isOverdue = task.IsOverdue();

            // Assert
            isOverdue.Should().BeFalse();
        }

        [Fact]
        public void Create_WithHighPriorityTask_ShouldSetPriorityCorrectly()
        {
            // Arrange & Act
            var task = TaskTestDataBuilder.NewTask()
                .WithTitle("Critical Bug Fix")
                .WithPriority(TaskPriority.Critical)
                .WithCreatedBy("senior-dev")
                .Build();

            // Assert
            task.Title.Should().Be("Critical Bug Fix");
            task.Priority.Should().Be(TaskPriority.Critical);
            task.CreatedBy.Should().Be("senior-dev");
            task.Status.Should().Be(TaskStatus.Pending);
        }

        [Fact]
        public void Create_WithCustomDescription_ShouldSetDescriptionCorrectly()
        {
            // Arrange & Act
            var customDescription = "This is a detailed task description for testing purposes";
            var task = TaskTestDataBuilder.NewTask()
                .WithDescription(customDescription)
                .Build();

            // Assert
            task.Description.Should().Be(customDescription);
        }
    }
}
