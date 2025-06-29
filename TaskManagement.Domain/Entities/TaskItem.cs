using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime? DueDate { get; private set; }
        public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
        public TaskStatus Status { get; private set; } = TaskStatus.Pending;

        // Foreign Key
        public int? AssignedUserId { get; private set; }

        // Navigation property
        public User? AssignedUser { get; set; }

        // Private constructor for EF Core
        private TaskItem() { }

        // Factory method (Creational Pattern)
        public static TaskItem Create(string title, string description, DateTime? dueDate,
            TaskPriority priority, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
                throw new ArgumentException("Due date cannot be in the past", nameof(dueDate));

            return new TaskItem
            {
                Title = title.Trim(),
                Description = description?.Trim() ?? string.Empty,
                DueDate = dueDate,
                Priority = priority,
                Status = TaskStatus.Pending,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDetails(string title, string description, DateTime? dueDate,
            TaskPriority priority, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
                throw new ArgumentException("Due date cannot be in the past", nameof(dueDate));

            Title = title.Trim();
            Description = description?.Trim() ?? string.Empty;
            DueDate = dueDate;
            Priority = priority;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(TaskStatus status, string updatedBy)
        {
            Status = status;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignToUser(int userId, string updatedBy)
        {
            AssignedUserId = userId;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UnassignFromUser(string updatedBy)
        {
            AssignedUserId = null;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanBeDeleted()
        {
            return Status != TaskStatus.InProgress;
        }

        public bool IsOverdue()
        {
            return DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != TaskStatus.Completed;
        }
    }
}
