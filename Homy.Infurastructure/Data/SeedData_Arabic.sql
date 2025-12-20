-- ================================================================
-- Homy Project - Arabic Test Data
-- ================================================================
-- هذا السكريبت يحتوي على بيانات تجريبية باللغة العربية
-- يمكن تشغيله مباشرة على SQL Server بدون تضارب
-- ================================================================

USE HomeyProject;
GO

-- Set required options for SQL Server
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
GO

-- ================================================================
-- 1. المستخدمين (Users)
-- ================================================================

-- مسح البيانات القديمة (اختياري - احذف هذا القسم إذا كنت تريد الإضافة فقط)
-- DELETE FROM PropertyReviews;
-- DELETE FROM Notifications;
-- DELETE FROM SavedProperties;
-- DELETE FROM PropertyAmenities;
-- DELETE FROM PropertyImages;
-- DELETE FROM Properties;
-- DELETE FROM UserSubscriptions;
-- DELETE FROM Projects;
-- DELETE FROM Districts;
-- DELETE FROM Cities;
-- DELETE FROM Amenities;
-- DELETE FROM PropertyTypes;
-- DELETE FROM Packages;
-- DELETE FROM AspNetUsers;

-- إدراج المستخدمين
INSERT INTO AspNetUsers (Id, FullName, Email, NormalizedEmail, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, 
                         SecurityStamp, ConcurrencyStamp, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
                         IsDeleted, Role, IsVerified, IsActive, CreatedAt, PasswordHash, UserName, NormalizedUserName)
VALUES
-- أدمن
('11111111-1111-1111-1111-111111111111', N'أحمد محمد الإداري', 'admin@homy.com', 'ADMIN@HOMY.COM', '01000000001', 
 1, 1, NEWID(), NEWID(), 0, 1, 0, 0, 3, 1, 1, GETDATE(), 
 'AQAAAAIAAYagAAAAEJ3VHDWQZQd4c8kJVqJYJb0mXLcPuZwZPqvI5wHQqN/hUZFPQmFpY7bQKEGqLw==', -- كلمة المرور: Admin@123
 '01000000001', '01000000001'),
 
-- ملاك عقارات
('22222222-2222-2222-2222-222222222222', N'محمود سعيد العقاري', 'mahmoud@example.com', 'MAHMOUD@EXAMPLE.COM', '01100000002',
 1, 1, NEWID(), NEWID(), 0, 1, 0, 0, 1, 1, 1, GETDATE(),
 'AQAAAAIAAYagAAAAEJ3VHDWQZQd4c8kJVqJYJb0mXLcPuZwZPqvI5wHQqN/hUZFPQmFpY7bQKEGqLw==',
 '01100000002', '01100000002'),
 
('33333333-3333-3333-3333-333333333333', N'فاطمة حسن المالك', 'fatma@example.com', 'FATMA@EXAMPLE.COM', '01200000003',
 1, 1, NEWID(), NEWID(), 0, 1, 0, 0, 1, 1, 1, GETDATE(),
 'AQAAAAIAAYagAAAAEJ3VHDWQZQd4c8kJVqJYJb0mXLcPuZwZPqvI5wHQqN/hUZFPQmFpY7bQKEGqLw==',
 '01200000003', '01200000003'),
 
('44444444-4444-4444-4444-444444444444', N'خالد علي الصاوي', 'khaled@example.com', 'KHALED@EXAMPLE.COM', '01500000004',
 1, 1, NEWID(), NEWID(), 0, 1, 0, 0, 1, 1, 1, GETDATE(),
 'AQAAAAIAAYagAAAAEJ3VHDWQZQd4c8kJVqJYJb0mXLcPuZwZPqvI5wHQqN/hUZFPQmFpY7bQKEGqLw==',
 '01500000004', '01500000004'),

