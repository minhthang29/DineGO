using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;

/// <summary>
/// Application database context managing entity configuration and seeding initial data.
/// </summary>
/// <author>Thangtm, sieuhdd</author>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartFood> CartFoods { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerVoucher> CustomerVouchers { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<FoodMenu> FoodMenus { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationCustomer> NotificationCustomers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantOwner> RestaurantOwners { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }
    public DbSet<Verification> Verifications { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<TableArea> TableAreas { get; set; }

    public DbSet<Priority> Priorities { get; set; }
    public DbSet<RestaurantRating> RestaurantRatings { get; set; }


    public DbSet<Report> Reports { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<DashboardStats> DashboardStats { get; set; }
    public DbSet<CustomerPoint> CustomerPoints { get; set; }
    public DbSet<CustomerPointHistory> CustomerPointHistories { get; set; }
    public DbSet<AdSlot> AdSlots { get; set; }
    public DbSet<AdRegistration> AdRegistrations { get; set; }
    public DbSet<AdHistory> AdHistories { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === PRIMARY KEYS
        modelBuilder.Entity<Admin>().HasKey(e => e.ad_id);
        modelBuilder.Entity<Blog>().HasKey(e => e.blog_id);
        modelBuilder.Entity<Cart>().HasKey(e => e.cart_id);
        modelBuilder.Entity<CartFood>().HasKey(e => e.cart_food_id);
        modelBuilder.Entity<Category>().HasKey(e => e.cate_id);
        modelBuilder.Entity<Comment>().HasKey(e => e.comment_id);
        modelBuilder.Entity<Customer>().HasKey(e => e.cus_id);
        modelBuilder.Entity<CustomerVoucher>().HasKey(e => e.customer_voucher_id);
        modelBuilder.Entity<Delivery>().HasKey(e => e.de_id);
        modelBuilder.Entity<Follower>().HasKey(e => e.follower_id);
        modelBuilder.Entity<Food>().HasKey(e => e.food_id);
        modelBuilder.Entity<FoodMenu>().HasKey(e => e.food_menu_id);
        modelBuilder.Entity<Like>().HasKey(e => e.like_id);
        modelBuilder.Entity<Menu>().HasKey(e => e.menu_id);
        modelBuilder.Entity<Notification>().HasKey(e => e.noti_id);
        modelBuilder.Entity<NotificationCustomer>().HasKey(e => e.noti_customer_id);
        modelBuilder.Entity<Order>().HasKey(e => e.order_id);
        modelBuilder.Entity<OrderDetail>().HasKey(e => e.order_detail_id);
        modelBuilder.Entity<Payment>().HasKey(e => e.pay_id);
        modelBuilder.Entity<Post>().HasKey(e => e.post_id);
        modelBuilder.Entity<Reservation>().HasKey(e => e.reser_id);
        modelBuilder.Entity<Restaurant>().HasKey(e => e.res_id);
        modelBuilder.Entity<RestaurantOwner>().HasKey(e => e.res_owner_id);
        modelBuilder.Entity<SystemLog>().HasKey(e => e.sys_log_id);
        modelBuilder.Entity<Verification>().HasKey(e => e.ver_id);
        modelBuilder.Entity<Voucher>().HasKey(e => e.voucher_id);
        modelBuilder.Entity<Table>().HasKey(t => t.table_id);

        modelBuilder.Entity<Report>().HasKey(e => e.report_id);
        modelBuilder.Entity<Contact>().HasKey(e => e.contact_id);
        // === FOREIGN KEYS

        modelBuilder.Entity<Blog>()
            .HasOne(b => b.restaurantOwner)
            .WithMany(ro => ro.blogs)
            .HasForeignKey(b => b.res_owner_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Blog>()
            .HasOne(b => b.admin)
            .WithMany(a => a.blogs)
            .HasForeignKey(b => b.ad_id)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.customer)
            .WithMany(cu => cu.carts)
            .HasForeignKey(c => c.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.restaurant)
            .WithMany(r => r.carts)
            .HasForeignKey(c => c.res_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CartFood>()
            .HasOne(cf => cf.food)
            .WithMany(f => f.cart_foods)
            .HasForeignKey(cf => cf.food_id);

        modelBuilder.Entity<CartFood>()
            .HasOne(cf => cf.cart)
            .WithMany(c => c.cartFoods)
            .HasForeignKey(cf => cf.cart_id);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.customer)
            .WithMany(cu => cu.comments)
            .HasForeignKey(c => c.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.post)
            .WithMany(p => p.comments)
            .HasForeignKey(c => c.post_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CustomerVoucher>()
            .HasOne(cv => cv.customer)
            .WithMany(c => c.customerVouchers)
            .HasForeignKey(cv => cv.cus_id);

        modelBuilder.Entity<CustomerVoucher>()
            .HasOne(cv => cv.voucher)
            .WithMany(v => v.customerVouchers)
            .HasForeignKey(cv => cv.voucher_id);

        modelBuilder.Entity<Delivery>()
            .HasOne(d => d.order)
            .WithMany(o => o.delivery)
            .HasForeignKey(d => d.order_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.customer)
            .WithMany(c => c.followers)
            .HasForeignKey(f => f.cus_id);

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.restaurantOwner)
            .WithMany(ro => ro.followers)
            .HasForeignKey(f => f.res_owner_id);

        modelBuilder.Entity<FoodMenu>()
            .HasOne(fm => fm.food)
            .WithMany(f => f.food_menus)
            .HasForeignKey(fm => fm.food_id);

        modelBuilder.Entity<FoodMenu>()
            .HasOne(fm => fm.menu)
            .WithMany(m => m.food_menus)
            .HasForeignKey(fm => fm.menu_id);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.customer)
            .WithMany(c => c.likes)
            .HasForeignKey(l => l.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.post)
            .WithMany(p => p.likes)
            .HasForeignKey(l => l.post_id)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Menu>()
            .HasOne(m => m.restaurant)
            .WithMany(r => r.menus)
            .HasForeignKey(m => m.res_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NotificationCustomer>()
            .HasOne(nc => nc.notification)
            .WithMany(n => n.notificationCustomers)
            .HasForeignKey(nc => nc.noti_id);

        modelBuilder.Entity<NotificationCustomer>()
            .HasOne(nc => nc.customer)
            .WithMany(c => c.notificationCustomers)
            .HasForeignKey(nc => nc.cus_id);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.restaurant)
            .WithMany(r => r.orders)
            .HasForeignKey(o => o.res_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.customer)
            .WithMany()
            .HasForeignKey(o => o.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.order)
            .WithMany(o => o.orderDetails)
            .HasForeignKey(od => od.order_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.customer)
            .WithMany(c => c.payments)
            .HasForeignKey(p => p.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.reservation)
            .WithMany(r => r.payments)
            .HasForeignKey(p => p.reser_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.cart)
            .WithMany(c => c.payments)
            .HasForeignKey(p => p.cart_id);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.customer)
            .WithMany(c => c.posts)
            .HasForeignKey(p => p.cus_id);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.restaurant)
            .WithMany(r => r.posts)
            .HasForeignKey(p => p.res_id);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.customer)
            .WithMany(c => c.reservations)
            .HasForeignKey(r => r.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.restaurant)
            .WithMany(res => res.reservations)
            .HasForeignKey(r => r.res_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Restaurant>()
            .HasOne(r => r.category)
            .WithMany(c => c.restaurants)
            .HasForeignKey(r => r.cate_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Restaurant>()
            .HasOne(r => r.restaurantOwner)
            .WithMany(ro => ro.restaurants)
            .HasForeignKey(r => r.res_owner_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RestaurantOwner>()
            .HasOne(ro => ro.customer)
            .WithMany(c => c.restaurantOwners)
            .HasForeignKey(ro => ro.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SystemLog>()
            .HasOne(sl => sl.admin)
            .WithMany(a => a.systemLogs)
            .HasForeignKey(sl => sl.ad_id);

        modelBuilder.Entity<Verification>()
            .HasOne(v => v.restaurant)
            .WithMany(r => r.verifications)
            .HasForeignKey(v => v.res_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Voucher>()
            .HasOne(v => v.admin)
            .WithMany(a => a.vouchers)
            .HasForeignKey(v => v.ad_id)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Food>()
            .HasOne(f => f.menu)
            .WithMany(m => m.foods)
            .HasForeignKey(f => f.menu_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.Customer)
            .WithMany(c => c.Friends)
            .HasForeignKey(f => f.customer_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.FriendCustomer)
            .WithMany(c => c.FriendOf)
            .HasForeignKey(f => f.friend_customer_id)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Report>()
            .HasOne(r => r.customer)
            .WithMany()
            .HasForeignKey(r => r.cus_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.admin)
            .WithMany()
            .HasForeignKey(r => r.admin_id)
            .OnDelete(DeleteBehavior.SetNull);



        modelBuilder.Entity<Priority>().HasKey(p => p.priority_id);

        modelBuilder.Entity<Priority>()
            .HasOne(p => p.customer)
            .WithMany(c => c.priorities)
            .HasForeignKey(p => p.cus_id)
            .OnDelete(DeleteBehavior.Cascade); // hoặc .Restrict nếu không muốn xóa theo


        modelBuilder.Entity<RestaurantRating>().HasKey(r => r.rating_id);

        modelBuilder.Entity<RestaurantRating>()
            .HasOne(r => r.customer)
            .WithMany(c => c.restaurantRatings)
            .HasForeignKey(r => r.cus_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RestaurantRating>()
            .HasOne(r => r.restaurant)
            .WithMany(res => res.restaurantRatings)
            .HasForeignKey(r => r.res_id)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Table>()
            .HasOne(t => t.Restaurant)
            .WithMany(r => r.Tables)
            .HasForeignKey(t => t.res_id)
            .OnDelete(DeleteBehavior.NoAction);

        // === DECIMAL PRECISIONS
        modelBuilder.Entity<Food>().Property(f => f.food_price).HasPrecision(12, 2);
        modelBuilder.Entity<Order>().Property(o => o.order_total).HasPrecision(12, 2);
        modelBuilder.Entity<Order>().Property(o => o.order_price_discount).HasPrecision(12, 2);
        modelBuilder.Entity<OrderDetail>().Property(od => od.order_price).HasPrecision(12, 2);
        modelBuilder.Entity<Payment>().Property(p => p.pay_price).HasPrecision(12, 2);
        modelBuilder.Entity<Payment>().Property(p => p.pay_price_discount).HasPrecision(12, 2);
        modelBuilder.Entity<Restaurant>().Property(r => r.res_rate).HasPrecision(3, 2);
        modelBuilder.Entity<Restaurant>().Property(r => r.res_reservation_fee).HasPrecision(12, 2);
        modelBuilder.Entity<Restaurant>().Property(r => r.res_discount_promotion).HasPrecision(12, 2);
        modelBuilder.Entity<Voucher>().Property(v => v.voucher_discount).HasPrecision(12, 2);

        // === UNIQUE INDEXES
        modelBuilder.Entity<Customer>().HasIndex(c => c.cus_email).IsUnique();
        modelBuilder.Entity<Customer>().HasIndex(c => c.cus_username).IsUnique();
        modelBuilder.Entity<Admin>().HasIndex(a => a.ad_username).IsUnique();
        modelBuilder.Entity<Voucher>().HasIndex(v => v.voucher_code).IsUnique();

        // === JSON CONVERSION (Restaurant Images)
        modelBuilder.Entity<Restaurant>()
            .Property(r => r.res_images)
            .HasConversion<string>(
                v => v,
                v => v
            );
        //Set up default value
        modelBuilder.Entity<Admin>()
            .Property(a => a.admin_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Customer>()
            .Property(c => c.cus_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Restaurant>()
            .Property(r => r.res_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Restaurant>()
            .Property(r => r.res_is_authorized)
            .HasDefaultValue(false);

        modelBuilder.Entity<RestaurantOwner>()
            .Property(ro => ro.res_owner_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Category>()
            .Property(c => c.cate_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Blog>()
            .Property(b => b.blog_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Cart>()
            .Property(c => c.cart_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Reservation>()
            .Property(r => r.reser_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Order>()
            .Property(o => o.order_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Payment>()
            .Property(p => p.pay_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Voucher>()
            .Property(v => v.voucher_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Verification>()
            .Property(v => v.ver_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Notification>()
            .Property(n => n.noti_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Delivery>()
            .Property(d => d.de_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Report>()
            .Property(d => d.report_is_deleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Contact>()
            .Property(d => d.contact_is_deleted)
            .HasDefaultValue(false);

        // === OPTIONAL FIELDS
        modelBuilder.Entity<Customer>()
            .Property(c => c.login_provider)
            .HasMaxLength(20)
            .IsRequired(false);

        modelBuilder.Entity<Customer>()
            .Property(c => c.google_id)
            .HasMaxLength(100)
            .IsRequired(false);

        modelBuilder.Entity<TableArea>().HasQueryFilter(a => !a.is_deleted);
        // CustomerPoint
        modelBuilder.Entity<CustomerPoint>().HasKey(p => p.point_id);

        modelBuilder.Entity<CustomerPoint>()
            .HasOne(p => p.customer)
            .WithOne(c => c.customerPoint)
            .HasForeignKey<CustomerPoint>(p => p.cus_id)
            .OnDelete(DeleteBehavior.Cascade);

        // CustomerPointHistory
        modelBuilder.Entity<CustomerPointHistory>().HasKey(h => h.history_id);

        modelBuilder.Entity<CustomerPointHistory>()
            .HasOne(h => h.customerPoint)
            .WithMany(p => p.pointHistories)
            .HasForeignKey(h => h.point_id)
            .OnDelete(DeleteBehavior.Cascade);

        // AdSlot
        modelBuilder.Entity<AdSlot>().HasKey(s => s.slot_id);
        modelBuilder.Entity<AdSlot>()
            .Property(s => s.slot_price)
            .HasColumnType("decimal(12,2)");

        // AdRegistration
        modelBuilder.Entity<AdRegistration>().HasKey(r => r.ad_id);

        modelBuilder.Entity<AdRegistration>()
            .HasOne(r => r.slot)
            .WithMany(s => s.registrations)
            .HasForeignKey(r => r.slot_id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AdRegistration>()
            .HasOne(r => r.restaurantOwner)
            .WithMany(ro => ro.adRegistrations) // navigation mới thêm
            .HasForeignKey(r => r.res_owner_id)
            .OnDelete(DeleteBehavior.NoAction);

        // AdHistory
        modelBuilder.Entity<AdHistory>().HasKey(h => h.history_id);
    }



    /// <summary>
    /// Seed initial data into the database if it's newly created.
    /// </summary>
    /// <param name="context">ApplicationDbContext instance</param>
    public static void SeedData(ApplicationDbContext context)
    {
        if (context.Database.EnsureCreated())
        {
            // 1. Categories
            var categories = new[]
            {
                new Category { cate_type = "Nhà hàng chay", cate_description = "Món chay dinh dưỡng, không dùng nguyên liệu động vật" },
                new Category { cate_type = "Nhà hàng hải sản", cate_description = "Chuyên các món từ hải sản tươi sống" },
                new Category { cate_type = "Quán ăn gia đình", cate_description = "Thân thiện, gần gũi và phù hợp cho mọi lứa tuổi" },
                new Category { cate_type = "Nhà hàng Nhật", cate_description = "Sushi, sashimi và các món Nhật truyền thống" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // 2. Admins
            var admin = new Admin
            {
                ad_username = "admin1",
                ad_password = "/hQotwzL7hZpwXjnNKHvKg==:FpcCgVrhQu4SEBTlDDl9n89XSs5y2vDJH+W0ETj3MDM=",
                ad_name = "Super Admin",
                ad_email = "admin1@example.com",
                ad_birthday = new DateTime(1980, 1, 1),
                ad_image = "1.png",
                ad_is_use = true
            };
            context.Admins.Add(admin);
            context.SaveChanges();

            // 3. Customers
            var customers = new[]
            {
                new Customer {
                    cus_username = "devdinego1", cus_password = "/hQotwzL7hZpwXjnNKHvKg==:FpcCgVrhQu4SEBTlDDl9n89XSs5y2vDJH+W0ETj3MDM=", cus_name = "Dev1", cus_email = "john@example.com",
                    cus_phone = "0901111222", cus_address = "123 Le Loi", cus_birthday = new DateTime(1990,1,1),
                    cus_gender = true, cus_is_kyc = true, cus_is_use = true, login_provider = "Local"
                },
                new Customer {
                    cus_username = "devdinego2", cus_password = "/hQotwzL7hZpwXjnNKHvKg==:FpcCgVrhQu4SEBTlDDl9n89XSs5y2vDJH+W0ETj3MDM=", cus_name = "Dev2", cus_email = "jane@example.com",
                    cus_phone = "0903333444", cus_address = "456 Nguyen Hue", cus_birthday = new DateTime(1992,5,10),
                    cus_gender = false, cus_is_kyc = false, cus_is_use = true, login_provider = "Local"
                }
            };
            context.Customers.AddRange(customers);
            context.SaveChanges();

            // 4. RestaurantOwners
            var owners = new[]
            {
                new RestaurantOwner {
                    cus_id = customers[0].cus_id, res_owner_name = "Owner 1",
                    res_owner_created_date = DateTime.Now,
                    res_owner_follower_count = 0, res_owner_is_use = true
                },
                new RestaurantOwner {
                    cus_id = customers[1].cus_id, res_owner_name = "Owner 2",
                    res_owner_created_date = DateTime.Now,
                    res_owner_follower_count = 0, res_owner_is_use = true
                }
            };
            context.RestaurantOwners.AddRange(owners);
            context.SaveChanges();

            // 5. Restaurants
            var restaurants = new[]
            {
                new Restaurant {
                    res_name = "Chay Garden", cate_id = categories[0].cate_id, res_owner_id = owners[0].res_owner_id,
                    res_address = "Thành phố Cần Thơ", res_email = "chay@example.com", res_phone = "0900000001",
                    res_description = "Quán chay yên tĩnh", res_rate = 4.5m,
                    res_reservation_fee = 50000, res_discount_promotion = 10, res_is_use = true, res_latitude = 10.020192714533945,
                    res_longitude = 105.74216523650907
                },
                new Restaurant {
                    res_name = "Seafood King", cate_id = categories[1].cate_id, res_owner_id = owners[1].res_owner_id,
                    res_address = "tỉnh Bạc Liêu", res_email = "seafood@example.com", res_phone = "0900000002",
                    res_description = "Hải sản tươi sống", res_rate = 4.2m,
                    res_reservation_fee = 75000, res_discount_promotion = 15, res_is_use = true, res_latitude = 9.182630606104851,
                    res_longitude = 105.1562726769267
                }
            };
            context.Restaurants.AddRange(restaurants);
            context.SaveChanges();

            // 6. Menu
            var menus = new[]
            {
                new Menu { res_id = restaurants[0].res_id, menu_type = "Lunch", menu_image = "/img/m1.jpg", menu_name = "Chay Menu" },
                new Menu { res_id = restaurants[1].res_id, menu_type = "Dinner", menu_image = "/img/m2.jpg", menu_name = "Seafood Menu" }
            };
            context.Menus.AddRange(menus);
            context.SaveChanges();

            // 7. Food
            var foods = new[]
            {
                new Food {
                    menu_id = menus[0].menu_id, food_name = "Chay Roll", food_description = "Cuốn chay thanh đạm",
                    food_price = 35000, food_image = "/img/f1.jpg", food_status = 1
                },
                new Food {
                    menu_id = menus[1].menu_id, food_name = "Tôm nướng", food_description = "Tôm nướng mọi",
                    food_price = 85000, food_image = "/img/f2.jpg", food_status = 1
                }
            };
            context.Foods.AddRange(foods);
            context.SaveChanges();

            // 8. Vouchers
            var vouchers = new[]
            {
                new Voucher {
                    ad_id = admin.ad_id, voucher_code = "WELCOME10", voucher_discount = 10,
                    voucher_start_date = DateTime.Now, voucher_end_date = DateTime.Now.AddMonths(1), voucher_stock = 100,
                    voucher_type = 0, voucher_apply_type = 0
                }
            };
            context.Vouchers.AddRange(vouchers);
            context.SaveChanges();

            // 8.1 TableArea
            var areas = new[]
            {
                new TableArea { area_name = "Sảnh A", res_id = restaurants[0].res_id },
                new TableArea { area_name = "Sảnh B", res_id = restaurants[1].res_id }
            };
            context.TableAreas.AddRange(areas);
            context.SaveChanges();


            // 8.2 Tables
            var tables = new[]
            {
                new Table { res_id = restaurants[0].res_id, table_name = "Bàn 1", table_seat = 2, area_id = areas[0].area_id },
                new Table { res_id = restaurants[0].res_id, table_name = "Bàn 2", table_seat = 4, area_id = areas[0].area_id },
                new Table { res_id = restaurants[1].res_id, table_name = "Bàn 3", table_seat = 2, area_id = areas[1].area_id },
                new Table { res_id = restaurants[1].res_id, table_name = "Bàn 4", table_seat = 6, area_id = areas[1].area_id }
            };
            context.Tables.AddRange(tables);
            context.SaveChanges();

            // 9. CustomerVoucher
            var custVouchers = new[]
            {
                new CustomerVoucher {
                    cus_id = customers[0].cus_id, voucher_id = vouchers[0].voucher_id, customer_voucher_quantity = 1
                }
            };
            context.CustomerVouchers.AddRange(custVouchers);
            context.SaveChanges();
            // 10. Reservations
            var reservations = new[]
            {
                new Reservation {
                    cus_id = customers[0].cus_id, res_id = restaurants[0].res_id,
                    table_id = tables[0].table_id,
                    reser_date = DateTime.Now.AddDays(1),
                    reser_status = 0, reser_note = "Bàn ngoài trời"
                },
                new Reservation {
                    cus_id = customers[1].cus_id, res_id = restaurants[1].res_id,
                    table_id = tables[0].table_id,
                    reser_date = DateTime.Now.AddDays(2),
                    reser_status = 1, reser_note = "Bàn ngoài trời"
                }
            };
            context.Reservations.AddRange(reservations);
            context.SaveChanges();

            // 11. Payments
            var payments = new[]
            {
                new Payment {
                    cus_id = customers[0].cus_id, reser_id = reservations[0].reser_id,
                    pay_price = 70000, pay_status = 1, pay_created_date = DateTime.Now,
                    pay_price_discount = 10000
                },
                new Payment {
                    cus_id = customers[1].cus_id, reser_id = reservations[1].reser_id,
                    pay_price = 100000, pay_status = 1, pay_created_date = DateTime.Now,
                    pay_price_discount = 0
                }
            };
            context.Payments.AddRange(payments);
            context.SaveChanges();

            // 12. Orders
            var orders = new[]
            {
                new Order {
                    cus_id = customers[0].cus_id, res_id = restaurants[0].res_id,
                    order_date = DateTime.Now, order_status = 1, order_total = 70000, order_price_discount = 10000
                },
                new Order {
                    cus_id = customers[1].cus_id, res_id = restaurants[1].res_id,
                    order_date = DateTime.Now, order_status = 0, order_total = 100000, order_price_discount = 0
                }
            };
            context.Orders.AddRange(orders);
            context.SaveChanges();

            // 13. OrderDetails
            var orderDetails = new[]
            {
                new OrderDetail {
                    order_id = orders[0].order_id, order_quantity = 2,
                    order_price = 35000
                },
                new OrderDetail {
                    order_id = orders[1].order_id, order_quantity = 1,
                    order_price = 100000
                }
            };
            context.OrderDetails.AddRange(orderDetails);
            context.SaveChanges();

            // 14. Deliveries
            var deliveries = new[]
            {
                new Delivery {
                    order_id = orders[0].order_id, de_status = 1,
                    de_start = DateTime.Now, de_end = DateTime.Now.AddMinutes(30),
                    de_note = "Giao sớm"
                },
                new Delivery {
                    order_id = orders[1].order_id, de_status = 2,
                    de_start = DateTime.Now.AddMinutes(-60), de_end = DateTime.Now,
                    de_note = "Đã giao"
                }
            };
            context.Deliveries.AddRange(deliveries);
            context.SaveChanges();

            // 15. Posts
            var posts = new[]
            {
                new Post {
                    cus_id = customers[0].cus_id, res_id = restaurants[0].res_id,
                    post_content = "Món ăn ngon tuyệt vời!",
                    post_image = "/img/post1.jpg", post_created_date = DateTime.Now, post_updated_date = DateTime.Now
                }
            };
            context.Posts.AddRange(posts);
            context.SaveChanges();

            // 16. Comments
            var comments = new[]
            {
                new Comment {
                    post_id = posts[0].post_id, cus_id = customers[1].cus_id,
                    comment_content = "Tôi cũng thấy vậy!", comment_created_date = DateTime.Now, comment_updated_date = DateTime.Now
                }
            };
            context.Comments.AddRange(comments);
            context.SaveChanges();

            // 17. Likes
            var likes = new[]
            {
                new Like {
                    post_id = posts[0].post_id, cus_id = customers[1].cus_id,
                    like_emotion_type = 1
                }
            };
            context.Likes.AddRange(likes);
            context.SaveChanges();

            // 18. Follower
            var followers = new[]
            {
                new Follower {
                    cus_id = customers[1].cus_id,
                    res_owner_id = owners[0].res_owner_id,
                    follower_created = DateTime.Now
                }
            };
            context.Followers.AddRange(followers);
            context.SaveChanges();

            // 19. Verification
            var verifications = new[]
            {
                new Verification {
                    res_id = restaurants[0].res_id, ver_license = "ABC-123",
                    ver_tax_code = "TAX-456", ver_document = "/files/doc1.pdf",
                    ver_status = 1, ver_date_submitted = DateTime.Now,
                    ver_date_verified = DateTime.Now, ver_file_attachment = "/files/att1.pdf"
                }
            };
            context.Verifications.AddRange(verifications);
            context.SaveChanges();

            // 20. Notifications
            var notifications = new[]
            {
                new Notification {
                    noti_title = "Đặt bàn thành công",
                    noti_content = "Bạn đã đặt bàn thành công tại nhà hàng.",
                    noti_type = "System", noti_date = DateTime.Now
                },
                new Notification {
                    noti_title = "Chào mừng đến với DineGO",
                    noti_content = "Chúng tôi rất vui mừng chào đón bạn đến với DineGO!",
                    noti_type = "WELCOME", noti_date = DateTime.Now
                }
            };

            context.Notifications.AddRange(notifications);
            context.SaveChanges();

            // 21. NotificationCustomer
            var notiCustomers = new[]
            {
                new NotificationCustomer {
                    noti_id = notifications[0].noti_id,
                    cus_id = customers[0].cus_id,
                    noti_customer_is_read = false
                }
            };
            context.NotificationCustomers.AddRange(notiCustomers);
            context.SaveChanges();
        }
    }
}
