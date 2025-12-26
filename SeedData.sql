-- ======================================
-- Homy Database Seeding Script (Final Verified)
-- ======================================
USE [HomeyProject];
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ======================================
-- 1. CLEANUP (Dependency Order)
-- ======================================
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

-- ======================================
-- 2. ROLES
-- ======================================
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @BrokerRoleId UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID());
ELSE SET @AdminRoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'User')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (@UserRoleId, 'User', 'USER', NEWID());
ELSE SET @UserRoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'User');

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Broker')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (@BrokerRoleId, 'Broker', 'BROKER', NEWID());
ELSE SET @BrokerRoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Broker');

-- ======================================
-- 3. USERS
-- ======================================
-- Model Check: FullName (Required), IsVerified (bool=false), IsActive (bool=true), IsDeleted (bool=false)
-- Pass@123
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @U1 UNIQUEIDENTIFIER = NEWID(); 
DECLARE @U2 UNIQUEIDENTIFIER = NEWID(); 
DECLARE @U3 UNIQUEIDENTIFIER = NEWID();
DECLARE @U4 UNIQUEIDENTIFIER = NEWID();
DECLARE @U5 UNIQUEIDENTIFIER = NEWID();

DECLARE @B1 UNIQUEIDENTIFIER = NEWID();
DECLARE @B2 UNIQUEIDENTIFIER = NEWID();
DECLARE @B3 UNIQUEIDENTIFIER = NEWID();
DECLARE @B4 UNIQUEIDENTIFIER = NEWID();
DECLARE @B5 UNIQUEIDENTIFIER = NEWID();

INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, IsActive, IsVerified, IsDeleted, CreatedAt)
VALUES 
(@AdminId, 'admin@homy.com', 'ADMIN@HOMY.COM', 'admin@homy.com', 'ADMIN@HOMY.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01000000000', 1, 0, 0, 0, N'System Admin', 1, 1, 0, GETDATE()),
(@U1, 'ahmed@gmail.com', 'AHMED@GMAIL.COM', 'ahmed@gmail.com', 'AHMED@GMAIL.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01111111111', 1, 0, 0, 0, N'Ahmed Mohamed', 1, 1, 0, GETDATE()),
(@U2, 'sara@gmail.com', 'SARA@GMAIL.COM', 'sara@gmail.com', 'SARA@GMAIL.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01222222222', 1, 0, 0, 0, N'Sara Hassan', 1, 1, 0, GETDATE()),
(@U3, 'mahmoud@gmail.com', 'MAHMOUD@GMAIL.COM', 'mahmoud@gmail.com', 'MAHMOUD@GMAIL.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01333333333', 1, 0, 0, 0, N'Mahmoud El-Ghandour', 1, 1, 0, GETDATE()),
(@U4, 'huda@gmail.com', 'HUDA@GMAIL.COM', 'huda@gmail.com', 'HUDA@GMAIL.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01444444444', 1, 0, 0, 0, N'Huda Nabil', 1, 1, 0, GETDATE()),
(@U5, 'tarek@gmail.com', 'TAREK@GMAIL.COM', 'tarek@gmail.com', 'TAREK@GMAIL.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01888888888', 1, 0, 0, 0, N'Tarek Aziz', 1, 1, 0, GETDATE()),
(@B1, 'omar@homy.com', 'OMAR@HOMY.COM', 'omar@homy.com', 'OMAR@HOMY.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01555555555', 1, 0, 0, 0, N'Omar El-Sayed', 1, 1, 0, GETDATE()),
(@B2, 'fatma@homy.com', 'FATMA@HOMY.COM', 'fatma@homy.com', 'FATMA@HOMY.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01666666666', 1, 0, 0, 0, N'Fatma Ahmed', 1, 1, 0, GETDATE()),
(@B3, 'khaled@homy.com', 'KHALED@HOMY.COM', 'khaled@homy.com', 'KHALED@HOMY.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01777777777', 1, 0, 0, 0, N'Khaled Mahmoud', 1, 1, 0, GETDATE()),
(@B4, 'youssef@homy.com', 'YOUSSEF@HOMY.COM', 'youssef@homy.com', 'YOUSSEF@HOMY.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01999999999', 1, 0, 0, 0, N'Youssef Kamel', 1, 1, 0, GETDATE()),
(@B5, 'mona@homy.com', 'MONA@HOMY.COM', 'mona@homy.com', 'MONA@HOMY.COM', 1, 'AQAAAAIAAYagAAAAEKk8qXGHBrL8DvBfxEh5WxJ3yMF5rNLVJQXZmKpwQZ9VHnFqTQJYPx2vR3FqZw==', NEWID(), NEWID(), '01010101010', 1, 0, 0, 0, N'Mona Zaki', 1, 1, 0, GETDATE());

INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES 
(@AdminId, @AdminRoleId),
(@U1, @UserRoleId), (@U2, @UserRoleId), (@U3, @UserRoleId), (@U4, @UserRoleId), (@U5, @UserRoleId),
(@B1, @BrokerRoleId), (@B2, @BrokerRoleId), (@B3, @BrokerRoleId), (@B4, @BrokerRoleId), (@B5, @BrokerRoleId);

-- ======================================
-- 4. LOCATIONS
-- ======================================
-- Model Check: City.Name (Req), District.Name (Req), CityId (Req)
INSERT INTO Cities (Name, NameEn, IsDeleted, CreatedAt, CreatedById) VALUES
(N'القاهرة', 'Cairo', 0, GETDATE(), @AdminId),
(N'الجيزة', 'Giza', 0, GETDATE(), @AdminId),
(N'الإسكندرية', 'Alexandria', 0, GETDATE(), @AdminId),
(N'الدقهلية', 'Dakahlia', 0, GETDATE(), @AdminId),
(N'الشرقية', 'Sharqia', 0, GETDATE(), @AdminId),
(N'البحيرة', 'Beheira', 0, GETDATE(), @AdminId),
(N'بورسعيد', 'Port Said', 0, GETDATE(), @AdminId),
(N'السويس', 'Suez', 0, GETDATE(), @AdminId),
(N'مطروح', 'Matrouh', 0, GETDATE(), @AdminId),
(N'البحر الأحمر', 'Red Sea', 0, GETDATE(), @AdminId);

DECLARE @Cairo BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Cairo');
DECLARE @Giza BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Giza');
DECLARE @Alex BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Alexandria');
DECLARE @Dakahlia BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Dakahlia');
DECLARE @Sharqia BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Sharqia');
DECLARE @Matrouh BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Matrouh');
DECLARE @RedSea BIGINT = (SELECT Id FROM Cities WHERE NameEn = 'Red Sea');

INSERT INTO Districts (Name, NameEn, CityId, IsDeleted, CreatedAt, CreatedById) VALUES
(N'مدينة نصر', 'Nasr City', @Cairo, 0, GETDATE(), @AdminId),
(N'المعادي', 'Maadi', @Cairo, 0, GETDATE(), @AdminId),
(N'الزمالك', 'Zamalek', @Cairo, 0, GETDATE(), @AdminId),
(N'التجمع الخامس', 'New Cairo', @Cairo, 0, GETDATE(), @AdminId),
(N'المقطم', 'Mokattam', @Cairo, 0, GETDATE(), @AdminId),
(N'شبرا', 'Shoubra', @Cairo, 0, GETDATE(), @AdminId),
(N'الرحاب', 'Al Rehab', @Cairo, 0, GETDATE(), @AdminId),
(N'مدينتي', 'Madinaty', @Cairo, 0, GETDATE(), @AdminId),
(N'المهندسين', 'Mohandessin', @Giza, 0, GETDATE(), @AdminId),
(N'الدقي', 'Dokki', @Giza, 0, GETDATE(), @AdminId),
(N'الشيخ زايد', 'Sheikh Zayed', @Giza, 0, GETDATE(), @AdminId),
(N'6 أكتوبر', '6th of October', @Giza, 0, GETDATE(), @AdminId),
(N'الهرم', 'Haram', @Giza, 0, GETDATE(), @AdminId),
(N'فيصل', 'Faisal', @Giza, 0, GETDATE(), @AdminId),
(N'سموحة', 'Smouha', @Alex, 0, GETDATE(), @AdminId),
(N'ميامي', 'Miami', @Alex, 0, GETDATE(), @AdminId),
(N'المنتزه', 'Montaza', @Alex, 0, GETDATE(), @AdminId),
(N'خالد بن الوليد', 'Khaled Ibn Al Walid', @Alex, 0, GETDATE(), @AdminId),
(N'المنصورة', 'Mansoura', @Dakahlia, 0, GETDATE(), @AdminId),
(N'طلخا', 'Talkha', @Dakahlia, 0, GETDATE(), @AdminId),
(N'الزقازيق', 'Zagazig', @Sharqia, 0, GETDATE(), @AdminId),
(N'العاشر من رمضان', '10th of Ramadan', @Sharqia, 0, GETDATE(), @AdminId),
(N'العلمين', 'Alamein', @Matrouh, 0, GETDATE(), @AdminId),
(N'الساحل الشمالي', 'North Coast', @Matrouh, 0, GETDATE(), @AdminId),
(N'الجونة', 'Gouna', @RedSea, 0, GETDATE(), @AdminId),
(N'الغردقة', 'Hurghada', @RedSea, 0, GETDATE(), @AdminId);

