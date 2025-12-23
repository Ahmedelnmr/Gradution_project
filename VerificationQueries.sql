-- ======================================
-- استعلامات مفيدة للتحقق من بيانات Seeding
-- Homy Database Verification Queries
-- ======================================

USE [HomyDB]; -- غير اسم قاعدة البيانات حسب الحاجة
GO

-- ======================================
-- 1. إحصائيات عامة
-- ======================================
PRINT '========== إحصائيات عامة ==========';

SELECT 'إجمالي المستخدمين' AS Item, COUNT(*) AS Count FROM AspNetUsers WHERE IsDeleted = 0
UNION ALL
SELECT 'إجمالي العقارات', COUNT(*) FROM Properties WHERE IsDeleted = 0
UNION ALL
SELECT 'العقارات المفعلة', COUNT(*) FROM Properties WHERE Status = 1 AND IsDeleted = 0
UNION ALL
SELECT 'العقارات المميزة', COUNT(*) FROM Properties WHERE IsFeatured = 1 AND IsDeleted = 0
UNION ALL
SELECT 'إجمالي المدن', COUNT(*) FROM Cities WHERE IsDeleted = 0
UNION ALL
SELECT 'إجمالي الأحياء', COUNT(*) FROM Districts WHERE IsDeleted = 0
UNION ALL
SELECT 'إجمالي الباقات', COUNT(*) FROM Packages WHERE IsDeleted = 0
UNION ALL
SELECT 'الاشتراكات النشطة', COUNT(*) FROM UserSubscriptions WHERE IsActive = 1;

-- ======================================
-- 2. عرض المستخدمين مع أدوارهم
-- ======================================
PRINT '';
PRINT '========== المستخدمين ==========';

SELECT 
    u.FullName AS [الاسم الكامل],
    u.Email AS [البريد الإلكتروني],
    u.PhoneNumber AS [رقم الهاتف],
    r.Name AS [الدور],
    u.IsVerified AS [موثق],
    u.IsActive AS [نشط],
    u.CreatedAt AS [تاريخ التسجيل]
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.IsDeleted = 0
ORDER BY r.Name, u.FullName;

-- ======================================
-- 3. عرض العقارات بالتفاصيل
-- ======================================
PRINT '';
PRINT '========== العقارات ==========';

SELECT 
    p.Id AS [رقم العقار],
    p.Title AS [العنوان],
    pt.Name AS [النوع],
    c.Name AS [المدينة],
    d.Name AS [الحي],
    CASE p.Purpose 
        WHEN 0 THEN N'للبيع'
        WHEN 1 THEN N'للإيجار'
    END AS [الغرض],
    FORMAT(p.Price, 'N2') + N' جنيه' AS [السعر],
    CASE 
        WHEN p.RentPriceMonthly IS NOT NULL 
        THEN FORMAT(p.RentPriceMonthly, 'N2') + N' جنيه/شهر'
        ELSE N'-'
    END AS [الإيجار الشهري],
    p.Area AS [المساحة (متر)],
    p.Rooms AS [الغرف],
    p.Bathrooms AS [الحمامات],
    CASE p.Status 
        WHEN 0 THEN N'قيد المراجعة'
        WHEN 1 THEN N'نشط'
        WHEN 2 THEN N'مرفوض'
        WHEN 3 THEN N'منتهي'
    END AS [الحالة],
    p.IsFeatured AS [مميز],
    p.ViewCount AS [المشاهدات],
    u.FullName AS [المالك]
FROM Properties p
JOIN Cities c ON p.CityId = c.Id
LEFT JOIN Districts d ON p.DistrictId = d.Id
JOIN PropertyTypes pt ON p.PropertyTypeId = pt.Id
JOIN AspNetUsers u ON p.UserId = u.Id
WHERE p.IsDeleted = 0
ORDER BY p.CreatedAt DESC;

-- ======================================
-- 4. عرض العقارات مع عدد الصور والميزات
-- ======================================
PRINT '';
PRINT '========== العقارات مع الصور والميزات ==========';

SELECT 
    p.Title AS [العنوان],
    c.Name AS [المدينة],
    COUNT(DISTINCT pi.Id) AS [عدد الصور],
    COUNT(DISTINCT pa.Id) AS [عدد الميزات],
    p.ViewCount AS [المشاهدات],
    p.PhoneClicks AS [نقرات الهاتف],
    p.WhatsAppClicks AS [نقرات واتساب]
FROM Properties p
JOIN Cities c ON p.CityId = c.Id
LEFT JOIN PropertyImages pi ON p.Id = pi.PropertyId AND pi.IsDeleted = 0
LEFT JOIN PropertyAmenities pa ON p.Id = pa.PropertyId AND pa.IsDeleted = 0
WHERE p.IsDeleted = 0
GROUP BY p.Title, c.Name, p.ViewCount, p.PhoneClicks, p.WhatsAppClicks
ORDER BY p.ViewCount DESC;

-- ======================================
-- 5. عرض الميزات لكل عقار
-- ======================================
PRINT '';
PRINT '========== ميزات العقارات ==========';

