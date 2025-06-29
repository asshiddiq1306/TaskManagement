using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<UserResponseDto>> CreateUserAsync(CreateUserDto createUserDto, string createdBy);
        Task<ServiceResult<UserResponseDto>> GetUserByIdAsync(int id);
        Task<ServiceResult<IEnumerable<UserResponseDto>>> GetAllUsersAsync();
        Task<ServiceResult<UserResponseDto>> GetUserByEmailAsync(string email);
        Task<ServiceResult> DeleteUserAsync(int id);
    }
}
