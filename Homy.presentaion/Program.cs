using Homy.Application.Contract_Service;
using Homy.Application.Service;
using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Homy.Infurastructure.Repository;
<<<<<<< HEAD
using Homy.Domin.Contract_Repo;
using Homy.Application.Contract_Service;
using Homy.Application.Service;
using Homy.Infurastructure.Unitofworks;

=======
using Homy.Infurastructure.Service;
using Homy.Infurastructure.Unitofworks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
>>>>>>> 0ca8d04ab3305a7cc483404e79ad64412492f101
namespace Homy.presentaion
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<HomyContext>()
                .AddDefaultTokenProviders();

            // Configure DbContext
            builder.Services.AddDbContext<HomyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==================== Register Unit of Work ====================
            builder.Services.AddScoped<IUnitofwork, Unitofwork>();

            // ==================== Register Repositories ====================
            builder.Services.AddScoped<IProperty_Repo, Property_Repo>();
<<<<<<< HEAD
            builder.Services.AddScoped<IReports_Repo, Reports_Repo>(); // ← جديد

            // ==================== Register Services ====================
            builder.Services.AddScoped<IReports_Service, Reports_Service>(); // ← جديد

=======
<<<<<<< HEAD

            // Register UnitOfWork
            builder.Services.AddScoped<Homy.Infurastructure.Unitofworks.IUnitofwork, Homy.Infurastructure.Unitofworks.Unitofwork>();

            // Register Services
            builder.Services.AddScoped<Homy.Domin.Contract_Service.IDashboard_Service, Homy.Infurastructure.Service.Dashboard_Service>();
            builder.Services.AddScoped<Homy.Domin.Contract_Service.IAdminProperty_Service, Homy.Application.Service.AdminProperty_Service>();
            builder.Services.AddScoped<Homy.Domin.Contract_Service.IPropertyType_AdminService, Homy.Application.Service.PropertyType_AdminService>();

            // Register Mapster Mappings
            Homy.Application.Mapping.MappingConfig.RegisterMappings();
=======
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IUnitofwork, Unitofwork>();
            builder.Services.AddScoped<IUser_Service, User_Service>();

>>>>>>> sharqawy
>>>>>>> 0ca8d04ab3305a7cc483404e79ad64412492f101
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HomyContext>();
                db.Database.EnsureCreated();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // مهم للـ Identity
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}