-- وكلاء عقاريين
('55555555-5555-5555-5555-555555555555', N'مريم عبدالله الوكيل', 'mariam@example.com', 'MARIAM@EXAMPLE.COM', '01000000005',
 1, 1, NEWID(), NEWID(), 0, 1, 0, 0, 2, 1, 1, GETDATE(),
 'AQAAAAIAAYagAAAAEJ3VHDWQZQd4c8kJVqJYJb0mXLcPuZwZPqvI5wHQqN/hUZFPQmFpY7bQKEGqLw==',
 '01000000005', '01000000005'),
 
('66666666-6666-6666-6666-666666666666', N'عمر حسين السمسار', 'omar@example.com', 'OMAR@EXAMPLE.COM', '01100000006',
 1, 1, NEWID(), NEWID(), 0, 1, 0, 0, 2, 1, 1, GETDATE(),
 'AQAAAAIAAYagAAAAEJ3VHDWQZQd4c8kJVqJYJb0mXLcPuZwZPqvI5wHQqN/hUZFPQmFpY7bQKEGqLw==',
 '01100000006', '01100000006');

-- ================================================================
-- 2. المدن (Cities)
-- ================================================================
SET IDENTITY_INSERT Cities ON;

INSERT INTO Cities (Id, Name, CreatedAt, IsDeleted)
VALUES
(1, N'القاهرة', GETDATE(), 0),
(2, N'الإسكندرية', GETDATE(), 0),
(3, N'الجيزة', GETDATE(), 0),
(4, N'الشرقية', GETDATE(), 0),
(5, N'المنيا', GETDATE(), 0);

SET IDENTITY_INSERT Cities OFF;

-- ================================================================
-- 3. الأحياء (Districts)
-- ================================================================
SET IDENTITY_INSERT Districts ON;

INSERT INTO Districts (Id, CityId, Name, CreatedAt, IsDeleted)
VALUES
-- القاهرة
(1, 1, N'مصر الجديدة', GETDATE(), 0),
(2, 1, N'المعادي', GETDATE(), 0),
(3, 1, N'مدينة نصر', GETDATE(), 0),
(4, 1, N'التجمع الخامس', GETDATE(), 0),
(5, 1, N'الرحاب', GETDATE(), 0),
-- الإسكندرية
(6, 2, N'سموحة', GETDATE(), 0),
(7, 2, N'المندرة', GETDATE(), 0),
(8, 2, N'سيدي جابر', GETDATE(), 0),
-- الجيزة
(9, 3, N'المهندسين', GETDATE(), 0),
(10, 3, N'الدقي', GETDATE(), 0),
(11, 3, N'6 أكتوبر', GETDATE(), 0),
(12, 3, N'الشيخ زايد', GETDATE(), 0);

SET IDENTITY_INSERT Districts OFF;

-- ================================================================
-- 4. أنواع العقارات (PropertyTypes)
-- ================================================================
SET IDENTITY_INSERT PropertyTypes ON;

INSERT INTO PropertyTypes (Id, Name, IconUrl, CreatedAt, IsDeleted)
VALUES
(1, N'شقة', N'/icons/apartment.svg', GETDATE(), 0),
(2, N'فيلا', N'/icons/villa.svg', GETDATE(), 0),
(3, N'دوبلكس', N'/icons/duplex.svg', GETDATE(), 0),
(4, N'محل تجاري', N'/icons/shop.svg', GETDATE(), 0),
(5, N'مكتب', N'/icons/office.svg', GETDATE(), 0),
(6, N'أرض', N'/icons/land.svg', GETDATE(), 0),
(7, N'بنتهاوس', N'/icons/penthouse.svg', GETDATE(), 0);

SET IDENTITY_INSERT PropertyTypes OFF;

-- ================================================================
-- 5. المرافق (Amenities)
-- ================================================================
SET IDENTITY_INSERT Amenities ON;

