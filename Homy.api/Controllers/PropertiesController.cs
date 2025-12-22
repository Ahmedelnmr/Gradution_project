using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homy.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyApiService _propertyService;

        public PropertiesController(IPropertyApiService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// Get paginated and filtered properties
        /// </summary>
        /// <param name="filter">Filter and pagination parameters</param>
        /// <returns>Paginated list of properties</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<PropertyListItemDto>>> GetProperties([FromQuery] PropertyFilterDto filter)
        {
            try
            {
                var result = await _propertyService.GetPropertiesAsync(filter);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching properties", error = ex.Message });
            }
        }

        /// <summary>
        /// Get property details by ID
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Complete property details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PropertyDetailsDto>> GetPropertyById(long id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                
                if (property == null)
                    return NotFound(new { message = "Property not found" });

                return Ok(property);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching property details", error = ex.Message });
            }
        }

        /// <summary>
        /// Increment property view count
        /// </summary>
        /// <param name="id">Property ID</param>
        [HttpPost("{id}/view")]
        [AllowAnonymous]
        public async Task<IActionResult> IncrementViewCount(long id)
        {
            try
            {
                await _propertyService.IncrementViewCountAsync(id);
                return Ok(new { message = "View count incremented" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Track WhatsApp click
        /// </summary>
        /// <param name="id">Property ID</param>
        [HttpPost("{id}/whatsapp-click")]
        [AllowAnonymous]
        public async Task<IActionResult> IncrementWhatsAppClick(long id)
        {
            try
            {
                await _propertyService.IncrementWhatsAppClickAsync(id);
                return Ok(new { message = "WhatsApp click tracked" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Track phone click
        /// </summary>
        /// <param name="id">Property ID</param>
        [HttpPost("{id}/phone-click")]
        [AllowAnonymous]
        public async Task<IActionResult> IncrementPhoneClick(long id)
        {
            try
            {
                await _propertyService.IncrementPhoneClickAsync(id);
                return Ok(new { message = "Phone click tracked" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
