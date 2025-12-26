using Mapster;

namespace Homy.Application.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Register all mapping configurations
            AdminPropertyMappingConfig.Configure();
            
            // Add more mapping configs here as needed
            // Example: UserMappingConfig.Configure();
            
            // UserSubscription -> UserSubReadDTO
            TypeAdapterConfig<Homy.Domin.models.UserSubscription, Homy.Application.Dtos.UserDtos.UserSubDTOs.UserSubReadDTO>.NewConfig()
                .Map(dest => dest.UserName, src => src.User.FullName)
                .Map(dest => dest.PackageName, src => src.Package.Name);
        }
    }
}
