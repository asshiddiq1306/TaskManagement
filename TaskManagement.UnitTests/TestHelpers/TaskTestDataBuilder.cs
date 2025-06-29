using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.UnitTests.TestHelpers
{
    public class TaskTestDataBuilder
    {
        private string _title = "Default Task";
        private string _description = "Default Description";
        private DateTime? _dueDate = DateTime.UtcNow.AddDays(7);
        private TaskPriority _priority = TaskPriority.Medium;
        private string _createdBy = "test-user";

        public TaskTestDataBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public TaskTestDataBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public TaskTestDataBuilder WithDueDate(DateTime? dueDate)
        {
            _dueDate = dueDate;
            return this;
        }

        public TaskTestDataBuilder WithPriority(TaskPriority priority)
        {
            _priority = priority;
            return this;
        }

        public TaskTestDataBuilder WithCreatedBy(string createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public TaskTestDataBuilder WithFutureDueDate()
        {
            _dueDate = DateTime.UtcNow.AddDays(7);
            return this;
        }

        public TaskItem Build()
        {
            return TaskItem.Create(_title, _description, _dueDate, _priority, _createdBy);
        }

        public static TaskTestDataBuilder NewTask()
        {
            return new TaskTestDataBuilder();
        }
    }
}
