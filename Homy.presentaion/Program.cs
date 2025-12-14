using Microsoft.AspNetCore.Identity;
using Homy.Domin.models;
using Microsoft.EntityFrameworkCore;
using Homy.Infurastructure.Data;
using Homy.Infurastructure.Repository;
using Homy.Domin.Contract_Repo;
using Homy.Application.Contract_Service;
using Homy.Application.Service;
using Homy.Infurastructure.Unitofworks;

namespace Homy.presentaion
{
    public class Program
    {
        public static void Main(string[] args)
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
            builder.Services.AddScoped<IReports_Repo, Reports_Repo>(); // ← جديد

            // ==================== Register Services ====================
            builder.Services.AddScoped<IReports_Service, Reports_Service>(); // ← جديد

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