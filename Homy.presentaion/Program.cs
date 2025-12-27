using Homy.Application.Contract_Service;
using Homy.Application.Service;
using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;

using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Homy.Infurastructure.Repository;
using Homy.Infurastructure.Service;
using Homy.Infurastructure.Unitofworks;
using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Service.ApiServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Homy.presentaion
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========== Configure Console Encoding for Arabic ==========
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // ========== Services Configuration ==========

            // Add MVC Controllers with Views
            builder.Services.AddControllersWithViews();

            // Configure Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<HomyContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/Error";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            // Configure DbContext
            builder.Services.AddDbContext<HomyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==================== Register Repositories ====================
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
            builder.Services.AddScoped<IProject_Repo, Project_Repo>();
            builder.Services.AddScoped<IReports_Repo, Reports_Repo>();
            builder.Services.AddScoped<IPropertyReview_Repo, PropertyReview_Repo>();
            builder.Services.AddScoped<INotification_Repo, Notification_Repo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();

            // ==================== Register Unit of Work ====================
            builder.Services.AddScoped<IUnitofwork, Unitofwork>();


            // ==================== Register Services ====================
            builder.Services.AddScoped<ICity_Service, City_Service>();
            builder.Services.AddScoped<IDistrict_Service, District_Service>();
            builder.Services.AddScoped<IProperty_Service, Property_Service>();
            builder.Services.AddScoped<IPropertyType_Service, PropertyType_Service>();
            builder.Services.AddScoped<IAmenity_Service, Amenity_Service>();
            builder.Services.AddScoped<IPropertyImage_Service, PropertyImage_Service>();
            builder.Services.AddScoped<IPropertyAmenity_Service, PropertyAmenity_Service>();
            builder.Services.AddScoped<ISavedProperty_Service, SavedProperty_Service>();
            builder.Services.AddScoped<IPackage_Service, Package_Service>();
            builder.Services.AddScoped<IUserSubscription_Service, UserSubscription_Service>();
            builder.Services.AddScoped<IProject_Service, Project_Service>();
            builder.Services.AddScoped<IReports_Service, Reports_Service>();
            builder.Services.AddScoped<IUser_Service, User_Service>();
            
            // Dashboard & Admin Services
            builder.Services.AddScoped<IDashboard_Service, Dashboard_Service>();
            builder.Services.AddScoped<IAdminProperty_Service, AdminProperty_Service>();
            builder.Services.AddScoped<IPropertyType_AdminService, PropertyType_AdminService>();
            builder.Services.AddScoped<IPropertyReview_Service, PropertyReview_Service>();

            // API Services
            builder.Services.AddScoped<IPropertyApiService, PropertyApiService>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();

            // Register Mapster Mappings
            Homy.Application.Mapping.MappingConfig.RegisterMappings();


            // ========== Build Application ==========
            


var app = builder.Build();

            // Ensure Database Created
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HomyContext>();
                // db.Database.EnsureCreated(); // Only if not using Migrations
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Enable serving files from shared upload directory
            var sharedUploadPath = builder.Configuration["FileUploadSettings:SharedUploadPath"];
            if (!string.IsNullOrEmpty(sharedUploadPath) && System.IO.Directory.Exists(sharedUploadPath))
            {
                app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
                {
                    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(sharedUploadPath),
                    RequestPath = "/uploads"
                });
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}