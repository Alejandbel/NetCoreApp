using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using WebLab.IdentityServer.Data;
using WebLab.IdentityServer.Models;

namespace WebLab.IdentityServer
{
	public class SeedData
	{
		public static void EnsureSeedData(WebApplication app)
		{
			using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
			var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
			context.Database.Migrate();

			var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			var adminRole = roleMgr.FindByNameAsync("admin").Result;
			if (adminRole == null)
			{
				roleMgr.CreateAsync(new IdentityRole("admin"));
				Log.Debug("admin role created");
			}
			else
			{
				Log.Debug("admin role already exists");
			}

			var admin = userMgr.FindByNameAsync("admin").Result;
			if (admin == null)
			{
				admin = new ApplicationUser
				{
					UserName = "admin",
					Email = "admin@gmail.com",
					EmailConfirmed = true,
				};
				var result = userMgr.CreateAsync(admin, "Admin123!").Result;
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
				result = userMgr.AddClaimsAsync(admin, new Claim[]{
							new Claim(JwtClaimTypes.Name, "Admin Admin"),
							new Claim(JwtClaimTypes.GivenName, "Admin"),
							new Claim(JwtClaimTypes.FamilyName, "Admin"),
							new Claim(JwtClaimTypes.WebSite, "http://admin.com"),
						}).Result;
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
				Log.Debug("admin created");
			}
			else
			{
				Log.Debug("admin already exists");
			}

			var user = userMgr.FindByNameAsync("user").Result;
			if (user == null)
			{
				user = new ApplicationUser
				{
					UserName = "user",
					Email = "user@gmail.com",
					EmailConfirmed = true
				};
				var result = userMgr.CreateAsync(user, "User123!").Result;
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}

				result = userMgr.AddClaimsAsync(user, new Claim[]{
							new Claim(JwtClaimTypes.Name, "User User"),
							new Claim(JwtClaimTypes.GivenName, "User"),
							new Claim(JwtClaimTypes.FamilyName, "User"),
							new Claim(JwtClaimTypes.WebSite, "http://user.com"),
							new Claim("location", "somewhere")
						}).Result;
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
				Log.Debug("user created");
			}
			else
			{
				Log.Debug("user already exists");
			}
		}
	}
}