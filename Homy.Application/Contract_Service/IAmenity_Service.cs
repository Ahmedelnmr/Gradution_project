namespace Homy.Domin.Contract_Service
{
    public interface IAmenity_Service
    {
        Task<IEnumerable<Homy.Domin.models.Amenity>> GetAllAmenitiesAsync();
        Task<Homy.Domin.models.Amenity> GetAmenityByIdAsync(int id);
        
        // CRUD
        Task<Homy.Domin.models.Amenity> CreateAmenityAsync(string name, string? nameEn, string? iconUrl = null);
        Task<Homy.Domin.models.Amenity> UpdateAmenityAsync(int id, string name, string? nameEn, string? iconUrl = null);
        Task<bool> DeleteAmenityAsync(int id);
    }
}   