INSERT INTO Amenities (Id, Name, IconUrl, CreatedAt, IsDeleted)
VALUES
(1, N'مصعد', N'/icons/elevator.svg', GETDATE(), 0),
(2, N'جراج', N'/icons/garage.svg', GETDATE(), 0),
(3, N'حمام سباحة', N'/icons/pool.svg', GETDATE(), 0),
(4, N'حديقة', N'/icons/garden.svg', GETDATE(), 0),
(5, N'أمن وحراسة', N'/icons/security.svg', GETDATE(), 0),
(6, N'تكييف مركزي', N'/icons/ac.svg', GETDATE(), 0),
(7, N'إنترنت', N'/icons/wifi.svg', GETDATE(), 0),
(8, N'غرفة بواب', N'/icons/doorman.svg', GETDATE(), 0),
(9, N'جيم', N'/icons/gym.svg', GETDATE(), 0),
(10, N'ملاعب', N'/icons/playground.svg', GETDATE(), 0);

SET IDENTITY_INSERT Amenities OFF;

-- ================================================================
-- 6. الباقات (Packages)
-- ================================================================
SET IDENTITY_INSERT Packages ON;

INSERT INTO Packages (Id, Name, Price, DurationDays, MaxProperties, MaxFeatured, CanBumpUp, CreatedAt, IsDeleted)
VALUES
(1, N'الباقة المجانية', 0, 30, 2, 0, 0, GETDATE(), 0),
(2, N'الباقة الأساسية', 299, 30, 10, 2, 1, GETDATE(), 0),
(3, N'الباقة المميزة', 599, 30, 25, 5, 1, GETDATE(), 0),
(4, N'الباقة الذهبية', 999, 30, 50, 10, 1, GETDATE(), 0);

SET IDENTITY_INSERT Packages OFF;

-- ================================================================
-- 7. المشروعات (Projects)
-- ================================================================
SET IDENTITY_INSERT Projects ON;

INSERT INTO Projects (Id, Name, LogoUrl, CoverImageUrl, CityId, DistrictId, LocationDescription, IsActive, CreatedAt, IsDeleted)
VALUES
(1, N'كمبوند الرحاب', N'/projects/rehab-logo.jpg', N'/projects/rehab-cover.jpg', 
 1, 5, N'مدينة الرحاب - القاهرة الجديدة', 1, GETDATE(), 0),
 
(2, N'كمبوند بيفرلي هيلز', N'/projects/beverly-logo.jpg', N'/projects/beverly-cover.jpg',
 3, 12, N'الشيخ زايد - الجيزة', 1, GETDATE(), 0),
 
(3, N'مشروع العاصمة الإدارية', N'/projects/capital-logo.jpg', N'/projects/capital-cover.jpg',
 1, NULL, N'العاصمة الإدارية الجديدة', 1, GETDATE(), 0);

SET IDENTITY_INSERT Projects OFF;

-- ================================================================
-- 8. العقارات (Properties)
-- ================================================================
SET IDENTITY_INSERT Properties ON;

INSERT INTO Properties (Id, Title, Description, PropertyTypeId, CityId, DistrictId, ProjectId,
                       Price, RentPriceMonthly, Area, Rooms, Bathrooms, FloorNumber, FinishingType,
                       Purpose, Latitude, Longitude, AddressDetails, IsAgricultural, UserId,
                       ViewCount, WhatsAppClicks, PhoneClicks, IsFeatured, FeaturedUntil, Status,
                       CreatedAt, IsDeleted)
VALUES
-- شقة للبيع في مصر الجديدة
(1, N'شقة فاخرة 200 متر في مصر الجديدة', 
 N'شقة للبيع في موقع متميز بمصر الجديدة، 3 غرف نوم، 2 حمام، ريسبشن واسع، مطبخ، بلكونة. الشقة بحالة ممتازة وتشطيب سوبر لوكس. قريبة من جميع الخدمات والمواصلات.',
 1, 1, 1, NULL, 3500000, NULL, 200, 3, 2, 5, 2, 1,
 30.0922, 31.3244, N'شارع الحجاز - مصر الجديدة', 0, '22222222-2222-2222-2222-222222222222',
 45, 12, 8, 1, DATEADD(DAY, 15, GETDATE()), 1, GETDATE(), 0),

