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
        }
    }
}
