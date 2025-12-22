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
    }
}
