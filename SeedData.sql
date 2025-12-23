-- ======================================
-- Homy Database Seeding Script
-- ======================================
-- تأكد من تشغيل هذا السكريبت على قاعدة البيانات الصحيحة

USE [HomyDB]; -- غير اسم قاعدة البيانات حسب الحاجة
GO

-- ======================================
-- 1. تنظيف البيانات القديمة (إختياري)
-- ======================================
-- تحذير: هذا سيحذف جميع البيانات الموجودة
-- احذف التعليق من السطور التالية إذا كنت تريد البدء من الصفر
/*
DELETE FROM PropertyAmenities;
DELETE FROM PropertyImages;
DELETE FROM PropertyReviews;
DELETE FROM SavedProperties;
DELETE FROM Notifications;
DELETE FROM Properties;
DELETE FROM UserSubscriptions;
DELETE FROM Projects;
DELETE FROM Districts;
DELETE FROM Cities;
DELETE FROM PropertyTypes;
DELETE FROM Amenities;
DELETE FROM Packages;
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUsers;
DELETE FROM AspNetRoles;
*/

-- ======================================
-- 2. إدراج الأدوار (Roles)
-- ======================================
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @BrokerRoleId UNIQUEIDENTIFIER = NEWID();

INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
    (@AdminRoleId, 'Admin', 'ADMIN', NEWID()),
    (@UserRoleId, 'User', 'USER', NEWID()),
    (@BrokerRoleId, 'Broker', 'BROKER', NEWID());

-- ======================================
-- 3. إدراج المستخدمين (Users)
-- ======================================
-- ملاحظة: كلمات المرور هنا هاشد للنص "Pass@123"
-- Hash: AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==

DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @User1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Broker1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Broker2Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO AspNetUsers (
    Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
    PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
    TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, WhatsAppNumber,
    IsActive, IsVerified, IsDeleted, CreatedAt, ProfileImageUrl
)
VALUES 
    -- Admin User
    (@AdminUserId, 'admin@homy.com', 'ADMIN@HOMY.COM', 'admin@homy.com', 'ADMIN@HOMY.COM', 1,
     'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==',
     NEWID(), NEWID(), '01000000000', 1, 0, 0, 0, N'مدير النظام', '01000000000', 1, 1, 0, GETDATE(), NULL),
    
    -- Regular User 1
    (@User1Id, 'ahmed.mohamed@gmail.com', 'AHMED.MOHAMED@GMAIL.COM', 'ahmed.mohamed@gmail.com', 'AHMED.MOHAMED@GMAIL.COM', 1,
     'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==',
     NEWID(), NEWID(), '01111111111', 1, 0, 0, 0, N'أحمد محمد علي', '01111111111', 1, 1, 0, GETDATE(), NULL),
    
    -- Regular User 2
    (@User2Id, 'sara.hassan@gmail.com', 'SARA.HASSAN@GMAIL.COM', 'sara.hassan@gmail.com', 'SARA.HASSAN@GMAIL.COM', 1,
     'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==',
     NEWID(), NEWID(), '01222222222', 1, 0, 0, 0, N'سارة حسن إبراهيم', '01222222222', 1, 1, 0, GETDATE(), NULL),
    
    -- Broker 1
    (@Broker1Id, 'broker.omar@homy.com', 'BROKER.OMAR@HOMY.COM', 'broker.omar@homy.com', 'BROKER.OMAR@HOMY.COM', 1,
     'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==',
     NEWID(), NEWID(), '01555555555', 1, 0, 0, 0, N'عمر السيد - سمسار عقاري', '01555555555', 1, 1, 0, GETDATE(), NULL),
    
    -- Broker 2
    (@Broker2Id, 'broker.fatma@homy.com', 'BROKER.FATMA@HOMY.COM', 'broker.fatma@homy.com', 'BROKER.FATMA@HOMY.COM', 1,
     'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==',
     NEWID(), NEWID(), '01666666666', 1, 0, 0, 0, N'فاطمة أحمد - وسيط عقاري', '01666666666', 1, 1, 0, GETDATE(), NULL);

