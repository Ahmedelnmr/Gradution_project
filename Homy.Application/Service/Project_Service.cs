using Homy.Application.Dtos;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Service
{
    public class Project_Service : IProject_Service
    {
        private readonly IUnitofwork _unitofwork;

        public Project_Service(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _unitofwork.ProjectRepo.GetAllWithDetailsAsync();
            var projectDtos = new List<ProjectDto>();

            foreach (var project in projects)
            {
                var dto = await MapToProjectDto(project);
                projectDtos.Add(dto);
            }

            return projectDtos;
        }

        public async Task<IEnumerable<ProjectListDto>> GetProjectsListAsync()
        {
            var projects = await _unitofwork.ProjectRepo.GetAllWithDetailsAsync();
            return await MapToProjectListDto(projects);
        }

        public async Task<ProjectDto> GetProjectByIdAsync(long id)
        {
            var project = await _unitofwork.ProjectRepo.GetByIdWithDetailsAsync(id);
            if (project == null) return null;

            return await MapToProjectDto(project);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, Guid? createdById)
        {
            var city = await _unitofwork.CityRepo.GetByIdAsync((int)createDto.CityId);
            if (city == null)
                throw new ArgumentException("المدينة المحددة غير موجودة");

            if (createDto.DistrictId.HasValue)
            {
                var district = await _unitofwork.DistrictRepo.GetByIdAsync((int)createDto.DistrictId.Value);
                if (district == null)
                    throw new ArgumentException("الحي المحدد غير موجود");
            }

            var project = new Project
            {
                Name = createDto.Name,
                LogoUrl = createDto.LogoUrl,
                CoverImageUrl = createDto.CoverImageUrl,
                CityId = createDto.CityId,
                DistrictId = createDto.DistrictId,
                LocationDescription = createDto.LocationDescription,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.Now,
                CreatedById = createdById,
                IsDeleted = false
            };

            var createdProject = await _unitofwork.ProjectRepo.AddAsync(project);
            await _unitofwork.Save();

            return await GetProjectByIdAsync(createdProject.Id);
        }

        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updateDto, Guid? updatedById)
        {
            var project = await _unitofwork.ProjectRepo.GetByIdAsync((int)updateDto.Id);
            if (project == null)
                throw new ArgumentException("المشروع غير موجود");

            var city = await _unitofwork.CityRepo.GetByIdAsync((int)updateDto.CityId);
            if (city == null)
                throw new ArgumentException("المدينة المحددة غير موجودة");

            if (updateDto.DistrictId.HasValue)
            {
                var district = await _unitofwork.DistrictRepo.GetByIdAsync((int)updateDto.DistrictId.Value);
                if (district == null)
                    throw new ArgumentException("الحي المحدد غير موجود");
            }

            project.Name = updateDto.Name;
            project.LogoUrl = updateDto.LogoUrl;
            project.CoverImageUrl = updateDto.CoverImageUrl;
            project.CityId = updateDto.CityId;
            project.DistrictId = updateDto.DistrictId;
            project.LocationDescription = updateDto.LocationDescription;
            project.IsActive = updateDto.IsActive;
            project.UpdatedAt = DateTime.Now;
            project.UpdatedById = updatedById;

            _unitofwork.ProjectRepo.Update(project);
            await _unitofwork.Save();

            return await GetProjectByIdAsync(project.Id);
        }

        public async Task<bool> DeleteProjectAsync(long id)
        {
            var project = await _unitofwork.ProjectRepo.GetByIdAsync((int)id);
            if (project == null)
                return false;

            var propertiesCount = await _unitofwork.ProjectRepo.GetPropertiesCountAsync(id);
            if (propertiesCount > 0)
                throw new InvalidOperationException($"لا يمكن حذف المشروع لوجود {propertiesCount} وحدة مرتبطة به");

            project.IsDeleted = true;
            project.UpdatedAt = DateTime.Now;

            _unitofwork.ProjectRepo.Update(project);
            await _unitofwork.Save();

            return true;
        }

        public async Task<bool> ToggleProjectStatusAsync(long id)
        {
            var project = await _unitofwork.ProjectRepo.GetByIdAsync((int)id);
            if (project == null)
                return false;

            project.IsActive = !project.IsActive;
            project.UpdatedAt = DateTime.Now;

            _unitofwork.ProjectRepo.Update(project);
            await _unitofwork.Save();

            return true;
        }

        public async Task<IEnumerable<ProjectListDto>> SearchProjectsAsync(string searchTerm)
        {
            var projects = await _unitofwork.ProjectRepo.SearchByNameAsync(searchTerm);
            return await MapToProjectListDto(projects);
        }

        public async Task<IEnumerable<ProjectListDto>> GetProjectsByCityAsync(long cityId)
        {
            var projects = await _unitofwork.ProjectRepo.GetByCityIdAsync(cityId);
            return await MapToProjectListDto(projects);
        }

        public async Task<IEnumerable<ProjectListDto>> GetActiveProjectsAsync()
        {
            var projects = await _unitofwork.ProjectRepo.GetActiveProjectsAsync();
            return await MapToProjectListDto(projects);
        }

        /// <summary>
        /// فلترة المشاريع حسب النوع المحسوب (بدون تعديل Database)
        /// </summary>
        public async Task<IEnumerable<ProjectListDto>> GetProjectsByComputedTypeAsync(ProjectTypeEnum type)
        {
            // جلب كل المشاريع أولاً
            var allProjects = await GetProjectsListAsync();

            // فلترة حسب النوع المحسوب في الـ DTO
            return allProjects.Where(p => p.ComputedType == type).ToList();
        }

        public async Task<ProjectStatsDto> GetProjectStatsAsync()
        {
            var allProjects = await _unitofwork.ProjectRepo.GetAllAsync();
            var activeProjects = await _unitofwork.ProjectRepo.GetActiveProjectsAsync();

            // حساب قيد الإنشاء من الـ DTOs
            var allProjectsList = await GetProjectsListAsync();
            var underConstruction = allProjectsList.Count(p => p.ComputedType == ProjectTypeEnum.UnderConstruction);

            var allProjectsArray = allProjects.ToList();
            int totalUnits = 0;
            foreach (var project in allProjectsArray)
            {
                totalUnits += await _unitofwork.ProjectRepo.GetPropertiesCountAsync(project.Id);
            }

            return new ProjectStatsDto
            {
                TotalProjects = allProjectsArray.Count,
                ActiveProjects = activeProjects.Count(),
                UnderConstruction = underConstruction,
                TotalUnits = totalUnits
            };
        }

        // ==================== Helper Methods ====================

        private async Task<ProjectDto> MapToProjectDto(Project project)
        {
            var propertiesCount = await _unitofwork.ProjectRepo.GetPropertiesCountAsync(project.Id);
            var minPrice = await _unitofwork.ProjectRepo.GetMinPriceAsync(project.Id);

            int? minArea = null;
            int? maxArea = null;

            if (project.Properties != null && project.Properties.Any())
            {
                var activeProperties = project.Properties.Where(p => !p.IsDeleted).ToList();
                if (activeProperties.Any())
                {
                    minArea = activeProperties.Min(p => p.Area);
                    maxArea = activeProperties.Max(p => p.Area);
                }
            }

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                LogoUrl = project.LogoUrl,
                CoverImageUrl = project.CoverImageUrl,
                CityId = project.CityId,
                CityName = project.City?.Name ?? "غير محدد",
                DistrictId = project.DistrictId,
                DistrictName = project.District?.Name,
                LocationDescription = project.LocationDescription,
                IsActive = project.IsActive,
                PropertiesCount = propertiesCount,
                MinPrice = minPrice,
                MinArea = minArea,
                MaxArea = maxArea,
                CreatedAt = project.CreatedAt
            };
        }

        private async Task<List<ProjectListDto>> MapToProjectListDto(IEnumerable<Project> projects)
        {
            var listDtos = new List<ProjectListDto>();

            foreach (var project in projects)
            {
                var propertiesCount = await _unitofwork.ProjectRepo.GetPropertiesCountAsync(project.Id);
                var minPrice = await _unitofwork.ProjectRepo.GetMinPriceAsync(project.Id);

                listDtos.Add(new ProjectListDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    LogoUrl = project.LogoUrl,
                    CityName = project.City?.Name ?? "غير محدد",
                    DistrictName = project.District?.Name,
                    LocationDescription = project.LocationDescription,
                    IsActive = project.IsActive,
                    PropertiesCount = propertiesCount,
                    MinPrice = minPrice
                });
            }

            return listDtos;
        }
    }
}