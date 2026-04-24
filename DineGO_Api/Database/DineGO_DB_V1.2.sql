CREATE TABLE [dbo].[admin] (
  [ad_id] int  NOT NULL,
  [ad_username] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_password] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_name] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_email] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_birthday] datetime2(3)  NOT NULL,
  [ad_image] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ad_is_use] bit  NULL,
  CONSTRAINT [admins_ad_id_primary] PRIMARY KEY CLUSTERED ([ad_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[admin] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[blog] (
  [blog_id] int  NOT NULL,
  [res_owner_id] int  NOT NULL,
  [blog_title] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [blog_information] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [blog_date] datetime2(7)  NOT NULL,
  [blog_image] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [blogs_blog_id_primary] PRIMARY KEY CLUSTERED ([blog_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[blog] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[cart] (
  [cart_id] int  NOT NULL,
  [res_id] int NULL,
  [cus_id] int  NULL,
  CONSTRAINT [PK_carts] PRIMARY KEY CLUSTERED ([cart_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[cart] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[cart_food] (
  [cart_food_id] int NOT NULL,
  [food_id] int NULL,
  [cart_id] int NULL,
  [is_buy] bit NULL,
  [food_quantity] int NULL,
  PRIMARY KEY CLUSTERED ([cart_food_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[category] (
  [cate_id] int  NOT NULL,
  [cate_type] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cate_description] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [categories_cate_id_primary] PRIMARY KEY CLUSTERED ([cate_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[category] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[comment] (
  [comment_id] int  NOT NULL,
  [post_id] int  NOT NULL,
  [cus_id] int NOT NULL,
  [comment_content] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [comment_created_date] datetime  NOT NULL,
  [comment_updated_date] datetime  NOT NULL,
  CONSTRAINT [PK__comments__E7957687B8F16733] PRIMARY KEY CLUSTERED ([comment_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[comment] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[customer] (
  [cus_id] int  NOT NULL,
  [cus_username] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_password] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_name] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_email] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_phone] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_address] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [cus_birthday] datetime2(3)  NULL,
  [cus_gender] bit  NULL,
  [cus_image] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [cus_is_kyc] bit  NULL,
  [google_id] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [login_provider] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [cus_status] int NULL,
  [cus_created_date] datetime NULL,
  [cus_last_login_date] datetime NULL,
  [cus_is_use] bit NOT NULL DEFAULT 1,
  CONSTRAINT [customers_cus_id_primary] PRIMARY KEY CLUSTERED ([cus_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[customer] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[customer_voucher] (
  [customer_voucher_id] int NOT NULL,
  [cus_id] int NOT NULL,
  [voucher_id] int NOT NULL,
  [customer_voucher_quantity] int NOT NULL,
  PRIMARY KEY CLUSTERED ([customer_voucher_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[delivery] (
  [de_id] int  NOT NULL,
  [order_id] int  NOT NULL,
  [de_status] int  NOT NULL,
  [de_start] datetime  NOT NULL,
  [de_end] datetime  NOT NULL,
  [de_note] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  CONSTRAINT [deliveries_de_id_primary] PRIMARY KEY CLUSTERED ([de_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[delivery] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[follower] (
  [follower_id] int NOT NULL,
  [res_owner_id] int NOT NULL,
  [cus_id] int NOT NULL,
  [follower_created] datetime  NOT NULL,
  CONSTRAINT [PK__follower__444E322FF8750C77] PRIMARY KEY CLUSTERED ([follower_id] DESC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[follower] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[food] (
  [food_id] int  NOT NULL,
  [menu_id] int  NOT NULL,
  [food_name] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [food_description] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [food_price] decimal(8,2)  NOT NULL,
  [food_image] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [food_status] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  CONSTRAINT [menus_menu_id_primary] PRIMARY KEY CLUSTERED ([food_id] DESC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[food] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[food_menu] (
  [food_menu_id] int NOT NULL,
  [food_id] int NULL,
  [menu_id] int NULL,
  PRIMARY KEY CLUSTERED ([food_menu_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[like] (
  [like_id] int  NOT NULL,
  [post_id] int  NULL,
  [cus_id] int NULL,
  [like_emotion_type] int NULL,
  CONSTRAINT [PK__likes__992C7930961EF49A] PRIMARY KEY CLUSTERED ([like_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[like] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[localization_records] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Key] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Value] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Culture] nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [LastModified] datetime DEFAULT getdate() NULL,
  CONSTRAINT [PK__Localiza__3214EC076B7D93C8] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[localization_records] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[menu] (
  [menu_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [menu_type] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [menu_image] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [menu_name] nvarchar(20) NOT NULL,
  CONSTRAINT [PK__menus__4CA0FADC6AAA968A] PRIMARY KEY CLUSTERED ([menu_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[menu] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[notification] (
  [noti_id] int  NOT NULL,
  [noti_title] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [noti_content] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [noti_type] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [noti_date] datetime  NOT NULL,
  CONSTRAINT [notifications_noti_id_primary] PRIMARY KEY CLUSTERED ([noti_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[notification] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[notification_customer] (
  [noti_customer_id] int NOT NULL,
  [noti_id] int NOT NULL,
  [cus_id] int NOT NULL,
  [noti_customer_is_read] bit NOT NULL DEFAULT 0,
  PRIMARY KEY CLUSTERED ([noti_customer_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[order] (
  [order_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [order_date] datetime  NOT NULL,
  [order_status] int  NOT NULL,
  [order_total] decimal(8,2)  NOT NULL,
  [order_price_discount] decimal(8,2) NULL,
  CONSTRAINT [orders_order_id_primary] PRIMARY KEY CLUSTERED ([order_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[order] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[order_detail] (
  [order_detail_id] int  NOT NULL,
  [cart_id] int NULL,
  [order_id] int  NOT NULL,
  [order_quantity] int  NOT NULL,
  [order_price] decimal(8,2)  NOT NULL,
  CONSTRAINT [orderdetails_order_detail_id_primary] PRIMARY KEY CLUSTERED ([order_detail_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[order_detail] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[payment] (
  [pay_id] int  NOT NULL,
  [cart_id] int NULL,
  [cus_id] int  NOT NULL,
  [reser_id] int  NULL,
  [pay_price] decimal(8,2)  NOT NULL,
  [pay_status] int  NULL,
  [pay_created_date] datetime  NOT NULL,
  [pay_price_discount] decimal(8,2) NULL,
  CONSTRAINT [payments_pay_id_primary] PRIMARY KEY CLUSTERED ([pay_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[payment] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[post] (
  [post_id] int  NOT NULL,
  [res_id] int NULL,
  [cus_id] int NULL,
  [post_content] text COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [post_image] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [post_created_date] datetime  NOT NULL,
  [post_updated_date] datetime  NOT NULL,
  [post_title] nvarchar(255) NULL,
  [post_author_name] nvarchar(50) NULL,
  [post_like_count] int NULL,
  [post_comment_count] int NULL,
  CONSTRAINT [PK__posts__3ED78766AB051C48] PRIMARY KEY CLUSTERED ([post_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[post] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[reservation] (
  [reser_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [reser_date] datetime  NOT NULL,
  [reser_quantity] int  NOT NULL,
  [reser_status] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [reser_note] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [reservations_reser_id_primary] PRIMARY KEY CLUSTERED ([reser_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[reservation] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[restaurant] (
  [res_id] int  NOT NULL,
  [cate_id] int  NOT NULL,
  [res_owner_id] int  NOT NULL,
  [res_name] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [res_address] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_email] nvarchar(255) NULL,
  [res_phone] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_description] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_rate] decimal(3,2)  NULL,
  [res_reservation_fee] decimal(10,2)  NULL,
  [res_discount_promotion] decimal(5,2)  NULL,
  [res_images] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_is_use] bit NOT NULL DEFAULT 1,
  CONSTRAINT [restaurants_res_id_primary] PRIMARY KEY CLUSTERED ([res_id] DESC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[restaurant] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[restaurant_owner] (
  [res_owner_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [res_owner_name] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [res_owner_created_date] datetime  NOT NULL,
  [res_owner_is_authorize] bit  NOT NULL,
  [res_owner_follower_count] int NOT NULL,
  [res_owner_is_use] bit NOT NULL DEFAULT 1,
  CONSTRAINT [restaurantowners_resowner_id_primary] PRIMARY KEY CLUSTERED ([res_owner_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[restaurant_owner] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[system_log] (
  [sys_log_id] int NOT NULL,
  [ad_id] int NULL,
  [action] nvarchar(100) NULL,
  [description] nvarchar(max) NULL,
  [log_time] datetime2(3) NULL,
  [ip_address] nvarchar(45) NULL,
  [device_info] nvarchar(255) NULL,
  [status_code] int NULL,
  [is_success] bit NULL,
  PRIMARY KEY CLUSTERED ([sys_log_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[vertification] (
  [ver_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [ver_license] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_tax_code] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_document] nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_status] int  NOT NULL,
  [ver_date_submitted] datetime  NOT NULL,
  [ver_date_verified] datetime  NOT NULL,
  [ver_file_attachment] nvarchar(500) NULL,
  CONSTRAINT [vertifications_ver_id_primary] PRIMARY KEY CLUSTERED ([ver_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[vertification] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[voucher] (
  [voucher_id] int  NOT NULL,
  [ad_id] int NULL,
  [voucher_code] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [voucher_discount] decimal(8,2)  NOT NULL,
  [voucher_start_date] datetime  NOT NULL,
  [voucher_end_date] datetime  NOT NULL,
  [voucher_stock] int NULL,
  CONSTRAINT [vouchers_voucher_id_primary] PRIMARY KEY CLUSTERED ([voucher_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[voucher] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[blog] ADD CONSTRAINT [blogs_resowner_id_foreign] FOREIGN KEY ([res_owner_id]) REFERENCES [dbo].[restaurant_owner] ([res_owner_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[cart] ADD CONSTRAINT [FK_carts_customers] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[cart] ADD CONSTRAINT [fk_cart_order_detail_2] FOREIGN KEY ([cart_id]) REFERENCES [dbo].[order_detail] ([order_detail_id])
GO
ALTER TABLE [dbo].[cart] ADD CONSTRAINT [fk_cart_restaurant_3] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurant] ([res_id])
GO
ALTER TABLE [dbo].[cart_food] ADD CONSTRAINT [fk_cart_food_food_1] FOREIGN KEY ([food_id]) REFERENCES [dbo].[food] ([food_id])
GO
ALTER TABLE [dbo].[cart_food] ADD CONSTRAINT [fk_cart_food_cart_2] FOREIGN KEY ([cart_id]) REFERENCES [dbo].[cart] ([cart_id])
GO
ALTER TABLE [dbo].[comment] ADD CONSTRAINT [fk_comments_posts_1] FOREIGN KEY ([post_id]) REFERENCES [dbo].[post] ([post_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[comment] ADD CONSTRAINT [fk_comment_customer_2] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id])
GO
ALTER TABLE [dbo].[customer_voucher] ADD CONSTRAINT [fk_customer_voucher_customer_1] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id])
GO
ALTER TABLE [dbo].[customer_voucher] ADD CONSTRAINT [fk_customer_voucher_voucher_2] FOREIGN KEY ([voucher_id]) REFERENCES [dbo].[voucher] ([voucher_id])
GO
ALTER TABLE [dbo].[delivery] ADD CONSTRAINT [deliveries_order_id_foreign] FOREIGN KEY ([order_id]) REFERENCES [dbo].[order] ([order_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[follower] ADD CONSTRAINT [fk_follower_customer_1] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id])
GO
ALTER TABLE [dbo].[follower] ADD CONSTRAINT [fk_follower_restaurant_owner_2] FOREIGN KEY ([res_owner_id]) REFERENCES [dbo].[restaurant_owner] ([res_owner_id])
GO
ALTER TABLE [dbo].[food_menu] ADD CONSTRAINT [fk_food_menu_food_1] FOREIGN KEY ([food_id]) REFERENCES [dbo].[food] ([food_id])
GO
ALTER TABLE [dbo].[food_menu] ADD CONSTRAINT [fk_food_menu_menu_2] FOREIGN KEY ([menu_id]) REFERENCES [dbo].[menu] ([menu_id])
GO
ALTER TABLE [dbo].[like] ADD CONSTRAINT [fk_likes_posts_1] FOREIGN KEY ([post_id]) REFERENCES [dbo].[post] ([post_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[like] ADD CONSTRAINT [fk_like_customer_2] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id])
GO
ALTER TABLE [dbo].[menu] ADD CONSTRAINT [fk_menus_restaurants_1] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurant] ([res_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[notification_customer] ADD CONSTRAINT [fk_notification_customer_notification_1] FOREIGN KEY ([noti_id]) REFERENCES [dbo].[notification] ([noti_id])
GO
ALTER TABLE [dbo].[notification_customer] ADD CONSTRAINT [fk_notification_customer_customer_2] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id])
GO
ALTER TABLE [dbo].[order] ADD CONSTRAINT [fk_orders_restaurants_1] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurant] ([res_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[order] ADD CONSTRAINT [fk_orders_customers_2] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[order_detail] ADD CONSTRAINT [orderdetails_order_id_foreign] FOREIGN KEY ([order_id]) REFERENCES [dbo].[order] ([order_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[payment] ADD CONSTRAINT [payments_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[payment] ADD CONSTRAINT [payments_reser_id_foreign] FOREIGN KEY ([reser_id]) REFERENCES [dbo].[reservation] ([reser_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[payment] ADD CONSTRAINT [fk_payment_cart_3] FOREIGN KEY ([cart_id]) REFERENCES [dbo].[cart] ([cart_id])
GO
ALTER TABLE [dbo].[post] ADD CONSTRAINT [fk_post_customer_1] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id])
GO
ALTER TABLE [dbo].[post] ADD CONSTRAINT [fk_post_restaurant_2] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurant] ([res_id]);
GO
ALTER TABLE [dbo].[reservation] ADD CONSTRAINT [reservations_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[reservation] ADD CONSTRAINT [reservations_res_id_foreign] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurant] ([res_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[restaurant] ADD CONSTRAINT [restaurants_cate_id_foreign] FOREIGN KEY ([cate_id]) REFERENCES [dbo].[category] ([cate_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[restaurant] ADD CONSTRAINT [restaurants_resowner_id_foreign] FOREIGN KEY ([res_owner_id]) REFERENCES [dbo].[restaurant_owner] ([res_owner_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[restaurant_owner] ADD CONSTRAINT [restaurantowners_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customer] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[system_log] ADD CONSTRAINT [fk_System Log_admin_1] FOREIGN KEY ([ad_id]) REFERENCES [dbo].[admin] ([ad_id])
GO
ALTER TABLE [dbo].[vertification] ADD CONSTRAINT [vertifications_res_id_foreign] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurant] ([res_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[voucher] ADD CONSTRAINT [fk_voucher_admin_2] FOREIGN KEY ([ad_id]) REFERENCES [dbo].[admin] ([ad_id])
GO