-- ربط المستخدمين بالأدوار
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES 
    (@AdminUserId, @AdminRoleId),
    (@User1Id, @UserRoleId),
    (@User2Id, @UserRoleId),
    (@Broker1Id, @BrokerRoleId),
    (@Broker2Id, @BrokerRoleId);

-- ======================================
-- 4. إدراج المدن (Cities)
-- ======================================
INSERT INTO Cities (Name, NameEn, IsDeleted, CreatedAt, CreatedById)
VALUES 
    (N'القاهرة', 'Cairo', 0, GETDATE(), @AdminUserId),
    (N'الجيزة', 'Giza', 0, GETDATE(), @AdminUserId),
    (N'الإسكندرية', 'Alexandria', 0, GETDATE(), @AdminUserId),
    (N'الشرقية', 'Sharqia', 0, GETDATE(), @AdminUserId),
    (N'المنوفية', 'Monufia', 0, GETDATE(), @AdminUserId),
    (N'القليوبية', 'Qalyubia', 0, GETDATE(), @AdminUserId),
    (N'البحيرة', 'Beheira', 0, GETDATE(), @AdminUserId),
    (N'الدقهلية', 'Dakahlia', 0, GETDATE(), @AdminUserId);

-- الحصول على معرفات المدن
DECLARE @CairoId BIGINT = (SELECT Id FROM Cities WHERE Name = N'القاهرة');
DECLARE @GizaId BIGINT = (SELECT Id FROM Cities WHERE Name = N'الجيزة');
DECLARE @AlexId BIGINT = (SELECT Id FROM Cities WHERE Name = N'الإسكندرية');

-- ======================================
-- 5. إدراج الأحياء/المناطق (Districts)
-- ======================================
INSERT INTO Districts (Name, NameEn, CityId, IsDeleted, CreatedAt, CreatedById)
VALUES 
    -- أحياء القاهرة
    (N'مدينة نصر', 'Nasr City', @CairoId, 0, GETDATE(), @AdminUserId),
    (N'المعادي', 'Maadi', @CairoId, 0, GETDATE(), @AdminUserId),
    (N'الزمالك', 'Zamalek', @CairoId, 0, GETDATE(), @AdminUserId),
    (N'مصر الجديدة', 'Heliopolis', @CairoId, 0, GETDATE(), @AdminUserId),
    (N'التجمع الخامس', 'Fifth Settlement', @CairoId, 0, GETDATE(), @AdminUserId),
    (N'الرحاب', 'Rehab', @CairoId, 0, GETDATE(), @AdminUserId),
    (N'القاهرة الجديدة', 'New Cairo', @CairoId, 0, GETDATE(), @AdminUserId),
    
    -- أحياء الجيزة
    (N'الدقي', 'Dokki', @GizaId, 0, GETDATE(), @AdminUserId),
    (N'المهندسين', 'Mohandessin', @GizaId, 0, GETDATE(), @AdminUserId),
    (N'الهرم', 'Haram', @GizaId, 0, GETDATE(), @AdminUserId),
    (N'فيصل', 'Faisal', @GizaId, 0, GETDATE(), @AdminUserId),
    (N'6 أكتوبر', '6th of October', @GizaId, 0, GETDATE(), @AdminUserId),
    (N'الشيخ زايد', 'Sheikh Zayed', @GizaId, 0, GETDATE(), @AdminUserId),
    
    -- أحياء الإسكندرية
    (N'سموحة', 'Smouha', @AlexId, 0, GETDATE(), @AdminUserId),
    (N'سيدي جابر', 'Sidi Gaber', @AlexId, 0, GETDATE(), @AdminUserId),
    (N'ميامي', 'Miami', @AlexId, 0, GETDATE(), @AdminUserId),
    (N'العجمي', 'Agami', @AlexId, 0, GETDATE(), @AdminUserId);

