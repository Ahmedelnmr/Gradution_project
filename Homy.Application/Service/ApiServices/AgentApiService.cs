using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service.ApiServices
{
    public class AgentApiService : IAgentApiService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AgentApiService(IUnitofwork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<PagedResultDto<AgentCardDto>> GetAgentsAsync(AgentFilterDto filter)
        {
            // Get all active users (agents/owners)
            var allUsers = await _unitOfWork.UserRepo.GetAllUsersAsync(
                role:  UserRole.Agent,
                isVerified: filter.IsVerified,
                isActive: true,
                searchTerm: filter.SearchTerm
            );

            // DEBUG: Log count
            System.Diagnostics.Debug.WriteLine($"Total agents from DB: {allUsers.Count()}");
            System.Diagnostics.Debug.WriteLine($"Agents with Properties loaded: {allUsers.Count(u => u.Properties != null)}");
            System.Diagnostics.Debug.WriteLine($"Agents with non-deleted Properties: {allUsers.Count(u => u.Properties != null && u.Properties.Any(p => !p.IsDeleted))}");

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

        // NOTE: This method will be moved to a separate FileUploadService for better separation of concerns
        // For now, we'll keep it here and refactor later
        public async Task<bool> SubmitVerificationRequestAsync(Guid userId, VerificationRequestDto request)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null) return false;

            // Prevent already verified users from re-uploading
            if (user.IsVerified)
                throw new InvalidOperationException("Your account is already verified. You cannot submit verification documents again.");

            // Validation: Check if files are images
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var maxFileSize = 5 * 1024 * 1024; // 5MB

            var files = new[] { request.IdCardFront, request.IdCardBack, request.SelfieWithId };
            foreach (var file in files)
            {
                if (file.Length > maxFileSize)
                    throw new InvalidOperationException($"File {file.FileName} exceeds maximum size of 5MB");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    throw new InvalidOperationException($"File {file.FileName} has invalid extension. Allowed: jpg, jpeg, png");
            }

            // Save uploaded files
            // IMPORTANT: This will be injected via constructor - see updated constructor below
            // For now, this is a placeholder showing the logic
            user.IdCardFrontUrl = await SaveVerificationFileAsync(request.IdCardFront, userId, "id_front");
            user.IdCardBackUrl = await SaveVerificationFileAsync(request.IdCardBack, userId, "id_back");
            user.SelfieWithIdUrl = await SaveVerificationFileAsync(request.SelfieWithId, userId, "selfie");

            // Mark as pending verification (IsVerified stays false until admin approves)
            user.VerificationRejectReason = null; // Clear any previous rejection

            await _unitOfWork.Save();
            return true;
        }

        // Helper method to save verification files
        // This uses a static folder path - will be replaced with IWebHostEnvironment injection
        private async Task<string> SaveVerificationFileAsync(IFormFile file, Guid userId, string fileType)
        {
            // For production: This path will come from IWebHostEnvironment.WebRootPath
            // Temporary solution for development - to be refactored
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(webRootPath, "uploads", "verification");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename: userId_fileType_timestamp.extension
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{userId}_{fileType}_{timestamp}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative URL path
            return $"/uploads/verification/{uniqueFileName}";
        }

        // ===== Profile Management Implementations =====

        public async Task<AgentProfileDto?> GetMyProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            // Get properties count
            var propertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .CountAsync();

            var activePropertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.UserId == userId && !p.IsDeleted && p.Status == PropertyStatus.Active)
                .CountAsync();

            return new AgentProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                WhatsAppNumber = user.WhatsAppNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                ActivePropertiesCount = activePropertiesCount,
                TotalPropertiesCount = propertiesCount,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<(bool success, string message)> UpdateProfileAsync(Guid userId, AgentProfileUpdateDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                return (false, "المستخدم غير موجود");

            // Update profile fields
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.WhatsAppNumber = dto.WhatsAppNumber;

            // Upload profile image if provided
            if (dto.ProfileImage != null)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                string uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Delete old image if exists
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldImagePath = Path.Combine(webRootPath, user.ProfileImageUrl.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                // Upload new image
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string extension = Path.GetExtension(dto.ProfileImage.FileName);
                string uniqueFileName = $"{userId}_profile_{timestamp}{extension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(fileStream);
                }

                user.ProfileImageUrl = $"/uploads/profiles/{uniqueFileName}";
            }

            await _unitOfWork.Save();

            return (true, "تم تحديث الملف الشخصي بنجاح");
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);
            if (user == null)
                return (false, "المستخدم غير موجود");

            // Verify current password
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);

            if (verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                return (false, "كلمة المرور الحالية غير صحيحة");

            // Hash new password
            user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);
            user.SecurityStamp = Guid.NewGuid().ToString();

            await _unitOfWork.Save();

            return (true, "تم تغيير كلمة المرور بنجاح");
        }
    }
}
