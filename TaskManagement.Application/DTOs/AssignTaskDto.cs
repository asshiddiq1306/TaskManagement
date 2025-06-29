using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.DTOs
{
    public class AssignTaskDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }
    }
}
