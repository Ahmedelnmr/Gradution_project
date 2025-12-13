using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Homy.Infurastructure.Repository;
using Homy.Infurastructure.Service;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Homy.presentaion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========== Services Configuration ==========

            // Add MVC Controllers with Views
            builder.Services.AddControllersWithViews();

            // Configure Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<HomyContext>()
                .AddDefaultTokenProviders();

            // Configure Database Context
            builder.Services.AddDbContext<HomyContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // ========== Register Repositories ==========

            // Generic Repository
            builder.Services.AddScoped(typeof(IGenric_Repo<>), typeof(Genric_Repo<>));

            // Specific Repositories
            builder.Services.AddScoped<ICity_Repo, City_Repo>();
            builder.Services.AddScoped<IDistrict_Repo, District_Repo>();
            builder.Services.AddScoped<IProperty_Repo, Property_Repo>();
            builder.Services.AddScoped<IPropertyType_Repo, PropertyType_Repo>();
            builder.Services.AddScoped<IAmenity_Repo, Amenity_Repo>();
            builder.Services.AddScoped<IPropertyImage_Repo, PropertyImage_Repo>();
            builder.Services.AddScoped<IPropertyAmenity_Repo, PropertyAmenity_Repo>();
            builder.Services.AddScoped<ISavedProperty_Repo, SavedProperty_Repo>();
            builder.Services.AddScoped<IPackage_Repo, Package_Repo>();
            builder.Services.AddScoped<IUserSubscription_Repo, UserSubscription_Repo>();
            builder.Services.AddScoped<IProject_Repo, Project_Repo>(); // ✅ المشاريع

            // ========== Register Unit of Work ==========
            builder.Services.AddScoped<IUnitofwork, Unitofwork>();

            // ========== Register Services ==========

            builder.Services.AddScoped<ICity_Service, City_Service>();
            builder.Services.AddScoped<IDistrict_Service, District_Service>();
            builder.Services.AddScoped<IProperty_Service, Property_Service>();
            builder.Services.AddScoped<IPropertyType_Service, PropertyType_Service>();
            builder.Services.AddScoped<IAmenity_Service, Amenity_Service>();
            builder.Services.AddScoped<IPropertyImage_Service, PropertyImage_Service>();
            builder.Services.AddScoped<IPropertyAmenity_Service, PropertyAmenity_Service>();
            builder.Services.AddScoped<ISavedProperty_Service, SavedProperty_Service>();
            builder.Services.AddScoped<IPackage_Service, Package_Service>();
            //builder.Services.AddScoped<IUserSubscription_Service, IUserSubscription_Service>();
            builder.Services.AddScoped<IProject_Service, Project_Service>(); // ✅ خدمة المشاريع

            // ========== Build Application ==========
            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // ✅ مهم للـ Identity
            app.UseAuthorization();

            // Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
