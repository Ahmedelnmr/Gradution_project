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
                Reviews = await _context.PropertyReviews.CountAsync()
            };
            return View(stats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateData()
        {
            try
            {
                // 1. Property Types
                if (!await _context.PropertyTypes.AnyAsync())
                {
                    var types = new List<PropertyType>
                    {
                        new PropertyType { Name = "Seed_Apartment", IconUrl = "https://cdn-icons-png.flaticon.com/512/2942/2942921.png" },
                        new PropertyType { Name = "Seed_Villa", IconUrl = "https://cdn-icons-png.flaticon.com/512/2169/2169335.png" },
                        new PropertyType { Name = "Seed_Land", IconUrl = "https://cdn-icons-png.flaticon.com/512/8287/8287948.png" },
                        new PropertyType { Name = "Seed_Office", IconUrl = "https://cdn-icons-png.flaticon.com/512/2942/2942200.png" }
                    };
                    await _context.PropertyTypes.AddRangeAsync(types);
                    await _context.SaveChangesAsync();
                }

                // 2. Cities & Districts
                if (!await _context.Cities.AnyAsync())
                {
                    var city = new City { Name = "Seed_Cairo" };
                    await _context.Cities.AddAsync(city);
                    await _context.SaveChangesAsync();

                    var districts = new List<District>
                    {
                        new District { Name = "Seed_Nasr City", CityId = city.Id },
                        new District { Name = "Seed_Maadi", CityId = city.Id }
                    };
                    await _context.Districts.AddRangeAsync(districts);
                    await _context.SaveChangesAsync();
                }

                // 3. Fake Users (Owners)
                var ownerEmail = "seed_owner@test.com";
                var ownerUser = await _userManager.FindByEmailAsync(ownerEmail);
                if (ownerUser == null)
                {
                    ownerUser = new User
                    {
                        UserName = ownerEmail,
                        Email = ownerEmail,
                        FullName = "Seed Owner",
                        EmailConfirmed = true,
                        Role = UserRole.Owner,
                        PhoneNumber = "01000000000"
                    };
                    await _userManager.CreateAsync(ownerUser, "Password@123");
                }

                // 4. Properties
                if (!await _context.Properties.AnyAsync(p => p.Title.StartsWith("Seed_")))
                {
                    var typeId = (await _context.PropertyTypes.FirstAsync()).Id;
                    var cityId = (await _context.Cities.FirstAsync()).Id;
                    var districtId = (await _context.Districts.FirstAsync()).Id;
                    var rnd = new Random();

                    var properties = new List<Property>();
                    for (int i = 1; i <= 20; i++)
                    {
                        var status = (PropertyStatus)(rnd.Next(1, 4)); // 1=Active, 2=Sold, 3=PendingReview (assuming logic)
                        // Adjusting status to match your enum: 1=Active, 2=SoldOrRented, 3=Hidden? 
                        // Let's check enum in Property.cs
                        // Actually in your system: PendingReview might be 0 or handled differently?
                        // Based on walkthrough: PendingReview, Active, Rejected.
                        // Let's assume standard Enum values. If not, default to Active.
                        
                        properties.Add(new Property
                        {
                            Title = $"Seed_Property {i}",
                            Description = "This is a fake property generated for testing purposes.",
                            Price = rnd.Next(1000000, 5000000),
                            Area = rnd.Next(80, 500),
                            Rooms = (byte)rnd.Next(2, 6),
                            Bathrooms = (byte)rnd.Next(1, 4),
                            FloorNumber = (byte)rnd.Next(1, 10),
                            // Fix: Use AddressDetails instead of Address
                            AddressDetails = "Fake Address St. " + i,
                            PropertyTypeId = typeId,
                            CityId = cityId,
                            DistrictId = districtId,
                            UserId = ownerUser.Id,
                            // Cycle through statuses: Pending, Active, Rejected
                            Status = (i % 3 == 0) ? PropertyStatus.PendingReview : 
                                     (i % 3 == 1) ? PropertyStatus.Active : 
                                     PropertyStatus.Rejected
                        });
                    }
                    
                    await _context.Properties.AddRangeAsync(properties);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message} \n {ex.StackTrace}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearData()
        {
            try
            {
                var properties = await _context.Properties.Where(p => p.Title.StartsWith("Seed_")).ToListAsync();
                _context.Properties.RemoveRange(properties);

                var types = await _context.PropertyTypes.Where(p => p.Name.StartsWith("Seed_")).ToListAsync();
                _context.PropertyTypes.RemoveRange(types);

                var districts = await _context.Districts.Where(d => d.Name.StartsWith("Seed_")).ToListAsync();
                _context.Districts.RemoveRange(districts);

                var cities = await _context.Cities.Where(c => c.Name.StartsWith("Seed_")).ToListAsync();
                _context.Cities.RemoveRange(cities);

                var user = await _userManager.FindByEmailAsync("seed_owner@test.com");
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }
    }
}