-- الحصول على معرفات الأحياء
DECLARE @NasrCityId BIGINT = (SELECT Id FROM Districts WHERE Name = N'مدينة نصر');
DECLARE @MaadiId BIGINT = (SELECT Id FROM Districts WHERE Name = N'المعادي');
DECLARE @NewCairoId BIGINT = (SELECT Id FROM Districts WHERE Name = N'القاهرة الجديدة');
DECLARE @SheikhZayedId BIGINT = (SELECT Id FROM Districts WHERE Name = N'الشيخ زايد');

-- ======================================
-- 6. إدراج أنواع العقارات (Property Types)
-- ======================================
INSERT INTO PropertyTypes (Name, NameEn, IconUrl, IsDeleted, CreatedAt, CreatedById)
VALUES 
    (N'شقة', 'Apartment', '/images/icons/apartment.svg', 0, GETDATE(), @AdminUserId),
    (N'فيلا', 'Villa', '/images/icons/villa.svg', 0, GETDATE(), @AdminUserId),
    (N'محل تجاري', 'Shop', '/images/icons/shop.svg', 0, GETDATE(), @AdminUserId),
    (N'مكتب', 'Office', '/images/icons/office.svg', 0, GETDATE(), @AdminUserId),
    (N'قطعة أرض', 'Land', '/images/icons/land.svg', 0, GETDATE(), @AdminUserId),
    (N'عمارة', 'Building', '/images/icons/building.svg', 0, GETDATE(), @AdminUserId),
    (N'دوبلكس', 'Duplex', '/images/icons/duplex.svg', 0, GETDATE(), @AdminUserId),
    (N'بنتهاوس', 'Penthouse', '/images/icons/penthouse.svg', 0, GETDATE(), @AdminUserId),
    (N'شاليه', 'Chalet', '/images/icons/chalet.svg', 0, GETDATE(), @AdminUserId);

-- الحصول على معرفات أنواع العقارات
DECLARE @ApartmentTypeId BIGINT = (SELECT Id FROM PropertyTypes WHERE Name = N'شقة');
DECLARE @VillaTypeId BIGINT = (SELECT Id FROM PropertyTypes WHERE Name = N'فيلا');
DECLARE @ShopTypeId BIGINT = (SELECT Id FROM PropertyTypes WHERE Name = N'محل تجاري');
DECLARE @LandTypeId BIGINT = (SELECT Id FROM PropertyTypes WHERE Name = N'قطعة أرض');

-- ======================================
-- 7. إدراج الميزات/المرافق (Amenities)
-- ======================================
INSERT INTO Amenities (Name, NameEn, IconUrl, IsDeleted, CreatedAt, CreatedById)
VALUES 
    (N'مسبح', 'Swimming Pool', 'fa-swimming-pool', 0, GETDATE(), @AdminUserId),
    (N'حديقة', 'Garden', 'fa-leaf', 0, GETDATE(), @AdminUserId),
    (N'مصعد', 'Elevator', 'fa-elevator', 0, GETDATE(), @AdminUserId),
    (N'موقف سيارات', 'Parking', 'fa-car', 0, GETDATE(), @AdminUserId),
    (N'أمن وحراسة', 'Security', 'fa-shield-alt', 0, GETDATE(), @AdminUserId),
    (N'صالة رياضية', 'Gym', 'fa-dumbbell', 0, GETDATE(), @AdminUserId),
    (N'تكييف مركزي', 'Central AC', 'fa-snowflake', 0, GETDATE(), @AdminUserId),
    (N'غاز طبيعي', 'Natural Gas', 'fa-fire', 0, GETDATE(), @AdminUserId),
    (N'إنترنت', 'Internet', 'fa-wifi', 0, GETDATE(), @AdminUserId),
    (N'بلكونة', 'Balcony', 'fa-building', 0, GETDATE(), @AdminUserId),
    (N'مفروش', 'Furnished', 'fa-couch', 0, GETDATE(), @AdminUserId),
    (N'مطبخ مجهز', 'Kitchen', 'fa-utensils', 0, GETDATE(), @AdminUserId);

