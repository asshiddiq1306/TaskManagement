using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TaskManagementDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
            Tasks = new TaskRepository(_context);
            Users = new UserRepository(_context);
        }

        public ITaskRepository Tasks { get; private set; }
        public IUserRepository Users { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                _logger.LogDebug("Saving changes to database...");
                var result = await _context.SaveChangesAsync();
                _logger.LogDebug("Successfully saved {ChangeCount} changes to database", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes to database");
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            try
            {
                _logger.LogInformation("Beginning database transaction");
                _transaction = await _context.Database.BeginTransactionAsync();
                _logger.LogDebug("Database transaction started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while beginning database transaction");
                throw;
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                try
                {
                    _logger.LogInformation("Committing database transaction");
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                    _logger.LogDebug("Database transaction committed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while committing database transaction");
                    throw;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                try
                {
                    _logger.LogWarning("Rolling back database transaction");
                    await _transaction.RollbackAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                    _logger.LogInformation("Database transaction rolled back successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while rolling back database transaction");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            try
            {
                _transaction?.Dispose();
                _context.Dispose();
                _logger.LogDebug("UnitOfWork disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disposing UnitOfWork");
            }
        }
    }
}
