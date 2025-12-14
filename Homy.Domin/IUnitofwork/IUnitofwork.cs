using System;
using System.Threading.Tasks;
using Homy.Domin.Contract_Repo;

namespace Homy.Infurastructure.Unitofworks
{
    public interface IUnitofwork : IDisposable
    {
        ICity_Repo CityRepo { get; }
        IDistrict_Repo DistrictRepo { get; }
        IPropertyType_Repo PropertyTypeRepo { get; }
        IAmenity_Repo AmenityRepo { get; }
        IProperty_Repo PropertyRepo { get; }
        IPropertyImage_Repo PropertyImageRepo { get; }
        IPropertyAmenity_Repo PropertyAmenityRepo { get; }
        ISavedProperty_Repo SavedPropertyRepo { get; }
        IPackage_Repo PackageRepo { get; }
        IUserSubscription_Repo UserSubscriptionRepo { get; }
        IProject_Repo ProjectRepo { get; }
        IUserRepo UserRepo { get; }

        Task Save();
    }
}
