using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Core.Services;
using DineGO_Api.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DineGO_Api.Data;
using Microsoft.EntityFrameworkCore;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Core.Models;
using Core.Common;
using DineGO_Api.SignalRHub;
using Amazon.S3;
using Microsoft.AspNetCore.SignalR;
using DineGO_Api.DAO;

namespace DineGO_Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddMemoryCache();
            //ConnectDB
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DineGO_Api", Version = "v1" });
            });
            services.AddSingleton<TokenService>();
            services.AddSingleton<HashService>();
            // Added: Đăng ký Distributed Memory Cache cho session
            services.AddDistributedMemoryCache();
            // Added: Đăng ký Session
            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromMinutes(30); // Session timeout sau 30 phút
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ✅ Authentication cấu hình đúng chuẩn
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google";
            })

            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddAuthorization();
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowClient",
                    builder =>
                    {
                        builder
                            .WithOrigins("https://localhost:5002") // domain client
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });
            services.AddSignalR();

            //cấu hình serialize JSON đúng cách để tránh đệ quy vô hạn do navigation property.
            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //Add dependency here
            services.AddScoped<RestaurantDAO>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<CategoryDAO>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<BlogDAO>();
            services.AddScoped<IBlogRepositoy, BlogRepository>();
            services.AddScoped<CustomerDAO>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IMailSenderRepository, MailSenderRepository>();
            services.AddScoped<RestaurantOwnerDAO>();
            services.AddScoped<IRestaurantOwnerRepository, RestaurantOwnerRepository>();
            services.AddScoped<ReservationDAO>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IRestaurantOwnerRepository, RestaurantOwnerRepository>();
            services.AddScoped<RestaurantOwnerDAO>();
            services.AddScoped<AdminDAO>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<OrderDAO>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<PostDAO>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<PaymentDAO>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<CommentDAO>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<FoodDAO>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<CartFoodDAO>();
            services.AddScoped<CartDAO>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<OrderDAO>();
            services.AddScoped<OrderDetailDAO>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<DeliveryDAO>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            services.AddScoped<VoucherDAO>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<NotificationDAO>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<VerificationDAO>();
            services.AddScoped<IVerificationRepository, VerificationRepository>();
            services.AddScoped<SystemLogDAO>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();

            services.AddScoped<ChatMessageDAO>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<FriendDAO>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<AIPredictRepository>();
            services.AddScoped<IAIPredictRepository, AIPredictRepository>();
            services.AddScoped<PriorityDAO>();
            services.AddScoped<CategoryDAO>();
            services.AddScoped<FollowerDAO>();

            services.AddScoped<LikeDAO>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<MenuDAO>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<RestaurantRatingDAO>();
            services.AddScoped<IRestaurantRatingRepository, RestaurantRatingRepository>();

            services.AddScoped<TableDAO>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<TableAreaDAO>();
            services.AddScoped<ITableAreaRepository, TableAreaRepository>();
            services.AddScoped<ReportDAO>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ContactDAO>();
            services.AddScoped<IContactRepository, ContactRepository>();
            //Add: S3 bucket to handle image
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<S3BucketAWS>();
            services.AddScoped<ImageHelper>();
            services.AddScoped<DashboardStatsAggregationService>();
            services.AddHostedService<DashboardBackgroundService>();
            services.AddHostedService<NotificationBackgroundService>();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddHttpClient();

            _ = services.AddScoped<OtpService>();
            services.AddScoped<CustomerPointDAO>(); 
            services.AddScoped<ICustomerPointRepository, CustomerPointRepository>(); 
            services.AddScoped<NotificationService>();
            services.AddScoped<AdDAO>(); 
            services.AddScoped<IAdRepository, AdRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DineGO_Api v1"));
            }
            app.UseCors("AllowClient");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                ApplicationDbContext.SeedData(context);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }
    }
}
