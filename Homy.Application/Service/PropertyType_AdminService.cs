using Homy.Application.Dtos.Admin;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service
{
    public class PropertyType_AdminService : IPropertyType_AdminService
    {
        private readonly IUnitofwork _unitOfWork;

        public PropertyType_AdminService(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PropertyTypeDto>> GetAllAsync()
        {
            var types = await _unitOfWork.PropertyTypeRepo.GetAllAsync();
            
            // Load properties count manually since GetAllAsync returns IEnumerable
            var typeDtos = new List<PropertyTypeDto>();
            foreach (var pt in types)
            {
                var propertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                    .Where(p => p.PropertyTypeId == pt.Id)
                    .CountAsync();
                
                // Mapster Mapping
                var dto = pt.Adapt<PropertyTypeDto>();
                dto.PropertiesCount = propertiesCount;
                typeDtos.Add(dto);
            }
            
            return typeDtos;
        }

        public async Task<PropertyTypeDto?> GetByIdAsync(long id)
        {
            var pt = await _unitOfWork.PropertyTypeRepo.GetByIdAsync((long)id);
            if (pt == null) return null;

            // Mapster Mapping
            return pt.Adapt<PropertyTypeDto>();
        }

        public async Task<PropertyTypeDto> CreateAsync(CreatePropertyTypeDto dto)
        {
            // Mapster Mapping (DTO to Entity)
            var propertyType = dto.Adapt<PropertyType>();
            propertyType.CreatedAt = DateTime.Now;

            await _unitOfWork.PropertyTypeRepo.AddAsync(propertyType);
            await _unitOfWork.Save();

            // Mapster Mapping (Entity to DTO)
            return propertyType.Adapt<PropertyTypeDto>();
        }

        public async Task<bool> UpdateAsync(UpdatePropertyTypeDto dto)
        {
            var propertyType = await _unitOfWork.PropertyTypeRepo.GetByIdAsync((int)dto.Id);
            if (propertyType == null) return false;

            // Mapster Mapping (Update existing entity)
            dto.Adapt(propertyType);
            propertyType.UpdatedAt = DateTime.Now;

            _unitOfWork.PropertyTypeRepo.Update(propertyType);
            await _unitOfWork.Save();
            return true;
        }

        public async Task<(bool Success, string Message)> DeleteAsync(long id)
        {
            var propertyType = await _unitOfWork.PropertyTypeRepo.GetByIdAsync((int)id);
            
            if (propertyType == null) 
                return (false, "النوع غير موجود");

            // Check if there are properties with this type
            var propertiesCount = await _unitOfWork.PropertyRepo.GetAll()
                .Where(p => p.PropertyTypeId == id)
                .CountAsync();
                
            if (propertiesCount > 0)
                return (false, "لا يمكن حذف هذا النوع لوجود إعلانات مرتبطة به");

            await _unitOfWork.PropertyTypeRepo.DeleteAsync((int)id);
            await _unitOfWork.Save();
            return (true, "تم الحذف بنجاح");
        }
    }
}
