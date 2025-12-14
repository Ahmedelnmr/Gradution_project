using Homy.Domin.Contract_Repo;
using Homy.Infurastructure.Data;
using Homy.Infurastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Unitofworks
{
    public class Unitofwork : IUnitofwork
    {
        private readonly HomyContext _homyContext;

        private ICity_Repo _cityRepo;
        private IDistrict_Repo _districtRepo;
        private IPropertyType_Repo _propertyTypeRepo;
        private IAmenity_Repo _amenityRepo;
        private IProperty_Repo _propertyRepo;
        private IPropertyImage_Repo _propertyImageRepo;
        private IPropertyAmenity_Repo _propertyAmenityRepo;
        private ISavedProperty_Repo _savedPropertyRepo;
        private IPackage_Repo _packageRepo;
        private IUserSubscription_Repo _userSubscriptionRepo;
        private IProject_Repo _projectRepo;
<<<<<<< HEAD
        private IPropertyReview_Repo _propertyReviewRepo;
        private INotification_Repo _notificationRepo;
=======
        private IUserRepo _userRepo;
>>>>>>> sharqawy

        public Unitofwork(HomyContext homyContext)
        {
            _homyContext = homyContext;
        }

        public ICity_Repo CityRepo => _cityRepo ??= new City_Repo(_homyContext);
        public IDistrict_Repo DistrictRepo => _districtRepo ??= new District_Repo(_homyContext);
        public IPropertyType_Repo PropertyTypeRepo => _propertyTypeRepo ??= new PropertyType_Repo(_homyContext);
        public IAmenity_Repo AmenityRepo => _amenityRepo ??= new Amenity_Repo(_homyContext);
        public IProperty_Repo PropertyRepo => _propertyRepo ??= new Property_Repo(_homyContext);
        public IPropertyImage_Repo PropertyImageRepo => _propertyImageRepo ??= new PropertyImage_Repo(_homyContext);
        public IPropertyAmenity_Repo PropertyAmenityRepo => _propertyAmenityRepo ??= new PropertyAmenity_Repo(_homyContext);
        public ISavedProperty_Repo SavedPropertyRepo => _savedPropertyRepo ??= new SavedProperty_Repo(_homyContext);
        public IPackage_Repo PackageRepo => _packageRepo ??= new Package_Repo(_homyContext);
        public IUserSubscription_Repo UserSubscriptionRepo => _userSubscriptionRepo ??= new UserSubscription_Repo(_homyContext);
        public IProject_Repo ProjectRepo => _projectRepo ??= new Project_Repo(_homyContext);
<<<<<<< HEAD
        public IPropertyReview_Repo PropertyReviewRepo => _propertyReviewRepo ??= new PropertyReview_Repo(_homyContext);
        public INotification_Repo NotificationRepo => _notificationRepo ??= new Notification_Repo(_homyContext);
=======
        public IUserRepo UserRepo => _userRepo ??= new UserRepo(_homyContext);


>>>>>>> sharqawy

        public async Task Save()
        {
            await _homyContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _homyContext?.Dispose();
        }
    }
}