SELECT 
    p.Title AS [العقار],
    STRING_AGG(a.Name, N'، ') AS [الميزات]
FROM Properties p
JOIN PropertyAmenities pa ON p.Id = pa.PropertyId
JOIN Amenities a ON pa.AmenityId = a.Id
WHERE p.IsDeleted = 0 AND pa.IsDeleted = 0
GROUP BY p.Id, p.Title
ORDER BY p.Title;

-- ======================================
-- 6. عرض المدن مع عدد العقارات
-- ======================================
PRINT '';
PRINT '========== المدن وعدد العقارات ==========';

SELECT 
    c.Name AS [المدينة],
    c.NameEn AS [City Name],
    COUNT(DISTINCT d.Id) AS [عدد الأحياء],
    COUNT(DISTINCT p.Id) AS [عدد العقارات],
    COUNT(DISTINCT CASE WHEN p.Purpose = 0 THEN p.Id END) AS [للبيع],
    COUNT(DISTINCT CASE WHEN p.Purpose = 1 THEN p.Id END) AS [للإيجار]
FROM Cities c
LEFT JOIN Districts d ON c.Id = d.CityId AND d.IsDeleted = 0
LEFT JOIN Properties p ON c.Id = p.CityId AND p.IsDeleted = 0
WHERE c.IsDeleted = 0
GROUP BY c.Name, c.NameEn
ORDER BY COUNT(DISTINCT p.Id) DESC;

-- ======================================
-- 7. عرض الباقات مع عدد المشتركين
-- ======================================
PRINT '';
PRINT '========== الباقات ==========';

SELECT 
    pkg.Name AS [اسم الباقة],
    FORMAT(pkg.Price, 'N2') + N' جنيه' AS [السعر],
    pkg.DurationDays AS [المدة (أيام)],
    pkg.MaxProperties AS [أقصى عدد عقارات],
    pkg.MaxFeatured AS [أقصى عقارات مميزة],
    CASE pkg.CanBumpUp 
        WHEN 1 THEN N'نعم'
        ELSE N'لا'
    END AS [يمكن الترقية],
    COUNT(us.Id) AS [عدد المشتركين]
FROM Packages pkg
LEFT JOIN UserSubscriptions us ON pkg.Id = us.PackageId AND us.IsActive = 1
WHERE pkg.IsDeleted = 0
GROUP BY pkg.Name, pkg.Price, pkg.DurationDays, pkg.MaxProperties, pkg.MaxFeatured, pkg.CanBumpUp
ORDER BY pkg.Price;

-- ======================================
-- 8. عرض الاشتراكات النشطة
-- ======================================
PRINT '';
PRINT '========== الاشتراكات النشطة ==========';

SELECT 
    u.FullName AS [المستخدم],
    pkg.Name AS [الباقة],
    FORMAT(us.AmountPaid, 'N2') + N' جنيه' AS [المبلغ المدفوع],
    us.PaymentMethod AS [طريقة الدفع],
    us.StartDate AS [تاريخ البداية],
    us.EndDate AS [تاريخ الانتهاء],
    DATEDIFF(DAY, GETDATE(), us.EndDate) AS [الأيام المتبقية]
FROM UserSubscriptions us
JOIN AspNetUsers u ON us.UserId = u.Id
JOIN Packages pkg ON us.PackageId = pkg.Id
WHERE us.IsActive = 1 AND us.IsDeleted = 0
ORDER BY us.EndDate;

-- ======================================
-- 9. عرض المشاريع/الكمبوندات
-- ======================================
PRINT '';
PRINT '========== المشاريع والكمبوندات ==========';

SELECT 
    proj.Name AS [اسم المشروع],
    c.Name AS [المدينة],
    d.Name AS [الحي],
    proj.LocationDescription AS [وصف الموقع],
    COUNT(p.Id) AS [عدد العقارات],
    CASE proj.IsActive 
        WHEN 1 THEN N'نشط'
        ELSE N'غير نشط'
    END AS [الحالة]
FROM Projects proj
JOIN Cities c ON proj.CityId = c.Id
LEFT JOIN Districts d ON proj.DistrictId = d.Id
LEFT JOIN Properties p ON proj.Id = p.ProjectId AND p.IsDeleted = 0
WHERE proj.IsDeleted = 0
GROUP BY proj.Name, c.Name, d.Name, proj.LocationDescription, proj.IsActive
ORDER BY proj.Name;

-- ======================================
-- 10. عرض العقارات المحفوظة
-- ======================================
PRINT '';
PRINT '========== العقارات المحفوظة ==========';

SELECT 
    u.FullName AS [المستخدم],
    p.Title AS [العقار],
    FORMAT(p.Price, 'N2') + N' جنيه' AS [السعر],
    sp.SavedAt AS [تاريخ الحفظ]
FROM SavedProperties sp
JOIN AspNetUsers u ON sp.UserId = u.Id
JOIN Properties p ON sp.PropertyId = p.Id
WHERE sp.IsDeleted = 0 AND p.IsDeleted = 0
ORDER BY sp.SavedAt DESC;

