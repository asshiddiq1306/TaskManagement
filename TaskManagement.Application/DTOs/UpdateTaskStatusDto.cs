using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.DTOs
{
    public class UpdateTaskStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        public TaskStatus Status { get; set; }
    }
}
