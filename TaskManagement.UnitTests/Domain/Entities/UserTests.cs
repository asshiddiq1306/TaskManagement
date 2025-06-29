using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.UnitTests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_Creation_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow
            };

            // Act & Assert
            user.Id.Should().Be(1);
            user.Name.Should().Be("John Doe");
            user.Email.Should().Be("john.doe@example.com");
            user.CreatedBy.Should().Be("system");
            user.AssignedTasks.Should().NotBeNull();
            user.AssignedTasks.Should().BeEmpty();
        }

        [Fact]
        public void User_AssignedTasks_ShouldInitializeAsEmptyCollection()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.AssignedTasks.Should().NotBeNull();
            user.AssignedTasks.Should().BeEmpty();
            user.AssignedTasks.Should().BeOfType<List<TaskItem>>();
        }
    }
}
