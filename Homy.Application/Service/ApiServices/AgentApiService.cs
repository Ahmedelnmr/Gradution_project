using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service.ApiServices
{
    public class AgentApiService : IAgentApiService
    {
        private readonly IUnitofwork _unitOfWork;

        public AgentApiService(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<AgentCardDto>> GetAgentsAsync(AgentFilterDto filter)
        {
            // Get all active users (agents/owners)
            var allUsers = await _unitOfWork.UserRepo.GetAllUsersAsync(
                role: null,
                isVerified: filter.IsVerified,
                isActive: true,
                searchTerm: filter.SearchTerm
            );

            // Filter users with properties
            var usersQuery = allUsers.Where(u => u.Properties != null && u.Properties.Any(p => !p.IsDeleted));

            // Filter by city (agents who have properties in this city)
            if (filter.CityId.HasValue)
            {
                usersQuery = usersQuery.Where(u =>
                    u.Properties.Any(p => p.CityId == filter.CityId.Value && !p.IsDeleted));
            }

            // Filter by purpose (agents who have properties for sale/rent)
            if (filter.Purpose.HasValue)
            {
                if (filter.Purpose == 1) // ForSale
                {
                    usersQuery = usersQuery.Where(u =>
                        u.Properties.Any(p => !p.IsDeleted &&
                            (p.Purpose == PropertyPurpose.ForSale || p.Purpose == PropertyPurpose.Both)));
                }
                else if (filter.Purpose == 2) // ForRent
                {
                    usersQuery = usersQuery.Where(u =>
                        u.Properties.Any(p => !p.IsDeleted &&
                            (p.Purpose == PropertyPurpose.ForRent || p.Purpose == PropertyPurpose.Both)));
                }
            }

            // Map to DTOs with property counts
            var agentsList = usersQuery.Select(u => new
            {
                User = u,
                ActivePropertiesCount = u.Properties.Count(p => !p.IsDeleted && p.Status == PropertyStatus.Active),
                TotalPropertiesCount = u.Properties.Count(p => !p.IsDeleted)
            }).ToList();

            // Get total count
            var totalCount = agentsList.Count;

            // Sorting
            agentsList = filter.SortBy?.ToLower() switch
            {
                "fullname" => filter.SortDescending
                    ? agentsList.OrderByDescending(a => a.User.FullName).ToList()
                    : agentsList.OrderBy(a => a.User.FullName).ToList(),
                "createdat" => filter.SortDescending
                    ? agentsList.OrderByDescending(a => a.User.CreatedAt).ToList()
                    : agentsList.OrderBy(a => a.User.CreatedAt).ToList(),
                _ => filter.SortDescending
                    ? agentsList.OrderByDescending(a => a.ActivePropertiesCount).ToList()
                    : agentsList.OrderBy(a => a.ActivePropertiesCount).ToList()
            };

            // Pagination
            var paginatedAgents = agentsList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var items = paginatedAgents.Select(a => new AgentCardDto
            {
                Id = a.User.Id,
                FullName = a.User.FullName,
                Email = a.User.Email,
                Phone = a.User.PhoneNumber,
                WhatsAppNumber = a.User.WhatsAppNumber,
                ProfileImageUrl = a.User.ProfileImageUrl,
                IsVerified = a.User.IsVerified,
                IsActive = a.User.IsActive,
                ActivePropertiesCount = a.ActivePropertiesCount,
                TotalPropertiesCount = a.TotalPropertiesCount,
                CreatedAt = a.User.CreatedAt
            }).ToList();

            return new PagedResultDto<AgentCardDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<AgentProfileDto?> GetAgentProfileAsync(Guid agentId)
        {
            var agent = await _unitOfWork.UserRepo.GetUserByIdAsync(agentId);

            if (agent == null || agent.IsDeleted)
                return null;

            // Load properties with related data
            var properties = _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.UserId == agentId && !p.IsDeleted)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Project)
                .Include(p => p.Images)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            var propertyDtos = properties
                .Select(p => new PropertyListItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    RentPriceMonthly = p.RentPriceMonthly,
                    Currency = "EGP",
                    PropertyType = p.PropertyType.Name,
                    PropertyTypeEn = p.PropertyType.NameEn,
                    City = p.City.Name,
                    CityEn = p.City.NameEn,
                    District = p.District?.Name,
                    DistrictEn = p.District?.NameEn,
                    ProjectName = p.Project?.Name,
                    Area = p.Area,
                    Rooms = p.Rooms,
                    Bathrooms = p.Bathrooms,
                    MainImageUrl = p.Images.OrderBy(i => i.SortOrder).FirstOrDefault(i => i.IsMain) != null
                        ? p.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl
                        : p.Images.OrderBy(i => i.SortOrder).FirstOrDefault() != null
                            ? p.Images.OrderBy(i => i.SortOrder).FirstOrDefault()!.ImageUrl
                            : null,
                    IsFeatured = p.IsFeatured,
                    Status = p.Status.ToString(),
                    Purpose = p.Purpose.ToString(),
                    FinishingType = p.FinishingType.HasValue ? p.FinishingType.Value.ToString() : null,
                    AgentId = agent.Id,
                    AgentName = agent.FullName,
                    AgentProfileImage = agent.ProfileImageUrl,
                    CreatedAt = p.CreatedAt,
                    ViewCount = p.ViewCount
                })
                .ToList();

            return new AgentProfileDto
            {
                Id = agent.Id,
                FullName = agent.FullName,
                Email = agent.Email,
                Phone = agent.PhoneNumber,
                WhatsAppNumber = agent.WhatsAppNumber,
                ProfileImageUrl = agent.ProfileImageUrl,
                IsVerified = agent.IsVerified,
                IsActive = agent.IsActive,
                CreatedAt = agent.CreatedAt,
                TotalPropertiesCount = propertyDtos.Count,
                ActivePropertiesCount = propertyDtos.Count(p => p.Status == "Active"),
                SoldOrRentedCount = propertyDtos.Count(p => p.Status == "SoldOrRented"),
                Properties = propertyDtos
            };
        }
    }
}
