CREATE TABLE [dbo].[admins] (
  [ad_id] int  NOT NULL,
  [ad_username] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_password] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_name] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_email] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ad_birthday] datetime2(7)  NOT NULL,
  [ad_image] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [admins_ad_id_primary] PRIMARY KEY CLUSTERED ([ad_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[admins] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[blogs] (
  [blog_id] int  NOT NULL,
  [resOwner_id] int  NOT NULL,
  [blog_title] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [blog_information] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [blog_date] datetime2(7)  NOT NULL,
  [blog_image] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [blogs_blog_id_primary] PRIMARY KEY CLUSTERED ([blog_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[blogs] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[carts] (
  [cart_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [menu_id] int  NULL,
  [cart_quantity] int  NOT NULL,
  [cart_price] decimal(10,2)  NULL,
  CONSTRAINT [PK_carts] PRIMARY KEY CLUSTERED ([cart_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[carts] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[categories] (
  [cate_id] int  NOT NULL,
  [cate_type] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cate_description] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [categories_cate_id_primary] PRIMARY KEY CLUSTERED ([cate_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[categories] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[comments] (
  [comment_id] int NOT NULL,
  [post_id] int NOT NULL,
  [user_id] int NOT NULL,
  [user_role] bit NOT NULL,
  [comment_content] nvarchar NOT NULL,
  [comment_created] datetime NOT NULL,
  PRIMARY KEY CLUSTERED ([comment_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[customers] (
  [cus_id] int  NOT NULL,
  [cus_username] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_password] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_name] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_email] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_phone] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [cus_address] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [cus_birthday] datetime2(7)  NULL,
  [cus_gender] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [cus_image] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [cus_isKYI] bit  NULL,
  [google_id] nvarchar(100) NULL,
  [login_provider] nvarchar(20) NULL,
  CONSTRAINT [customers_cus_id_primary] PRIMARY KEY CLUSTERED ([cus_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[customers] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[deliveries] (
  [de_id] int  NOT NULL,
  [order_id] int  NOT NULL,
  [de_status] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [de_start] datetime  NOT NULL,
  [de_end] datetime  NOT NULL,
  [de_note] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  CONSTRAINT [deliveries_de_id_primary] PRIMARY KEY CLUSTERED ([de_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[deliveries] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[followers ] (
  [follower_id] int NOT NULL,
  [user_id] int NOT NULL,
  [user_role] bit NOT NULL,
  [follower_created] datetime NOT NULL,
  PRIMARY KEY CLUSTERED ([follower_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[foods] (
  [food_id] int  NOT NULL,
  [menu_id] int  NOT NULL,
  [food_name] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [food_description] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [food_price] decimal(8,2)  NOT NULL,
  [food_image] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [food_status] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  CONSTRAINT [menus_menu_id_primary] PRIMARY KEY CLUSTERED ([food_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[foods] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[likes] (
  [like_id] int NOT NULL,
  [user_id] int NOT NULL,
  [user_role] bit NOT NULL,
  [post_id] int NOT NULL,
  [post_created] datetime NOT NULL,
  PRIMARY KEY CLUSTERED ([like_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[menus] (
  [menu_id] int NOT NULL,
  [res_id] int NOT NULL,
  [menu_type] nvarchar(20) NOT NULL,
  [menu_image] varchar(255) NOT NULL,
  PRIMARY KEY CLUSTERED ([menu_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[notifications] (
  [noti_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [noti_title] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [noti_content] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [noti_type] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [noti_date] datetime2(7)  NOT NULL,
  [note_is_read] bit NOT NULL,
  CONSTRAINT [notifications_noti_id_primary] PRIMARY KEY CLUSTERED ([noti_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[notifications] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[orderDetails] (
  [order_detail_id] int  NOT NULL,
  [order_id] int  NOT NULL,
  [menu_id] int  NOT NULL,
  [order_quantity] int  NOT NULL,
  [order_price] decimal(8,2)  NOT NULL,
  CONSTRAINT [orderdetails_order_detail_id_primary] PRIMARY KEY CLUSTERED ([order_detail_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[orderDetails] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[orders] (
  [order_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [order_date] datetime  NOT NULL,
  [order_status] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [order_total] decimal(8,2)  NOT NULL,
  CONSTRAINT [orders_order_id_primary] PRIMARY KEY CLUSTERED ([order_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[orders] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[payments] (
  [pay_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [reser_id] int  NOT NULL,
  [pay_price] float(53)  NOT NULL,
  [pay_status] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [pay_createdDate] datetime2(7)  NOT NULL,
  CONSTRAINT [payments_pay_id_primary] PRIMARY KEY CLUSTERED ([pay_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[payments] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[posts] (
  [post_id] int NOT NULL,
  [user_id] int NOT NULL,
  [user_role] bit NOT NULL,
  [post_content] nvarchar(max) NOT NULL,
  [post_image] nvarchar(max) NOT NULL,
  [post_created] datetime NOT NULL,
  [post_updated] datetime NOT NULL,
  PRIMARY KEY CLUSTERED ([post_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [dbo].[reservations] (
  [reser_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [reser_date] datetime2(7)  NOT NULL,
  [reser_quantity] int  NOT NULL,
  [reser_status] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [reser_note] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [reservations_reser_id_primary] PRIMARY KEY CLUSTERED ([reser_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[reservations] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[restaurantOwners] (
  [resOwner_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [resOwner_name] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [resOwner_createdDate] datetime2(7)  NOT NULL,
  [resOwner_isAuthorize] bit  NOT NULL,
  CONSTRAINT [restaurantowners_resowner_id_primary] PRIMARY KEY CLUSTERED ([resOwner_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[restaurantOwners] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[restaurants] (
  [res_id] int  NOT NULL,
  [cate_id] int  NULL,
  [resOwner_id] int  NOT NULL,
  [res_name] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [res_address] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_phone] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_information] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [res_rate] decimal(18,2)  NULL,
  [res_price] decimal(18,2)  NULL,
  [res_discount] decimal(18,2)  NULL,
  [res_images] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  CONSTRAINT [restaurants_res_id_primary] PRIMARY KEY CLUSTERED ([res_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[restaurants] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[vertifications] (
  [ver_id] int  NOT NULL,
  [res_id] int  NOT NULL,
  [ver_license] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_tax_code] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_document] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_status] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ver_date_submitted] datetime  NOT NULL,
  [ver_date_verified] datetime  NOT NULL,
  CONSTRAINT [vertifications_ver_id_primary] PRIMARY KEY CLUSTERED ([ver_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[vertifications] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE [dbo].[vouchers] (
  [voucher_id] int  NOT NULL,
  [cus_id] int  NOT NULL,
  [voucher_code] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [voucher_dícount] decimal(8,2)  NOT NULL,
  [voucher_start] datetime  NOT NULL,
  [voucher_end] bigint  NOT NULL,
  CONSTRAINT [vouchers_voucher_id_primary] PRIMARY KEY CLUSTERED ([voucher_id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO
ALTER TABLE [dbo].[vouchers] SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [dbo].[blogs] ADD CONSTRAINT [blogs_resowner_id_foreign] FOREIGN KEY ([resOwner_id]) REFERENCES [dbo].[restaurantOwners] ([resOwner_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[carts] ADD CONSTRAINT [FK_carts_customers] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[carts] ADD CONSTRAINT [FK_carts_menus] FOREIGN KEY ([menu_id]) REFERENCES [dbo].[foods] ([food_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[comments] ADD CONSTRAINT [fk_comments_posts_1] FOREIGN KEY ([post_id]) REFERENCES [dbo].[posts] ([post_id])
GO
ALTER TABLE [dbo].[deliveries] ADD CONSTRAINT [deliveries_order_id_foreign] FOREIGN KEY ([order_id]) REFERENCES [dbo].[orders] ([order_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[foods] ADD CONSTRAINT [fk_foods_menus_1] FOREIGN KEY ([menu_id]) REFERENCES [dbo].[menus] ([menu_id])
GO
ALTER TABLE [dbo].[likes] ADD CONSTRAINT [fk_likes_posts_1] FOREIGN KEY ([post_id]) REFERENCES [dbo].[posts] ([post_id])
GO
ALTER TABLE [dbo].[menus] ADD CONSTRAINT [fk_menus_restaurants_1] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurants] ([res_id])
GO
ALTER TABLE [dbo].[notifications] ADD CONSTRAINT [notifications_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[orderDetails] ADD CONSTRAINT [orderdetails_order_id_foreign] FOREIGN KEY ([order_id]) REFERENCES [dbo].[orders] ([order_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[orders] ADD CONSTRAINT [fk_orders_restaurants_1] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurants] ([res_id])
GO
ALTER TABLE [dbo].[orders] ADD CONSTRAINT [fk_orders_customers_2] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id])
GO
ALTER TABLE [dbo].[payments] ADD CONSTRAINT [payments_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[payments] ADD CONSTRAINT [payments_reser_id_foreign] FOREIGN KEY ([reser_id]) REFERENCES [dbo].[reservations] ([reser_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[reservations] ADD CONSTRAINT [reservations_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[reservations] ADD CONSTRAINT [reservations_res_id_foreign] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurants] ([res_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[restaurantOwners] ADD CONSTRAINT [restaurantowners_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[restaurants] ADD CONSTRAINT [restaurants_cate_id_foreign] FOREIGN KEY ([cate_id]) REFERENCES [dbo].[categories] ([cate_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[restaurants] ADD CONSTRAINT [restaurants_resowner_id_foreign] FOREIGN KEY ([resOwner_id]) REFERENCES [dbo].[restaurantOwners] ([resOwner_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[vertifications] ADD CONSTRAINT [vertifications_res_id_foreign] FOREIGN KEY ([res_id]) REFERENCES [dbo].[restaurants] ([res_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[vouchers] ADD CONSTRAINT [vouchers_cus_id_foreign] FOREIGN KEY ([cus_id]) REFERENCES [dbo].[customers] ([cus_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

