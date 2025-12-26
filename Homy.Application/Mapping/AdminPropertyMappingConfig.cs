using Homy.Application.Dtos.Admin;
using Homy.Domin.models;
using Mapster;
using System.Linq;

namespace Homy.Application.Mapping
{
    public static class AdminPropertyMappingConfig
    {
        public static void Configure()
        {
            // Property -> PropertyListDto
            TypeAdapterConfig<Property, PropertyListDto>.NewConfig()
                .Map(dest => dest.ImageUrl, src => src.Images.FirstOrDefault() != null ? src.Images.FirstOrDefault().ImageUrl : null)
                .Map(dest => dest.PropertyTypeName, src => src.PropertyType != null ? src.PropertyType.Name : "")
                .Map(dest => dest.CityName, src => src.City != null ? src.City.Name : "")
                .Map(dest => dest.DistrictName, src => src.District != null ? src.District.Name : null)
                .Map(dest => dest.OwnerName, src => src.User.FullName)
                .Map(dest => dest.OwnerImage, src => src.User.ProfileImageUrl);

            // Property -> PropertyDetailsDto
            TypeAdapterConfig<Property, PropertyDetailsDto>.NewConfig()
                .Map(dest => dest.ImageUrl, src => src.Images.FirstOrDefault() != null ? src.Images.FirstOrDefault().ImageUrl : null)
                .Map(dest => dest.PropertyTypeName, src => src.PropertyType != null ? src.PropertyType.Name : "")
                .Map(dest => dest.PropertyTypeNameEn, src => src.PropertyType != null ? src.PropertyType.NameEn : "")
                .Map(dest => dest.CityName, src => src.City != null ? src.City.Name : "")
                .Map(dest => dest.CityNameEn, src => src.City != null ? src.City.NameEn : "")
                .Map(dest => dest.DistrictName, src => src.District != null ? src.District.Name : null)
                .Map(dest => dest.DistrictNameEn, src => src.District != null ? src.District.NameEn : null)
                .Map(dest => dest.OwnerName, src => src.User.FullName)
                .Map(dest => dest.OwnerImage, src => src.User.ProfileImageUrl)
                .Map(dest => dest.Images, src => src.Images.Select(i => i.ImageUrl).ToList())
                .Map(dest => dest.Amenities, src => src.PropertyAmenities.Select(pa => pa.Amenity.Name).ToList())
                .Map(dest => dest.OwnerPhone, src => src.User.PhoneNumber)
                .Map(dest => dest.OwnerWhatsApp, src => src.User.WhatsAppNumber)
                .Map(dest => dest.TitleEn, src => src.TitleEn)
                .Map(dest => dest.DescriptionEn, src => src.DescriptionEn)
                .Map(dest => dest.AddressDetailsEn, src => src.AddressDetailsEn);

            // PropertyReview -> PropertyReviewHistoryDto
            TypeAdapterConfig<PropertyReview, PropertyReviewHistoryDto>.NewConfig()
                .Map(dest => dest.AdminName, src => src.Admin != null ? src.Admin.FullName : "Unknown")
                .Map(dest => dest.ReviewedAt, src => src.CreatedAt);
        }
    }
}