-- فيلا للبيع في التجمع
(2, N'فيلا مستقلة 400 متر في التجمع الخامس',
 N'فيلا فاخرة للبيع في التجمع الخامس، مكونة من 5 غرف نوم ماستر، 4 حمامات، ريسبشن 3 قطع، غرفة طعام، مطبخ واسع، حديقة خاصة 200 متر، جراج لسيارتين. التشطيب سوبر لوكس بأفضل الخامات.',
 2, 1, 4, NULL, 8500000, NULL, 400, 5, 4, NULL, 2, 1,
 30.0131, 31.4306, N'الحي الأول - التجمع الخامس', 0, '22222222-2222-2222-2222-222222222222',
 89, 25, 18, 1, DATEADD(DAY, 20, GETDATE()), 1, GETDATE(), 0),

-- شقة للإيجار في المعادي
(3, N'شقة مفروشة للإيجار في المعادي',
 N'شقة للإيجار مفروشة بالكامل في المعادي الجديدة، 2 غرفة نوم، حمام، ريسبشن، مطبخ مجهز. الشقة في عمارة حديثة بها أمن وحراسة، قريبة من المترو.',
 1, 1, 2, NULL, 0, 8000, 120, 2, 1, 3, 2, 2,
 29.9606, 31.2497, N'المعادي الجديدة', 0, '33333333-3333-3333-3333-333333333333',
 34, 8, 6, 0, NULL, 1, GETDATE(), 0),

-- شقة في كمبوند الرحاب
(4, N'شقة 180 متر في كمبوند الرحاب',
 N'شقة للبيع في الرحاب، 3 غرف، 2 حمام، ريسبشن، مطبخ، بلكونة. العقار داخل كمبوند به جميع الخدمات من حمام سباحة، نادي، ملاعب، وأمن.',
 1, 1, 5, 1, 2800000, NULL, 180, 3, 2, 2, 2, 1,
 30.0588, 31.4994, N'الرحاب - القاهرة الجديدة', 0, '44444444-4444-4444-4444-444444444444',
 56, 15, 10, 1, DATEADD(DAY, 10, GETDATE()), 1, GETDATE(), 0),

-- دوبلكس في الشيخ زايد
(5, N'دوبلكس للبيع في كمبوند بيفرلي هيلز',
 N'دوبلكس مميز للبيع في بيفرلي هيلز - الشيخ زايد، مساحة 300 متر، 4 غرف نوم، 3 حمامات، 2 ريسبشن، حديقة خاصة. تشطيب فاخر وموقع مميز.',
 3, 3, 12, 2, 6500000, NULL, 300, 4, 3, NULL, 2, 1,
 30.0178, 30.9756, N'الشيخ زايد - بيفرلي هيلز', 0, '22222222-2222-2222-2222-222222222222',
 78, 20, 14, 1, DATEADD(DAY, 25, GETDATE()), 1, GETDATE(), 0),

-- محل تجاري للإيجار
(6, N'محل تجاري للإيجار على شارع رئيسي',
 N'محل تجاري مميز للإيجار في موقع حيوي بمصر الجديدة، المساحة 60 متر، واجهة زجاجية، مناسب لجميع الأنشطة التجارية.',
 4, 1, 1, NULL, 0, 15000, 60, NULL, 1, NULL, 1, 2,
 30.0875, 31.3242, N'شارع العروبة - مصر الجديدة', 0, '33333333-3333-3333-3333-333333333333',
 92, 35, 22, 0, NULL, 1, GETDATE(), 0),

-- شقة في سموحة
(7, N'شقة للبيع في سموحة - الإسكندرية',
 N'شقة مميزة للبيع في سموحة، 150 متر، 3 غرف نوم، 2 حمام، ريسبشن واسع، مطبخ، بلكونة. موقع متميز قريب من البحر وجميع الخدمات.',
 1, 2, 6, NULL, 2500000, NULL, 150, 3, 2, 4, 2, 1,
 31.2370, 29.9575, N'سموحة - الإسكندرية', 0, '44444444-4444-4444-4444-444444444444',
 41, 11, 7, 0, NULL, 1, GETDATE(), 0),

