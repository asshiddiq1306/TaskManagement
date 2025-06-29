using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<UserResponseDto>> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(createUserDto.Email);
                if (existingUser != null)
                {
                    return ServiceResult<UserResponseDto>.Failure("Email already exists");
                }

                var user = new User
                {
                    Name = createUserDto.Name,
                    Email = createUserDto.Email,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var response = MapToResponseDto(createdUser);
                return ServiceResult<UserResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return ServiceResult<UserResponseDto>.Failure("An error occurred while creating the user");
            }
        }

        public async Task<ServiceResult<UserResponseDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return ServiceResult<UserResponseDto>.Failure("User not found");
                }

                var response = MapToResponseDto(user);
                return ServiceResult<UserResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", id);
                return ServiceResult<UserResponseDto>.Failure("An error occurred while retrieving the user");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var responses = users.Select(MapToResponseDto);
                return ServiceResult<IEnumerable<UserResponseDto>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return ServiceResult<IEnumerable<UserResponseDto>>.Failure("An error occurred while retrieving users");
            }
        }

        public async Task<ServiceResult<UserResponseDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(email);
                if (user == null)
                {
                    return ServiceResult<UserResponseDto>.Failure("User not found");
                }

                var response = MapToResponseDto(user);
                return ServiceResult<UserResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return ServiceResult<UserResponseDto>.Failure("An error occurred while retrieving the user");
            }
        }

        public async Task<ServiceResult> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return ServiceResult.Failure("User not found");
                }

                // Check if user has assigned tasks
                var userTasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(id);
                if (userTasks.Any())
                {
                    return ServiceResult.Failure("Cannot delete user with assigned tasks");
                }

                await _unitOfWork.Users.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                return ServiceResult.Failure("An error occurred while deleting the user");
            }
        }

        private UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                TaskCount = user.AssignedTasks?.Count ?? 0
            };
        }
    }
}