-- ======================================
-- 11. عرض الإشعارات
-- ======================================
PRINT '';
PRINT '========== الإشعارات ==========';

SELECT 
    u.FullName AS [المستخدم],
    n.Title AS [العنوان],
    n.Message AS [الرسالة],
    CASE n.Type 
        WHEN 0 THEN N'عام'
        WHEN 1 THEN N'عقار'
        WHEN 2 THEN N'نظام'
    END AS [النوع],
    CASE n.IsRead 
        WHEN 1 THEN N'مقروء'
        ELSE N'غير مقروء'
    END AS [الحالة],
    n.CreatedAt AS [التاريخ]
FROM Notifications n
JOIN AspNetUsers u ON n.UserId = u.Id
WHERE n.IsDeleted = 0
ORDER BY n.CreatedAt DESC;

-- ======================================
-- 12. تحليل الأداء - أكثر العقارات مشاهدة
-- ======================================
PRINT '';
PRINT '========== أكثر العقارات مشاهدة ==========';

SELECT TOP 10
    p.Title AS [العنوان],
    c.Name AS [المدينة],
    pt.Name AS [النوع],
    p.ViewCount AS [المشاهدات],
    p.PhoneClicks AS [نقرات الهاتف],
    p.WhatsAppClicks AS [نقرات واتساب],
    u.FullName AS [المالك]
FROM Properties p
JOIN Cities c ON p.CityId = c.Id
JOIN PropertyTypes pt ON p.PropertyTypeId = pt.Id
JOIN AspNetUsers u ON p.UserId = u.Id
WHERE p.IsDeleted = 0
ORDER BY p.ViewCount DESC;

-- ======================================
-- 13. تحليل العقارات حسب السعر
-- ======================================
PRINT '';
PRINT '========== تحليل الأسعار ==========';

SELECT 
    CASE p.Purpose 
        WHEN 0 THEN N'للبيع'
        WHEN 1 THEN N'للإيجار'
    END AS [الغرض],
    pt.Name AS [النوع],
    COUNT(*) AS [العدد],
    FORMAT(MIN(p.Price), 'N2') AS [أقل سعر],
    FORMAT(MAX(p.Price), 'N2') AS [أعلى سعر],
    FORMAT(AVG(p.Price), 'N2') AS [متوسط السعر]
FROM Properties p
JOIN PropertyTypes pt ON p.PropertyTypeId = pt.Id
WHERE p.IsDeleted = 0
GROUP BY p.Purpose, pt.Name
ORDER BY p.Purpose, COUNT(*) DESC;

-- ======================================
-- 14. أكثر الوسطاء نشاطاً
-- ======================================
PRINT '';
PRINT '========== أكثر الوسطاء نشاطاً ==========';

SELECT 
    u.FullName AS [الوسيط],
    u.Email AS [البريد الإلكتروني],
    COUNT(p.Id) AS [عدد العقارات],
    SUM(p.ViewCount) AS [إجمالي المشاهدات],
    SUM(p.PhoneClicks) AS [إجمالي نقرات الهاتف],
    pkg.Name AS [الباقة الحالية]
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN Properties p ON u.Id = p.UserId AND p.IsDeleted = 0
LEFT JOIN UserSubscriptions us ON u.Id = us.UserId AND us.IsActive = 1
LEFT JOIN Packages pkg ON us.PackageId = pkg.Id
WHERE r.Name = 'Broker' AND u.IsDeleted = 0
GROUP BY u.FullName, u.Email, pkg.Name
ORDER BY COUNT(p.Id) DESC;

-- ======================================
-- 15. التحقق من سلامة البيانات
-- ======================================
PRINT '';
PRINT '========== التحقق من سلامة البيانات ==========';

-- عقارات بدون صور
SELECT 'عقارات بدون صور' AS [المشكلة], COUNT(*) AS [العدد]
FROM Properties p
LEFT JOIN PropertyImages pi ON p.Id = pi.PropertyId AND pi.IsDeleted = 0
WHERE p.IsDeleted = 0 AND pi.Id IS NULL

UNION ALL

-- عقارات بدون ميزات
SELECT 'عقارات بدون ميزات', COUNT(*)
FROM Properties p
LEFT JOIN PropertyAmenities pa ON p.Id = pa.PropertyId AND pa.IsDeleted = 0
WHERE p.IsDeleted = 0 AND pa.Id IS NULL

UNION ALL

-- مستخدمين بدون أدوار
SELECT 'مستخدمين بدون أدوار', COUNT(*)
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
WHERE u.IsDeleted = 0 AND ur.RoleId IS NULL

UNION ALL

-- أحياء بدون عقارات
SELECT 'أحياء بدون عقارات', COUNT(*)
FROM Districts d
LEFT JOIN Properties p ON d.Id = p.DistrictId AND p.IsDeleted = 0
WHERE d.IsDeleted = 0 AND p.Id IS NULL;

-- ======================================
-- النهاية
-- ======================================
PRINT '';
PRINT 'تم الانتهاء من جميع الاستعلامات!';
GO