-- ======================================
-- 8. إدراج الباقات (Packages)
-- ======================================
INSERT INTO Packages (
    Name, Price, DurationDays, MaxProperties, MaxFeatured, CanBumpUp,
    IsDeleted, CreatedAt, CreatedById
)
VALUES 
    (N'باقة مجانية', 0.00, 30, 3, 0, 0, 0, GETDATE(), @AdminUserId),
    (N'باقة برونزية', 199.00, 30, 10, 2, 1, 0, GETDATE(), @AdminUserId),
    (N'باقة فضية', 399.00, 30, 25, 5, 1, 0, GETDATE(), @AdminUserId),
    (N'باقة ذهبية', 699.00, 30, 50, 10, 1, 0, GETDATE(), @AdminUserId),
    (N'باقة بلاتينيوم', 1299.00, 30, 100, 20, 1, 0, GETDATE(), @AdminUserId);

-- الحصول على معرفات الباقات
DECLARE @FreePackageId BIGINT = (SELECT Id FROM Packages WHERE Name = N'باقة مجانية');
DECLARE @BronzePackageId BIGINT = (SELECT Id FROM Packages WHERE Name = N'باقة برونزية');
DECLARE @GoldPackageId BIGINT = (SELECT Id FROM Packages WHERE Name = N'باقة ذهبية');

-- ======================================
-- 9. إدراج مشاريع/كمبوندات (Projects)
-- ======================================
INSERT INTO Projects (
    Name, CityId, DistrictId, LocationDescription, LogoUrl, CoverImageUrl,
    IsActive, IsDeleted, CreatedAt, CreatedById
)
VALUES 
    (N'كمبوند الرحاب', @CairoId, @NewCairoId, N'موقع متميز بالقرب من الجامعة الأمريكية',
     '/images/projects/rehab-logo.png', '/images/projects/rehab-cover.jpg',
     1, 0, GETDATE(), @AdminUserId),
    
    (N'كمبوند بيفرلي هيلز', @GizaId, @SheikhZayedId, N'على طريق الواحات مباشرة',
     '/images/projects/beverly-logo.png', '/images/projects/beverly-cover.jpg',
     1, 0, GETDATE(), @AdminUserId),
    
    (N'كمبوند مونت جلالة', @CairoId, @NewCairoId, N'منتجع سياحي متكامل على البحر الأحمر',
     '/images/projects/galala-logo.png', '/images/projects/galala-cover.jpg',
     1, 0, GETDATE(), @AdminUserId);

DECLARE @RehabProjectId BIGINT = (SELECT Id FROM Projects WHERE Name = N'كمبوند الرحاب');
DECLARE @BeverlyProjectId BIGINT = (SELECT Id FROM Projects WHERE Name = N'كمبوند بيفرلي هيلز');

-- ======================================
-- 10. إدراج العقارات (Properties)
-- ======================================
-- عقار 1: شقة للبيع في مدينة نصر
DECLARE @Property1Id BIGINT;
INSERT INTO Properties (
    Title, Description, Price, PropertyTypeId, CityId, DistrictId, Purpose, Status,
    Area, Rooms, Bathrooms, FloorNumber, FinishingType, IsAgricultural, UserId, IsDeleted, IsFeatured,
    ViewCount, PhoneClicks, WhatsAppClicks, CreatedAt, CreatedById
)
VALUES (
    N'شقة فاخرة للبيع في منطقة راقية بمدينة نصر',
    N'شقة ممتازة بمساحة 150 متر مربع، 3 غرف نوم، 2 حمام، تشطيب سوبر لوكس، الدور الخامس، فيو رائع، قريبة من جميع الخدمات',
    2500000.00, @ApartmentTypeId, @CairoId, @NasrCityId, 0, 1,
    150, 3, 2, 5, 2, 0, @Broker1Id, 0, 1,
    45, 12, 8, GETDATE(), @Broker1Id
);
SET @Property1Id = SCOPE_IDENTITY();

