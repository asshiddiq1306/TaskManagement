using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Configurations;

namespace TaskManagement.Infrastructure.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TaskItemConfiguration());

            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new User
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );

            // Seed Tasks
            modelBuilder.Entity<TaskItem>().HasData(
                new
                {
                    Id = 1,
                    Title = "Sample Task 1",
                    Description = "This is a sample task for testing",
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Priority = TaskPriority.Medium,
                    Status = TaskStatus.Pending,
                    AssignedUserId = (int?)1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new
                {
                    Id = 2,
                    Title = "Sample Task 2",
                    Description = "Another sample task",
                    DueDate = DateTime.UtcNow.AddDays(3),
                    Priority = TaskPriority.High,
                    Status = TaskStatus.InProgress,
                    AssignedUserId = (int?)2,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update audit fields before saving
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
