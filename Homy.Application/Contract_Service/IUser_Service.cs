using Homy.Application.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service
{
        public interface IUser_Service
        {
            Task<PaginatedResponse<UserDto>> GetAllUsersAsync(UserFilterDto filter);

            Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);

            Task<ApiResponse<IEnumerable<UserDto>>> GetUnverifiedAgentsAsync();

            Task<ApiResponse<bool>> VerifyAgentAsync(VerificationRequestDto request);

            
            Task<ApiResponse<bool>> UpdateUserActiveStatusAsync(UpdateUserStatusDto request);

            
            Task<ApiResponse<bool>> DeleteUserAsync(Guid userId);

            
            Task<ApiResponse<UserStatisticsDto>> GetUserStatisticsAsync();

            
            Task<ApiResponse<IEnumerable<UserDto>>> SearchUsersAsync(string searchTerm);
        }
    }