-- عقار 2: فيلا للبيع في الشيخ زايد
DECLARE @Property2Id BIGINT;
INSERT INTO Properties (
    Title, Description, Price, PropertyTypeId, CityId, DistrictId, ProjectId, Purpose, Status,
    Area, Rooms, Bathrooms, FinishingType, IsAgricultural, UserId, IsDeleted, IsFeatured,
    ViewCount, PhoneClicks, WhatsAppClicks, CreatedAt, CreatedById
)
VALUES (
    N'فيلا مستقلة للبيع في كمبوند بيفرلي هيلز',
    N'فيلا على مساحة 400 متر مربع، 5 غرف نوم، 4 حمامات، حديقة خاصة 200 متر، مسبح خاص، تشطيب ألترا لوكس',
    8500000.00, @VillaTypeId, @GizaId, @SheikhZayedId, @BeverlyProjectId, 0, 1,
    400, 5, 4, 3, 0, @Broker2Id, 0, 1,
    78, 25, 18, GETDATE(), @Broker2Id
);
SET @Property2Id = SCOPE_IDENTITY();

-- عقار 3: شقة للإيجار في المعادي
DECLARE @Property3Id BIGINT;
INSERT INTO Properties (
    Title, Description, Price, RentPriceMonthly, PropertyTypeId, CityId, DistrictId, Purpose, Status,
    Area, Rooms, Bathrooms, FloorNumber, FinishingType, IsAgricultural, UserId, IsDeleted, IsFeatured,
    ViewCount, PhoneClicks, WhatsAppClicks, CreatedAt, CreatedById
)
VALUES (
    N'شقة مفروشة للإيجار في المعادي',
    N'شقة مفروشة بالكامل 120 متر، 2 غرفة نوم، حمامين، مطبخ مجهز، تكييف، قريبة من المترو',
    3500000.00, 8000.00, @ApartmentTypeId, @CairoId, @MaadiId, 1, 1,
    120, 2, 2, 3, 2, 0, @User1Id, 0, 0,
    23, 7, 5, GETDATE(), @User1Id
);
SET @Property3Id = SCOPE_IDENTITY();

-- عقار 4: محل تجاري للإيجار
DECLARE @Property4Id BIGINT;
INSERT INTO Properties (
    Title, Description, Price, RentPriceMonthly, PropertyTypeId, CityId, DistrictId, Purpose, Status,
    Area, IsAgricultural, UserId, IsDeleted, IsFeatured,
    ViewCount, PhoneClicks, WhatsAppClicks, CreatedAt, CreatedById
)
VALUES (
    N'محل تجاري للإيجار في موقع حيوي',
    N'محل بمساحة 50 متر على شارع رئيسي، واجهة زجاجية، مناسب لجميع الأنشطة التجارية',
    1500000.00, 15000.00, @ShopTypeId, @CairoId, @NasrCityId, 1, 1,
    50, 0, @Broker1Id, 0, 0,
    34, 15, 10, GETDATE(), @Broker1Id
);
SET @Property4Id = SCOPE_IDENTITY();

-- عقار 5: أرض زراعية للبيع
DECLARE @Property5Id BIGINT;
INSERT INTO Properties (
    Title, Description, Price, PropertyTypeId, CityId, DistrictId, Purpose, Status,
    Area, IsAgricultural, UserId, IsDeleted, IsFeatured,
    ViewCount, PhoneClicks, WhatsAppClicks, CreatedAt, CreatedById
)
VALUES (
    N'أرض زراعية للبيع 5 فدان',
    N'أرض زراعية خصبة بمساحة 5 فدان، متوفر بها بئر مياه، على طريق إسفلت',
    5000000.00, @LandTypeId, @GizaId, @SheikhZayedId, 0, 1,
    21000, 1, @Broker2Id, 0, 0,
    56, 20, 12, GETDATE(), @Broker2Id
);
SET @Property5Id = SCOPE_IDENTITY();

