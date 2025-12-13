using Microsoft.AspNetCore.Identity;
using Homy.Domin.models;
using Microsoft.EntityFrameworkCore;
using Homy.Infurastructure.Data;
using Homy.Infurastructure.Repository;
using Homy.Domin.Contract_Repo;
namespace Homy.presentaion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<HomyContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDbContext<HomyContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            builder.Services.AddScoped(typeof(IGenric_Repo<>), typeof(Genric_Repo<>));
            builder.Services.AddScoped<IProperty_Repo, Property_Repo>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
