﻿using Serilog;
using Serilog.Events;
using WebLab.Domain;
using WebLab.Extensions;
using WebLab.Models;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;
using WebLab.Services.Cart;
using WebLab.TagHelpers;

namespace WebLab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            AddServices(builder.Services);

            var uriData = builder.Configuration.GetSection("UriData").Get<UriData>()!;
            builder.Services.AddHttpClient("API", opt => opt.BaseAddress = new(uriData.ApiUri));

            builder.Services.AddHttpContextAccessor();
            builder.Services
                .AddAuthentication(opt =>
            {
                opt.DefaultScheme = "access_token";
                opt.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("access_token")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = builder.Configuration["InteractiveServiceSettings:AuthorityUrl"];
                    options.ClientId = builder.Configuration["InteractiveServiceSettings:ClientId"];
                    options.ClientSecret = builder.Configuration["InteractiveServiceSettings:ClientSecret"];
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseType = "code";
                    options.ResponseMode = "query";
                    options.SaveTokens = true;
                });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var logger = new LoggerConfiguration()
                               .ReadFrom.Configuration(builder.Configuration)
                               .CreateLogger();

            Log.Logger = logger;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();


            app.MapControllerRoute(
                  name: "Admin",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.UseLoggingMiddleware();

            app.MapRazorPages().RequireAuthorization();

            app.Run();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IBeerService, ApiBeerService>();
            services.AddScoped<IBeerTypeService, ApiBeerTypeService>();

            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

            services.AddScoped<PagerTagHelper>();
        }
    }
}