-- مكتب للإيجار
(8, N'مكتب إداري للإيجار في مدينة نصر',
 N'مكتب إداري 100 متر للإيجار في برج حديث بمدينة نصر، يتكون من 3 غرف وصالة استقبال وحمامين. المكتب مكيف ومجهز بالكامل.',
 5, 1, 3, NULL, 0, 12000, 100, NULL, 2, 8, 2, 2,
 30.0626, 31.3564, N'مدينة نصر - شارع عباس العقاد', 0, '55555555-5555-5555-5555-555555555555',
 29, 9, 5, 0, NULL, 1, GETDATE(), 0),

-- بنتهاوس للبيع
(9, N'بنتهاوس فاخر للبيع في المهندسين',
 N'بنتهاوس سوبر لوكس للبيع في المهندسين، 250 متر + 100 متر terraceجة، 4 غرف نوم، 3 حمامات، ريسبشن 3 قطع، مطبخ أمريكي. إطلالة بانورامية رائعة على النيل.',
 7, 3, 9, NULL, 7500000, NULL, 250, 4, 3, 12, 2, 1,
 30.0619, 31.2088, N'المهندسين - شارع جامعة الدول العربية', 0, '22222222-2222-2222-2222-222222222222',
 67, 18, 12, 1, DATEADD(DAY, 30, GETDATE()), 1, GETDATE(), 0),

-- شقة للإيجار في 6 أكتوبر
(10, N'شقة للإيجار في 6 أكتوبر',
 N'شقة للإيجار 130 متر في 6 أكتوبر، 2 غرفة نوم، 2 حمام، ريسبشن، مطبخ. الشقة نظيفة وجاهزة للسكن في كمبوند مغلق.',
 1, 3, 11, NULL, 0, 6000, 130, 2, 2, 1, 1, 2,
 29.9537, 30.9277, N'6 أكتوبر - الحي السابع', 0, '66666666-6666-6666-6666-666666666666',
 23, 5, 3, 0, NULL, 1, GETDATE(), 0),

-- شقة قيد المراجعة
(11, N'شقة للبيع في الدقي - تحت المراجعة',
 N'شقة للبيع 160 متر في الدقي، 3 غرف، 2 حمام، ريسبشن، مطبخ، بلكونة. موقع ممتاز قريب من المترو.',
 1, 3, 10, NULL, 3200000, NULL, 160, 3, 2, 6, 2, 1,
 30.0449, 31.2126, N'الدقي - شارع التحرير', 0, '44444444-4444-4444-4444-444444444444',
 5, 1, 0, 0, NULL, 0, GETDATE(), 0);

SET IDENTITY_INSERT Properties OFF;

-- ================================================================
-- 9. صور العقارات (PropertyImages)
-- ================================================================
SET IDENTITY_INSERT PropertyImages ON;

INSERT INTO PropertyImages (Id, PropertyId, ImageUrl, IsMain, SortOrder, CreatedAt, IsDeleted)
VALUES
-- شقة 1
(1, 1, N'/properties/1/main.jpg', 1, 1, GETDATE(), 0),
(2, 1, N'/properties/1/living-room.jpg', 0, 2, GETDATE(), 0),
(3, 1, N'/properties/1/bedroom.jpg', 0, 3, GETDATE(), 0),
(4, 1, N'/properties/1/kitchen.jpg', 0, 4, GETDATE(), 0),
-- فيلا 2
(5, 2, N'/properties/2/main.jpg', 1, 1, GETDATE(), 0),
(6, 2, N'/properties/2/garden.jpg', 0, 2, GETDATE(), 0),
(7, 2, N'/properties/2/pool.jpg', 0, 3, GETDATE(), 0),
-- شقة 3
(8, 3, N'/properties/3/main.jpg', 1, 1, GETDATE(), 0),
(9, 3, N'/properties/3/bedroom.jpg', 0, 2, GETDATE(), 0),
-- شقة 4
(10, 4, N'/properties/4/main.jpg', 1, 1, GETDATE(), 0),
(11, 4, N'/properties/4/view.jpg', 0, 2, GETDATE(), 0),
-- دوبلكس 5
(12, 5, N'/properties/5/main.jpg', 1, 1, GETDATE(), 0),
(13, 5, N'/properties/5/terrace.jpg', 0, 2, GETDATE(), 0),
-- محل 6
(14, 6, N'/properties/6/main.jpg', 1, 1, GETDATE(), 0),
-- شقة 7
(15, 7, N'/properties/7/main.jpg', 1, 1, GETDATE(), 0),
-- مكتب 8
(16, 8, N'/properties/8/main.jpg', 1, 1, GETDATE(), 0),
-- بنتهاوس 9
(17, 9, N'/properties/9/main.jpg', 1, 1, GETDATE(), 0),
(18, 9, N'/properties/9/terrace.jpg', 0, 2, GETDATE(), 0),
(19, 9, N'/properties/9/nile-view.jpg', 0, 3, GETDATE(), 0),
-- شقة 10
(20, 10, N'/properties/10/main.jpg', 1, 1, GETDATE(), 0);