DECLARE @NasrCity BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'Nasr City');
DECLARE @NewCairo BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'New Cairo');
DECLARE @Maadi BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'Maadi');
DECLARE @Zayed BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'Sheikh Zayed');
DECLARE @October BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = '6th of October');
DECLARE @Mansoura BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'Mansoura');
DECLARE @NorthCoast BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'North Coast');
DECLARE @Gouna BIGINT = (SELECT TOP 1 Id FROM Districts WHERE NameEn = 'Gouna');

-- ======================================
-- 5. TYPES & AMENITIES
-- ======================================
-- Model Check: Name (Req)
INSERT INTO PropertyTypes (Name, NameEn, IconUrl, IsDeleted, CreatedAt, CreatedById) VALUES
(N'شقة', 'Apartment', '/icons/apt.svg', 0, GETDATE(), @AdminId),
(N'فيلا', 'Villa', '/icons/villa.svg', 0, GETDATE(), @AdminId),
(N'شاليه', 'Chalet', '/icons/chalet.svg', 0, GETDATE(), @AdminId),
(N'مكتب', 'Office', '/icons/office.svg', 0, GETDATE(), @AdminId),
(N'محل', 'Shop', '/icons/shop.svg', 0, GETDATE(), @AdminId),
(N'عيادة', 'Clinic', '/icons/clinic.svg', 0, GETDATE(), @AdminId),
(N'أرض زراعية', 'Agricultural Land', '/icons/agri.svg', 0, GETDATE(), @AdminId),
(N'مخزن', 'Warehouse', '/icons/warehouse.svg', 0, GETDATE(), @AdminId);

DECLARE @TApt BIGINT = (SELECT Id FROM PropertyTypes WHERE NameEn = 'Apartment');
DECLARE @TVilla BIGINT = (SELECT Id FROM PropertyTypes WHERE NameEn = 'Villa');
DECLARE @TChalet BIGINT = (SELECT Id FROM PropertyTypes WHERE NameEn = 'Chalet');
DECLARE @TOffice BIGINT = (SELECT Id FROM PropertyTypes WHERE NameEn = 'Office');
DECLARE @TAgri BIGINT = (SELECT Id FROM PropertyTypes WHERE NameEn = 'Agricultural Land');

INSERT INTO Amenities (Name, NameEn, IconUrl, IsDeleted, CreatedAt, CreatedById) VALUES
(N'حمام سباحة', 'Pool', 'fa-pool', 0, GETDATE(), @AdminId),
(N'حديقة', 'Garden', 'fa-leaf', 0, GETDATE(), @AdminId),
(N'أمن', 'Security', 'fa-shield', 0, GETDATE(), @AdminId),
(N'جراج', 'Garage', 'fa-car', 0, GETDATE(), @AdminId),
(N'مصعد', 'Elevator', 'fa-arrows-v', 0, GETDATE(), @AdminId),
(N'مفروش', 'Furnished', 'fa-bed', 0, GETDATE(), @AdminId),
(N'مكيف', 'AC', 'fa-snowflake', 0, GETDATE(), @AdminId),
(N'واي فاي', 'WiFi', 'fa-wifi', 0, GETDATE(), @AdminId);

-- ======================================
-- 6. PROJECTS & PACKAGES
-- ======================================
INSERT INTO Packages (Name, NameEn, Price, DurationDays, MaxProperties, MaxFeatured, CanBumpUp, IsDeleted, CreatedAt, CreatedById) VALUES
(N'مجانية', 'Free', 0, 30, 3, 0, 0, 0, GETDATE(), @AdminId),
(N'فضية', 'Silver', 200, 30, 10, 2, 0, 0, GETDATE(), @AdminId),
(N'ذهبية', 'Gold', 500, 30, 50, 10, 1, 0, GETDATE(), @AdminId);