-- ======================================
-- 11. إدراج صور العقارات (Property Images)
-- ======================================
INSERT INTO PropertyImages (PropertyId, ImageUrl, IsMain, SortOrder, IsDeleted, CreatedAt, CreatedById)
VALUES 
    -- صور العقار 1
    (@Property1Id, '/images/properties/apt1-main.jpg', 1, 1, 0, GETDATE(), @Broker1Id),
    (@Property1Id, '/images/properties/apt1-living.jpg', 0, 2, 0, GETDATE(), @Broker1Id),
    (@Property1Id, '/images/properties/apt1-kitchen.jpg', 0, 3, 0, GETDATE(), @Broker1Id),
    (@Property1Id, '/images/properties/apt1-bedroom.jpg', 0, 4, 0, GETDATE(), @Broker1Id),
    
    -- صور العقار 2
    (@Property2Id, '/images/properties/villa1-main.jpg', 1, 1, 0, GETDATE(), @Broker2Id),
    (@Property2Id, '/images/properties/villa1-garden.jpg', 0, 2, 0, GETDATE(), @Broker2Id),
    (@Property2Id, '/images/properties/villa1-pool.jpg', 0, 3, 0, GETDATE(), @Broker2Id),
    
    -- صور العقار 3
    (@Property3Id, '/images/properties/apt2-main.jpg', 1, 1, 0, GETDATE(), @User1Id),
    (@Property3Id, '/images/properties/apt2-view.jpg', 0, 2, 0, GETDATE(), @User1Id);

-- ======================================
-- 12. إدراج مميزات العقارات (Property Amenities)
-- ======================================
DECLARE @PoolId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'مسبح');
DECLARE @GardenId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'حديقة');
DECLARE @ElevatorId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'مصعد');
DECLARE @ParkingId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'موقف سيارات');
DECLARE @SecurityId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'أمن وحراسة');
DECLARE @ACId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'تكييف مركزي');
DECLARE @InternetId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'إنترنت');
DECLARE @FurnishedId BIGINT = (SELECT Id FROM Amenities WHERE Name = N'مفروش');

INSERT INTO PropertyAmenities (PropertyId, AmenityId, IsDeleted, CreatedAt, CreatedById)
VALUES 
    -- مميزات العقار 1
    (@Property1Id, @ElevatorId, 0, GETDATE(), @Broker1Id),
    (@Property1Id, @ParkingId, 0, GETDATE(), @Broker1Id),
    (@Property1Id, @SecurityId, 0, GETDATE(), @Broker1Id),
    (@Property1Id, @ACId, 0, GETDATE(), @Broker1Id),
    
    -- مميزات العقار 2 (فيلا)
    (@Property2Id, @PoolId, 0, GETDATE(), @Broker2Id),
    (@Property2Id, @GardenId, 0, GETDATE(), @Broker2Id),
    (@Property2Id, @ParkingId, 0, GETDATE(), @Broker2Id),
    (@Property2Id, @SecurityId, 0, GETDATE(), @Broker2Id),
    (@Property2Id, @ACId, 0, GETDATE(), @Broker2Id),
    
    -- مميزات العقار 3
    (@Property3Id, @ElevatorId, 0, GETDATE(), @User1Id),
    (@Property3Id, @FurnishedId, 0, GETDATE(), @User1Id),
    (@Property3Id, @InternetId, 0, GETDATE(), @User1Id);

-- ======================================
-- 13. إدراج اشتراكات المستخدمين (User Subscriptions)
-- ======================================
INSERT INTO UserSubscriptions (
    UserId, PackageId, StartDate, EndDate, AmountPaid, PaymentMethod,
    IsActive, IsDeleted, CreatedAt, CreatedById
)
VALUES 
    -- اشتراك Broker1 - باقة ذهبية
    (@Broker1Id, @GoldPackageId, GETDATE(), DATEADD(DAY, 30, GETDATE()), 699.00, N'بطاقة ائتمان',
     1, 0, GETDATE(), @AdminUserId),
    
    -- اشتراك Broker2 - باقة ذهبية
    (@Broker2Id, @GoldPackageId, GETDATE(), DATEADD(DAY, 30, GETDATE()), 699.00, N'فودافون كاش',
     1, 0, GETDATE(), @AdminUserId),
    
    -- اشتراك User1 - باقة مجانية
    (@User1Id, @FreePackageId, GETDATE(), DATEADD(DAY, 30, GETDATE()), 0.00, N'مجاني',
     1, 0, GETDATE(), @AdminUserId);

