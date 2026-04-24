using System;
using System.IO;
using Core.Common;
using Core.Services;
using Core.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Amazon.S3;
using Core.Helper;

namespace DineGO_Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            // Cấu hình ApiSettings
            services.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ApiSettings>>().Value);

            services.AddHttpClient<ApiService>();
            services.AddSingleton<HashService>();
            services.AddScoped<RestaurantService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<RestaurantOwnerService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<BlogService>();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<S3BucketAWS>();
            services.AddScoped<ImageHelper>();
            services.AddScoped<MenuService>();
            services.AddScoped<GeoHelper>();
            services.AddScoped<FoodService>();
            services.AddScoped<CustomerPointService>();
            services.AddScoped<AdService>();

            // Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();

            // MVC
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            // Middleware bắt lỗi toàn cục
            app.Use(async (context, next) =>
            {
                try
                {
                    // Bỏ qua kiểm tra nếu là file tĩnh (css, js, images, favicon, ...)
                    var path = context.Request.Path.Value;
                    if (path.StartsWith("/images") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/favicon"))
                    {
                        await next();
                        return;
                    }

                    // Chỉ kiểm tra với các route cần đăng nhập
                    if (string.IsNullOrEmpty(context.Session.GetString("token")) && !context.Request.Path.StartsWithSegments("/Auth/Login"))
                    {
                        throw new UnauthorizedAccessException("User is not authenticated");
                    }
                    await next();
                }
                catch (UnauthorizedAccessException)
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                    }
                    else
                    {
                        context.Response.Redirect("/Auth/Login");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error: {ex.Message}");
                    context.Session.SetString("ErrorMessage", ex.Message);
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync("Internal Server Error");
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Error");
                    }
                }
            });
            app.UseMiddleware<SystemLogMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