INSERT INTO Projects (Name, NameEn, CityId, DistrictId, LocationDescription, LocationDescriptionEn, IsActive, IsDeleted, CreatedAt, CreatedById) VALUES
(N'ماونتن فيو', 'Mountain View', @Cairo, @NewCairo, N'التجمع الخامس', 'Fifth Settlement', 1, 0, GETDATE(), @AdminId),
(N'بالم هيلز', 'Palm Hills', @Giza, @Zayed, N'الشيخ زايد', 'Sheikh Zayed', 1, 0, GETDATE(), @AdminId),
(N'مراسي', 'Marassi', @Matrouh, @NorthCoast, N'سيدي عبد الرحمن', 'Sidi Abdelrahman', 1, 0, GETDATE(), @AdminId),
(N'الجونة', 'El Gouna', @RedSea, @Gouna, N'البحر الأحمر', 'Red Sea', 1, 0, GETDATE(), @AdminId);

DECLARE @ProjMV BIGINT = (SELECT TOP 1 Id FROM Projects WHERE Name LIKE N'ماونتن%');
DECLARE @ProjMra BIGINT = (SELECT TOP 1 Id FROM Projects WHERE Name LIKE N'مراسي%');

-- ======================================
-- 7. PROPERTIES (STRICTLY CHECKED)
-- ======================================
-- Required Columns Checked: Title, Price, PropertyTypeId, CityId, Purpose(enum), Status(enum), UserId, IsAgricultural, IsDeleted, IsFeatured
-- Defaulted Columns Explicitly Set: ViewCount(0), PhoneClicks(0), WhatsAppClicks(0)

-- BLOCK A: CAIRO APARTMENTS
INSERT INTO Properties (Title, TitleEn, Description, DescriptionEn, Price, PropertyTypeId, CityId, DistrictId, Purpose, Status, Area, Rooms, Bathrooms, FinishingType, UserId, IsDeleted, CreatedAt, CreatedById, IsAgricultural, ViewCount, PhoneClicks, WhatsAppClicks, IsFeatured) VALUES
(N'شقة لقطة بمدينة نصر', 'Hot Deal Apt in Nasr City', N'بجوار سيتي ستارز', 'Near City Stars', 2500000, @TApt, @Cairo, @NasrCity, 1, 1, 180, 3, 2, 2, @B1, 0, GETDATE(), @B1, 0, 10, 2, 1, 0),
(N'دوبلكس في التجمع', 'Duplex in New Cairo', N'تشطيب الترا مودرن', 'Ultra Modern Finishing', 6000000, @TApt, @Cairo, @NewCairo, 1, 1, 300, 4, 3, 2, @B1, 0, GETDATE(), @B1, 0, 55, 12, 8, 1),
(N'شقة للإيجار بالمعادي', 'Apt for Rent in Maadi', N'تطل على النيل', 'Nile View', 0, @TApt, @Cairo, @Maadi, 2, 1, 200, 3, 2, 1, @U1, 0, GETDATE(), @U1, 0, 4, 0, 0, 0),
(N'استوديو مفروش', 'Furnished Studio', N'قريب من الجامعة', 'Near AUC', 0, @TApt, @Cairo, @NewCairo, 2, 1, 70, 1, 1, 2, @B1, 0, GETDATE(), @B1, 0, 12, 5, 2, 0),
(N'شقة بحديقة في الرحاب', 'Garden Apt in Rehab', N'مرحلة رابعة', 'Phase 4', 4500000, @TApt, @Cairo, @NewCairo, 1, 1, 140, 2, 2, 1, @B1, 0, GETDATE(), @B1, 0, 23, 7, 3, 0);

UPDATE Properties SET RentPriceMonthly = 25000 WHERE TitleEn = 'Apt for Rent in Maadi';
UPDATE Properties SET RentPriceMonthly = 15000 WHERE TitleEn = 'Furnished Studio';

