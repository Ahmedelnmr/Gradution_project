using Homy.Application.Dtos.ApiDtos;
using System;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    public interface IAgentApiService
    {
        /// <summary>
        /// Get paginated and filtered agents list
        /// </summary>
        Task<PagedResultDto<AgentCardDto>> GetAgentsAsync(AgentFilterDto filter);
        
        /// <summary>
        /// Get complete agent profile with their properties
        /// </summary>
        Task<AgentProfileDto?> GetAgentProfileAsync(Guid agentId);

        /// <summary>
        /// Submit identity verification documents
        /// </summary>
        Task<bool> SubmitVerificationRequestAsync(Guid userId, VerificationRequestDto request);

        // ===== Profile Management =====

        /// <summary>
        /// Get current user's profile
        /// </summary>
        Task<AgentProfileDto?> GetMyProfileAsync(Guid userId);

        /// <summary>
        /// Update agent profile
        /// </summary>
        Task<(bool success, string message)> UpdateProfileAsync(Guid userId, AgentProfileUpdateDto dto);

        /// <summary>
        /// Change password
        /// </summary>
        Task<(bool success, string message)> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}
