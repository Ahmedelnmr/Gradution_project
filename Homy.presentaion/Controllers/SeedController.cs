using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homy.presentaion.Controllers
{
    public class SeedController : Controller
    {
        private readonly HomyContext _context;
        private readonly UserManager<User> _userManager;

        public SeedController(HomyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                PropertyTypes = await _context.PropertyTypes.CountAsync(),
                Cities = await _context.Cities.CountAsync(),
                Districts = await _context.Districts.CountAsync(),
                Users = await _context.Users.CountAsync(),
                Properties = await _context.Properties.CountAsync(),
                Reviews = await _context.PropertyReviews.CountAsync(),
                Projects = await _context.Projects.CountAsync()
            };
            return View(stats);
        }

        [HttpPost]
        public async Task<IActionResult> SeedArabicData()
        {
            try
            {
                // 1. Users
                var adminUser = await EnsureUserAsync("admin@homy.com", "أحمد محمد الإداري", "01000000001", UserRole.Admin);
                var ownerUser = await EnsureUserAsync("owner@homy.com", "محمود سعيد العقاري", "01100000002", UserRole.Owner);
                var agentUser = await EnsureUserAsync("agent@homy.com", "مريم عبدالله الوكيل", "01200000003", UserRole.Agent);

                // 2. Property Types
                if (!await _context.PropertyTypes.AnyAsync())
                {
                    var types = new List<PropertyType>
                    {
                        new PropertyType { Name = "شقة", IconUrl = "/icons/apartment.svg" },
                        new PropertyType { Name = "فيلا", IconUrl = "/icons/villa.svg" },
                        new PropertyType { Name = "دوبلكس", IconUrl = "/icons/duplex.svg" },
                        new PropertyType { Name = "محل تجاري", IconUrl = "/icons/shop.svg" },
                        new PropertyType { Name = "مكتب", IconUrl = "/icons/office.svg" },
                        new PropertyType { Name = "أرض", IconUrl = "/icons/land.svg" },
                        new PropertyType { Name = "بنتهاوس", IconUrl = "/icons/penthouse.svg" }
                    };
                    await _context.PropertyTypes.AddRangeAsync(types);
                    await _context.SaveChangesAsync();
                }

                // 3. Amenities
                if (!await _context.Amenities.AnyAsync())
                {
                    var amenities = new List<Amenity>
                    {
                        new Amenity { Name = "مصعد", IconUrl = "/icons/elevator.svg" },
                        new Amenity { Name = "جراج", IconUrl = "/icons/garage.svg" },
                        new Amenity { Name = "حمام سباحة", IconUrl = "/icons/pool.svg" },
                        new Amenity { Name = "حديقة", IconUrl = "/icons/garden.svg" },
                        new Amenity { Name = "أمن وحراسة", IconUrl = "/icons/security.svg" },
                        new Amenity { Name = "تكييف مركزي", IconUrl = "/icons/ac.svg" },
                        new Amenity { Name = "إنترنت", IconUrl = "/icons/wifi.svg" },
                        new Amenity { Name = "غرفة بواب", IconUrl = "/icons/doorman.svg" },
                        new Amenity { Name = "جيم", IconUrl = "/icons/gym.svg" },
                        new Amenity { Name = "ملاعب", IconUrl = "/icons/playground.svg" }
                    };
                    await _context.Amenities.AddRangeAsync(amenities);
                    await _context.SaveChangesAsync();
                }

                // 4. Cities & Districts
                if (!await _context.Cities.AnyAsync())
                {
                    var cairo = new City { Name = "القاهرة" };
                    var alex = new City { Name = "الإسكندرية" };
                    var giza = new City { Name = "الجيزة" };
                    
                    await _context.Cities.AddRangeAsync(cairo, alex, giza);
                    await _context.SaveChangesAsync();

                    var districts = new List<District>
                    {
                        // Cairo
                        new District { Name = "مصر الجديدة", CityId = cairo.Id },
                        new District { Name = "المعادي", CityId = cairo.Id },
                        new District { Name = "مدينة نصر", CityId = cairo.Id },
                        new District { Name = "التجمع الخامس", CityId = cairo.Id },
                        new District { Name = "الرحاب", CityId = cairo.Id },
                        // Alex
                        new District { Name = "سموحة", CityId = alex.Id },
                        new District { Name = "المندرة", CityId = alex.Id },
                        // Giza
                        new District { Name = "المهندسين", CityId = giza.Id },
                        new District { Name = "الدقي", CityId = giza.Id },
                        new District { Name = "6 أكتوبر", CityId = giza.Id },
                        new District { Name = "الشيخ زايد", CityId = giza.Id }
                    };
                    await _context.Districts.AddRangeAsync(districts);
                    await _context.SaveChangesAsync();
                }

                // 5. Packages
                if (!await _context.Packages.AnyAsync())
                {
                    var packages = new List<Package>
                    {
                        new Package { Name = "الباقة المجانية", Price = 0, DurationDays = 30, MaxProperties = 2, MaxFeatured = 0, CanBumpUp = false },
                        new Package { Name = "الباقة الأساسية", Price = 299, DurationDays = 30, MaxProperties = 10, MaxFeatured = 2, CanBumpUp = true },
                        new Package { Name = "الباقة المميزة", Price = 599, DurationDays = 30, MaxProperties = 25, MaxFeatured = 5, CanBumpUp = true },
                        new Package { Name = "الباقة الذهبية", Price = 999, DurationDays = 30, MaxProperties = 50, MaxFeatured = 10, CanBumpUp = true }
                    };
                    await _context.Packages.AddRangeAsync(packages);
                    await _context.SaveChangesAsync();
                }

                // 6. Projects
                if (!await _context.Projects.AnyAsync())
                {
                    var cairo = await _context.Cities.FirstAsync(c => c.Name == "القاهرة");
                    var giza = await _context.Cities.FirstAsync(c => c.Name == "الجيزة");
                    var rehabDistrict = await _context.Districts.FirstAsync(d => d.Name == "الرحاب");
                    var zayedDistrict = await _context.Districts.FirstAsync(d => d.Name == "الشيخ زايد");

                    var projects = new List<Project>
                    {
                        new Project 
                        { 
                            Name = "كمبوند الرحاب", 
                            LogoUrl = "/projects/rehab-logo.jpg", 
                            CoverImageUrl = "/projects/rehab-cover.jpg",
                            CityId = cairo.Id,
                            DistrictId = rehabDistrict.Id,
                            LocationDescription = "مدينة الرحاب - القاهرة الجديدة",
                            IsActive = true
                        },
                        new Project 
                        { 
                            Name = "كمبوند بيفرلي هيلز", 
                            LogoUrl = "/projects/beverly-logo.jpg", 
                            CoverImageUrl = "/projects/beverly-cover.jpg",
                            CityId = giza.Id,
                            DistrictId = zayedDistrict.Id,
                            LocationDescription = "الشيخ زايد - الجيزة",
                            IsActive = true
                        },
                        new Project 
                        { 
                            Name = "مشروع العاصمة الإدارية", 
                            LogoUrl = "/projects/capital-logo.jpg", 
                            CoverImageUrl = "/projects/capital-cover.jpg",
                            CityId = cairo.Id,
                            DistrictId = null,
                            LocationDescription = "العاصمة الإدارية الجديدة",
                            IsActive = true
                        }
                    };
                    await _context.Projects.AddRangeAsync(projects);
                    await _context.SaveChangesAsync();
                }

                // 7. Properties (Sample)
                if (!await _context.Properties.AnyAsync())
                {
                    var aptType = await _context.PropertyTypes.FirstAsync(t => t.Name == "شقة");
                    var villaType = await _context.PropertyTypes.FirstAsync(t => t.Name == "فيلا");
                    var nasrCity = await _context.Districts.FirstAsync(d => d.Name == "مدينة نصر");
                    var tagamoa = await _context.Districts.FirstAsync(d => d.Name == "التجمع الخامس");
                    
                    var props = new List<Property>
                    {
                        new Property
                        {
                            Title = "شقة فاخرة للبيع في مدينة نصر",
                            Description = "شقة 200 متر تشطيب سوبر لوكس في موقع متميز بمدينة نصر. 3 غرف نوم و 2 حمام.",
                            Price = 3500000,
                            Area = 200,
                            Rooms = 3,
                            Bathrooms = 2,
                            FloorNumber = 5,
                            AddressDetails = "شارع عباس العقاد - مدينة نصر",
                            PropertyTypeId = aptType.Id,
                            CityId = nasrCity.CityId,
                            DistrictId = nasrCity.Id,
                            UserId = ownerUser.Id,
                            Status = PropertyStatus.Active,
                            IsFeatured = true,
                            FeaturedUntil = DateTime.UtcNow.AddDays(15),
                            CreatedAt = DateTime.UtcNow
                        },
                        new Property
                        {
                            Title = "فيلا مستقلة في التجمع الخامس",
                            Description = "فيلا رائعة في التجمع الخامس، حديقة خاصة وحمام سباحة. تصميم حديث وموقع هادئ.",
                            Price = 12000000,
                            Area = 450,
                            Rooms = 5,
                            Bathrooms = 4,
                            FloorNumber = 2,
                            AddressDetails = "الحي الأول - التجمع الخامس",
                            PropertyTypeId = villaType.Id,
                            CityId = tagamoa.CityId,
                            DistrictId = tagamoa.Id,
                            UserId = ownerUser.Id,
                            Status = PropertyStatus.Active,
                            IsFeatured = true,
                            FeaturedUntil = DateTime.UtcNow.AddDays(30),
                            CreatedAt = DateTime.UtcNow
                        }
                    };
                    await _context.Properties.AddRangeAsync(props);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content($"Error seeding data: {ex.Message} \n {ex.StackTrace}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                // Delete in order to respect FK constraints
                
                // 1. Child tables first
                _context.PropertyReviews.RemoveRange(_context.PropertyReviews);
                _context.SavedProperties.RemoveRange(_context.SavedProperties);
                _context.PropertyAmenities.RemoveRange(_context.PropertyAmenities);
                _context.PropertyImages.RemoveRange(_context.PropertyImages);
                _context.UserSubscriptions.RemoveRange(_context.UserSubscriptions);
                _context.Notifications.RemoveRange(_context.Notifications);
                
                await _context.SaveChangesAsync();

                // 2. Properties
                _context.Properties.RemoveRange(_context.Properties);
                await _context.SaveChangesAsync();

                // 3. Projects
                _context.Projects.RemoveRange(_context.Projects);
                await _context.SaveChangesAsync();

                // 4. Districts
                _context.Districts.RemoveRange(_context.Districts);
                await _context.SaveChangesAsync();

                // 5. Cities
                _context.Cities.RemoveRange(_context.Cities);
                await _context.SaveChangesAsync();

                // 6. Property Types & Amenities & Packages
                _context.PropertyTypes.RemoveRange(_context.PropertyTypes);
                _context.Amenities.RemoveRange(_context.Amenities);
                _context.Packages.RemoveRange(_context.Packages);
                await _context.SaveChangesAsync();

                // 7. Users (optional, but requested to clean bad data)
                // CAUTION: This deletes all users including current logged in one if not careful.
                // We keep the current user if possible, or just delete specific roles. 
                // For a full reset, we delete all non-system users if needed.
                // Here we will delete our seeded users or all users.
                // Let's delete everyone except maybe the one running this? No, "Seed" usually implies full reset.
                var users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                {
                    await _userManager.DeleteAsync(user);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // If FK error, try raw sql or enhanced ordering
                return Content($"Error clearing data: {ex.Message}");
            }
        }

        private async Task<User> EnsureUserAsync(string email, string name, string phone, UserRole role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    EmailConfirmed = true,
                    Role = role,
                    PhoneNumber = phone,
                    PhoneNumberConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, "Password@123");
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            return user;
        }
    }
}