SET IDENTITY_INSERT PropertyImages OFF;

-- ================================================================
-- 10. مرافق العقارات (PropertyAmenities)
-- ================================================================
SET IDENTITY_INSERT PropertyAmenities ON;

INSERT INTO PropertyAmenities (Id, PropertyId, AmenityId, CreatedAt, IsDeleted)
VALUES
-- شقة 1
(1, 1, 1, GETDATE(), 0),  -- مصعد
(2, 1, 2, GETDATE(), 0),  -- جراج
(3, 1, 5, GETDATE(), 0),  -- أمن
-- فيلا 2
(4, 2, 2, GETDATE(), 0),  -- جراج
(5, 2, 3, GETDATE(), 0),  -- حمام سباحة
(6, 2, 4, GETDATE(), 0),  -- حديقة
(7, 2, 5, GETDATE(), 0),  -- أمن
(8, 2, 9, GETDATE(), 0),  -- جيم
-- شقة 3
(9, 3, 1, GETDATE(), 0),  -- مصعد
(10, 3, 5, GETDATE(), 0), -- أمن
(11, 3, 6, GETDATE(), 0), -- تكييف
-- شقة 4 (الرحاب)
(12, 4, 1, GETDATE(), 0), -- مصعد
(13, 4, 3, GETDATE(), 0), -- حمام سباحة
(14, 4, 5, GETDATE(), 0), -- أمن
(15, 4, 10, GETDATE(), 0), -- ملاعب
-- دوبلكس 5
(16, 5, 2, GETDATE(), 0), -- جراج
(17, 5, 4, GETDATE(), 0), -- حديقة
(18, 5, 5, GETDATE(), 0), -- أمن
(19, 5, 9, GETDATE(), 0), -- جيم
-- بنتهاوس 9
(20, 9, 1, GETDATE(), 0), -- مصعد
(21, 9, 2, GETDATE(), 0), -- جراج
(22, 9, 6, GETDATE(), 0), -- تكييف
(23, 9, 8, GETDATE(), 0); -- بواب

SET IDENTITY_INSERT PropertyAmenities OFF;

-- ================================================================
-- 11. الاشتراكات (UserSubscriptions)
-- ================================================================
SET IDENTITY_INSERT UserSubscriptions ON;

INSERT INTO UserSubscriptions (Id, UserId, PackageId, StartDate, EndDate, IsActive, PaymentMethod, AmountPaid, CreatedAt, IsDeleted)
VALUES
(1, '22222222-2222-2222-2222-222222222222', 3, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, N'فيزا', 599, GETDATE(), 0),
(2, '33333333-3333-3333-3333-333333333333', 2, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, N'فودافون كاش', 299, GETDATE(), 0),
(3, '44444444-4444-4444-4444-444444444444', 4, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, N'فيزا', 999, GETDATE(), 0);

SET IDENTITY_INSERT UserSubscriptions OFF;

-- ================================================================
-- 12. العقارات المحفوظة (SavedProperties)
-- ================================================================
SET IDENTITY_INSERT SavedProperties ON;

