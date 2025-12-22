using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Homy.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentApiService _agentService;

        public AgentsController(IAgentApiService agentService)
        {
            _agentService = agentService;
        }

        /// <summary>
        /// Get paginated and filtered agents/brokers
        /// </summary>
        /// <param name="filter">Filter and pagination parameters</param>
        /// <returns>Paginated list of agents</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<AgentCardDto>>> GetAgents([FromQuery] AgentFilterDto filter)
        {
            try
            {
                var result = await _agentService.GetAgentsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching agents", error = ex.Message });
            }
        }

        /// <summary>
        /// Get agent profile with their properties
        /// </summary>
        /// <param name="id">Agent ID (GUID)</param>
        /// <returns>Complete agent profile with properties</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AgentProfileDto>> GetAgentProfile(Guid id)
        {
            try
            {
                var agent = await _agentService.GetAgentProfileAsync(id);

                if (agent == null)
                    return NotFound(new { message = "Agent not found" });

                return Ok(agent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching agent profile", error = ex.Message });
            }
        }
    }
}