-- ======================================
-- 14. إدراج العقارات المحفوظة (Saved Properties)
-- ======================================
INSERT INTO SavedProperties (UserId, PropertyId, SavedAt, IsDeleted, CreatedAt, CreatedById)
VALUES 
    (@User1Id, @Property2Id, GETDATE(), 0, GETDATE(), @User1Id),
    (@User2Id, @Property1Id, GETDATE(), 0, GETDATE(), @User2Id),
    (@User2Id, @Property2Id, GETDATE(), 0, GETDATE(), @User2Id);

-- ======================================
-- 15. إدراج الإشعارات (Notifications)
-- ======================================
INSERT INTO Notifications (
    UserId, PropertyId, Title, Message, Type, IsRead,
    IsDeleted, CreatedAt, CreatedById
)
VALUES 
    (@Broker1Id, @Property1Id, N'إعلانك تم تفعيله', N'تم تفعيل إعلانك للشقة في مدينة نصر بنجاح', 1, 0,
     0, GETDATE(), @AdminUserId),
    
    (@User1Id, @Property2Id, N'عقار جديد في منطقتك المفضلة', N'تم إضافة فيلا جديدة في الشيخ زايد', 0, 0,
     0, GETDATE(), @AdminUserId),
    
    (@Broker2Id, @Property2Id, N'إعلانك حقق 50 مشاهدة', N'إعلان الفيلا الخاصة بك حقق 50 مشاهدة', 2, 1,
     0, DATEADD(DAY, -1, GETDATE()), @AdminUserId);

-- ======================================
-- 16. إدراج مراجعات العقارات (Property Reviews)
-- ======================================
INSERT INTO PropertyReviews (
    PropertyId, AdminId, Action, Message,
    IsDeleted, CreatedAt, CreatedById
)
VALUES 
    (@Property1Id, @AdminUserId, 1, N'تم قبول الإعلان بعد المراجعة',
     0, GETDATE(), @AdminUserId),
    
    (@Property2Id, @AdminUserId, 1, N'إعلان ممتاز، تم الموافقة',
     0, GETDATE(), @AdminUserId);

-- ======================================
-- النهاية
-- ======================================
PRINT 'تم إدخال البيانات بنجاح!';
PRINT '';
PRINT 'ملخص البيانات المضافة:';
PRINT '- عدد الأدوار: 3';
PRINT '- عدد المستخدمين: 5 (1 مدير، 2 مستخدم، 2 سمسار)';
PRINT '- عدد المدن: 8';
PRINT '- عدد الأحياء: 17';
PRINT '- عدد أنواع العقارات: 9';
PRINT '- عدد الميزات: 12';
PRINT '- عدد الباقات: 5';
PRINT '- عدد المشاريع: 3';
PRINT '- عدد العقارات: 5';
PRINT '- عدد صور العقارات: 9';
PRINT '- عدد مميزات العقارات: 12';
PRINT '- عدد الاشتراكات: 3';
PRINT '- عدد العقارات المحفوظة: 3';
PRINT '- عدد الإشعارات: 3';
PRINT '- عدد المراجعات: 2';
PRINT '';
PRINT 'معلومات تسجيل الدخول:';
PRINT 'Admin: admin@homy.com / Pass@123';
PRINT 'User 1: ahmed.mohamed@gmail.com / Pass@123';
PRINT 'User 2: sara.hassan@gmail.com / Pass@123';
PRINT 'Broker 1: broker.omar@homy.com / Pass@123';
PRINT 'Broker 2: broker.fatma@homy.com / Pass@123';
GO