INSERT INTO SavedProperties (Id, UserId, PropertyId, SavedAt, CreatedAt, IsDeleted)
VALUES
(1, '55555555-5555-5555-5555-555555555555', 1, GETDATE(), GETDATE(), 0),
(2, '55555555-5555-5555-5555-555555555555', 2, GETDATE(), GETDATE(), 0),
(3, '66666666-6666-6666-6666-666666666666', 4, GETDATE(), GETDATE(), 0),
(4, '66666666-6666-6666-6666-666666666666', 5, GETDATE(), GETDATE(), 0),
(5, '33333333-3333-3333-3333-333333333333', 9, GETDATE(), GETDATE(), 0);

SET IDENTITY_INSERT SavedProperties OFF;

-- ================================================================
-- 13. مراجعات العقارات (PropertyReviews)
-- ================================================================
SET IDENTITY_INSERT PropertyReviews ON;

INSERT INTO PropertyReviews (Id, PropertyId, AdminId, Action, Message, CreatedAt, IsDeleted)
VALUES
(1, 1, '11111111-1111-1111-1111-111111111111', 1, N'تم الموافقة على العقار ونشره', GETDATE(), 0),
(2, 2, '11111111-1111-1111-1111-111111111111', 1, N'عقار مميز - تم الموافقة', GETDATE(), 0),
(3, 11, '11111111-1111-1111-1111-111111111111', 0, N'العقار قيد المراجعة - يرجى إضافة المزيد من الصور', GETDATE(), 0);

SET IDENTITY_INSERT PropertyReviews OFF;

-- ================================================================
-- 14. الإشعارات (Notifications)
-- ================================================================
SET IDENTITY_INSERT Notifications ON;

INSERT INTO Notifications (Id, UserId, Title, Message, Type, IsRead, PropertyId, CreatedAt, IsDeleted)
VALUES
(1, '22222222-2222-2222-2222-222222222222', N'تم نشر عقارك', 
 N'تم مراجعة والموافقة على عقار "شقة فاخرة 200 متر في مصر الجديدة" ونشره على الموقع', 
 1, 0, 1, GETDATE(), 0),
 
(2, '22222222-2222-2222-2222-222222222222', N'عقارك ضمن المميز!', 
 N'عقارك "فيلا مستقلة 400 متر في التجمع الخامس" تم إضافته ضمن العقارات المميزة', 
 2, 0, 2, GETDATE(), 0),
 
(3, '33333333-3333-3333-3333-333333333333', N'استفسار عن عقارك', 
 N'هناك مستخدم قام بالضغط على رقم الواتساب الخاص بعقارك في المعادي', 
 3, 1, 3, DATEADD(DAY, -1, GETDATE()), 0),
 
(4, '44444444-4444-4444-4444-444444444444', N'اشتراكك على وشك الانتهاء', 
 N'اشتراكك في الباقة الذهبية سينتهي خلال 5 أيام. جدد الآن للاستمرار في الاستفادة من المزايا', 
 4, 0, NULL, GETDATE(), 0),

(5, '44444444-4444-4444-4444-444444444444', N'عقارك قيد المراجعة',
 N'تم استلام عقار "شقة للبيع في الدقي" وجاري المراجعة من قبل الإدارة',
 1, 1, 11, DATEADD(HOUR, -2, GETDATE()), 0);

SET IDENTITY_INSERT Notifications OFF;

-- ================================================================
-- انتهى السكريبت
-- ================================================================

PRINT N'تم إدراج البيانات التجريبية بنجاح! ✓';
PRINT N'';
PRINT N'الإحصائيات:';
PRINT N'- المستخدمين: 6 (1 أدمن، 3 ملاك، 2 وكلاء)';
PRINT N'- المدن: 5';
PRINT N'- الأحياء: 12';
PRINT N'- أنواع العقارات: 7';
PRINT N'- المرافق: 10';
PRINT N'- الباقات: 4';
PRINT N'- المشروعات: 3';
PRINT N'- العقارات: 11';
PRINT N'- الصور: 20';
PRINT N'- الإشعارات: 5';
PRINT N'';
PRINT N'يمكنك الآن تصفح الموقع وعرض البيانات العربية!';
GO