-- BLOCK B: GIZA VILLAS
INSERT INTO Properties (Title, TitleEn, Description, DescriptionEn, Price, PropertyTypeId, CityId, DistrictId, ProjectId, Purpose, Status, Area, Rooms, Bathrooms, FinishingType, UserId, IsDeleted, CreatedAt, CreatedById, IsAgricultural, ViewCount, PhoneClicks, WhatsAppClicks, IsFeatured) VALUES
(N'فيلا مستقلة الشيخ زايد', 'Standalone Villa Zayed', N'في كمبوند الربوة', 'Al Rabwa Compound', 15000000, @TVilla, @Giza, @Zayed, NULL, 1, 1, 600, 6, 5, 1, @B2, 0, GETDATE(), @B2, 0, 103, 30, 25, 1),
(N'تاون هاوس أكتوبر', 'Townhouse October', N'بالم هيلز جولف', 'Palm Hills Golf', 8000000, @TVilla, @Giza, @October, @ProjMV, 1, 1, 300, 4, 3, 0, @B2, 0, GETDATE(), @B2, 0, 45, 10, 5, 0),
(N'فيلا للإيجار', 'Villa for Rent', N'للسفارات والشركات', 'For Embassies', 0, @TVilla, @Giza, @Zayed, NULL, 2, 1, 1000, 8, 8, 2, @B2, 0, GETDATE(), @B2, 0, 80, 20, 10, 1),
(N'قصر صغير', 'Mini Palace', N'على الصحراوي مباشرة', 'Direct on Desert Road', 25000000, @TVilla, @Giza, @Zayed, NULL, 1, 1, 1200, 10, 10, 2, @B2, 0, GETDATE(), @B2, 0, 200, 40, 50, 1),
(N'توين هاوس', 'Twin House', N'تشطيب الشركة', 'Core & Shell', 6000000, @TVilla, @Giza, @October, NULL, 1, 1, 250, 3, 3, 0, @B2, 0, GETDATE(), @B2, 0, 30, 8, 4, 0);
UPDATE Properties SET RentPriceMonthly = 150000 WHERE TitleEn = 'Villa for Rent';

-- BLOCK C: COASTAL CHALETS
INSERT INTO Properties (Title, TitleEn, Description, DescriptionEn, Price, PropertyTypeId, CityId, DistrictId, ProjectId, Purpose, Status, Area, Rooms, Bathrooms, FinishingType, UserId, IsDeleted, CreatedAt, CreatedById, IsAgricultural, ViewCount, PhoneClicks, WhatsAppClicks, IsFeatured) VALUES
(N'شاليه مراسي فيردي', 'Chalet Marassi Verdi', N'يطل على اللاجون', 'Lagoon View', 12000000, @TChalet, @Matrouh, @NorthCoast, @ProjMra, 1, 1, 140, 3, 2, 2, @B3, 0, GETDATE(), @B3, 0, 90, 25, 12, 1),
(N'شاليه للإيجار الساحل', 'Chalet Rent North Coast', N'أمواج سيدي عبد الرحمن', 'Amwaj Sidi Abdelrahman', 0, @TChalet, @Matrouh, @NorthCoast, NULL, 2, 1, 100, 2, 1, 2, @B3, 0, GETDATE(), @B3, 0, 60, 15, 8, 1),
(N'شقة فندقية الجونة', 'Hotel Apt El Gouna', N'مارينا أبو تيج', 'Abu Tig Marina', 8000000, @TChalet, @RedSea, @Gouna, NULL, 1, 1, 90, 1, 1, 2, @AdminId, 0, GETDATE(), @AdminId, 0, 40, 10, 5, 0),
(N'فيلا الجونة', 'Gouna Villa', N'ويست جولف', 'West Golf', 30000000, @TVilla, @RedSea, @Gouna, NULL, 1, 1, 400, 5, 5, 2, @AdminId, 0, GETDATE(), @AdminId, 0, 120, 35, 20, 1),
(N'شاليه هاسييندا', 'Chalet Hacienda', N'موقع متميز', 'Prime Location', 11000000, @TChalet, @Matrouh, @NorthCoast, NULL, 1, 1, 150, 3, 2, 2, @B3, 0, GETDATE(), @B3, 0, 50, 12, 6, 0);
UPDATE Properties SET RentPriceMonthly = 55000 WHERE TitleEn = 'Chalet Rent North Coast';

