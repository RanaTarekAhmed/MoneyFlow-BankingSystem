using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneyFlow.Business.Services;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Data.Database;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories;
using MoneyFlow.Data.Repositories.Interfaces;
using MoneyFlow.Presentation.Data;


namespace MoneyFlow.Presentation
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Configure database connection and register DbContext
			var connectionString =
				builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException(
					"Connection string 'DefaultConnection' was not found.");

			builder.Services.AddDbContext<MoneyFlowDbContext>(options =>
				options.UseSqlServer(connectionString));

            // Configure ASP.NET Core Identity and connect it to the database.
            builder.Services
				.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<MoneyFlowDbContext>()
				.AddDefaultTokenProviders();

			// Business Services
			builder.Services.AddScoped<IAuthService, AuthService>();

			// Repositories
			builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

			var app = builder.Build();

            // Seed default Identity roles during application startup.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await RoleSeeder.SeedAsync(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapStaticAssets();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}")
				.WithStaticAssets();

            await app.RunAsync();
        }
	}
}