-- BLOCK D: COMMERCIAL
INSERT INTO Properties (Title, TitleEn, Description, DescriptionEn, Price, PropertyTypeId, CityId, DistrictId, Purpose, Status, Area, UserId, IsDeleted, CreatedAt, CreatedById, IsAgricultural, ViewCount, PhoneClicks, WhatsAppClicks, IsFeatured) VALUES
(N'مكتب إداري التجمع', 'Office New Cairo', N'مول 5A', '5A by Waterway', 15000000, @TOffice, @Cairo, @NewCairo, 1, 1, 120, @B5, 0, GETDATE(), @B5, 0, 15, 2, 0, 0),
(N'محل للإيجار وسط البلد', 'Shop Rent Downtown', N'شارع طلعت حرب', 'Talaat Harb St', 0, @TOffice, @Cairo, @NasrCity, 2, 1, 50, @B5, 0, GETDATE(), @B5, 0, 25, 5, 1, 0),
(N'عيادة طبية', 'Medical Clinic', N'مركز طبي متكامل', 'Medical Center', 4000000, @TOffice, @Giza, @Zayed, 1, 1, 80, @B5, 0, GETDATE(), @B5, 0, 10, 1, 0, 0),
(N'مخزن للإيجار', 'Warehouse Rent', N'المنطقة الصناعية', 'Industrial Zone', 0, @TOffice, @Cairo, @NewCairo, 2, 1, 500, @B5, 0, GETDATE(), @B5, 0, 8, 2, 0, 0),
(N'كافيه مجهز', 'Equipped Cafe', N'بالمعدات', 'With Equipment', 4500000, @TOffice, @Alex, @NorthCoast, 1, 1, 100, @B3, 0, GETDATE(), @B3, 0, 45, 9, 3, 0);
UPDATE Properties SET RentPriceMonthly = 35000 WHERE TitleEn = 'Shop Rent Downtown';
UPDATE Properties SET RentPriceMonthly = 60000 WHERE TitleEn = 'Warehouse Rent';

-- BLOCK E: DELTA & LAND
INSERT INTO Properties (Title, TitleEn, Description, DescriptionEn, Price, PropertyTypeId, CityId, DistrictId, Purpose, Status, Area, UserId, IsDeleted, CreatedAt, CreatedById, IsAgricultural, ViewCount, PhoneClicks, WhatsAppClicks, IsFeatured) VALUES
(N'أرض زراعية خصبة', 'Fertile Land', N'ري بحاري', 'River Irrigation', 5000000, @TAgri, @Dakahlia, @Mansoura, 1, 1, 4200, @B4, 0, GETDATE(), @B4, 1, 33, 5, 2, 0),
(N'شقة المنصورة', 'Mansoura Apt', N'حي الجامعة', 'University District', 2200000, @TApt, @Dakahlia, @Mansoura, 1, 1, 135, @B4, 0, GETDATE(), @B4, 0, 18, 4, 1, 0),
(N'برج جديد', 'New Building', N'على النيل مباشرة', 'Nile View', 25000000, @TApt, @Dakahlia, @Mansoura, 1, 1, 1000, @B4, 0, GETDATE(), @B4, 0, 120, 15, 10, 1),
(N'مزرعة دواجن', 'Poultry Farm', N'مجهزة بالكامل', 'Fully Equipped', 8000000, @TAgri, @Sharqia, @Mansoura, 1, 1, 2000, @B4, 0, GETDATE(), @B4, 1, 22, 3, 1, 0),
(N'أرض مباني', 'Building Land', N'ترخيص 6 أدوار', 'License 6 Floors', 6000000, @TAgri, @Sharqia, @Mansoura, 1, 1, 300, @B4, 0, GETDATE(), @B4, 0, 40, 8, 4, 0);

-- ======================================
-- 8. LINKING IMAGES & AMENITIES
-- ======================================
INSERT INTO PropertyImages (PropertyId, ImageUrl, IsMain, SortOrder, IsDeleted, CreatedAt)
SELECT Id, '/images/properties/default-1.jpg', 1, 1, 0, GETDATE() FROM Properties;

INSERT INTO PropertyImages (PropertyId, ImageUrl, IsMain, SortOrder, IsDeleted, CreatedAt)
SELECT Id, '/images/properties/default-2.jpg', 0, 2, 0, GETDATE() FROM Properties;

-- Amenities
DECLARE @Pool BIGINT = (SELECT Id FROM Amenities WHERE NameEn = 'Pool');
DECLARE @WiFi BIGINT = (SELECT Id FROM Amenities WHERE NameEn = 'WiFi');
DECLARE @Sec BIGINT = (SELECT Id FROM Amenities WHERE NameEn = 'Security');

INSERT INTO PropertyAmenities (PropertyId, AmenityId, IsDeleted, CreatedAt, CreatedById)
SELECT p.Id, @Pool, 0, GETDATE(), @AdminId
FROM Properties p JOIN PropertyTypes pt ON p.PropertyTypeId = pt.Id WHERE pt.NameEn = 'Villa';

INSERT INTO PropertyAmenities (PropertyId, AmenityId, IsDeleted, CreatedAt, CreatedById)
SELECT Id, @Sec, 0, GETDATE(), @AdminId FROM Properties;

PRINT 'Final Verified Seeding Script Completed Successfully!';
GO
