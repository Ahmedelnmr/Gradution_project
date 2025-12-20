-- ================================================================
-- Update English Names for Localization
-- ================================================================
USE HomeyProject;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
GO

-- ================================================================
-- 1. Update Cities with English Names
-- ================================================================
UPDATE Cities SET NameEn = 'Cairo' WHERE Name = N'القاهرة';
UPDATE Cities SET NameEn = 'Alexandria' WHERE Name = N'الإسكندر

ية';
UPDATE Cities SET NameEn = 'Giza' WHERE Name = N'الجيزة';
UPDATE Cities SET NameEn = 'Sharqia' WHERE Name = N'الشرقية';
UPDATE Cities SET NameEn = 'Minya' WHERE Name = N'المنيا';

-- ================================================================
-- 2. Update Districts with English Names
-- ================================================================
-- Cairo Districts
UPDATE Districts SET NameEn = 'Heliopolis' WHERE Name = N'مصر الجديدة';
UPDATE Districts SET NameEn = 'Maadi' WHERE Name = N'المعادي';
UPDATE Districts SET NameEn = 'Nasr City' WHERE Name = N'مدينة نصر';
UPDATE Districts SET NameEn = 'Fifth Settlement' WHERE Name = N'التجمع الخامس';
UPDATE Districts SET NameEn = 'Rehab' WHERE Name = N'الرحاب';

-- Alexandria Districts
UPDATE Districts SET NameEn = 'Smouha' WHERE Name = N'سموحة';
UPDATE Districts SET NameEn = 'Mandara' WHERE Name = N'المندرة';
UPDATE Districts SET NameEn = 'Sidi Gaber' WHERE Name = N'سيدي جابر';

-- Giza Districts
UPDATE Districts SET NameEn = 'Mohandessin' WHERE Name = N'المهندسين';
UPDATE Districts SET NameEn = 'Dokki' WHERE Name = N'الدقي';
UPDATE Districts SET NameEn = '6th of October' WHERE Name = N'6 أكتوبر';
UPDATE Districts SET NameEn = 'Sheikh Zayed' WHERE Name = N'الشيخ زايد';

-- ================================================================
-- 3. Update PropertyTypes with English Names
-- ================================================================
UPDATE PropertyTypes SET NameEn = 'Apartment' WHERE Name = N'شقة';
UPDATE PropertyTypes SET NameEn = 'Villa' WHERE Name = N'فيلا';
UPDATE PropertyTypes SET NameEn = 'Duplex' WHERE Name = N'دوبلكس';
UPDATE PropertyTypes SET NameEn = 'Shop' WHERE Name = N'محل تجاري';
UPDATE PropertyTypes SET NameEn = 'Office' WHERE Name = N'مكتب';
UPDATE PropertyTypes SET NameEn = 'Land' WHERE Name = N'أرض';
UPDATE PropertyTypes SET NameEn = 'Penthouse' WHERE Name = N'بنتهاوس';

-- ================================================================
-- 4. Update Amenities with English Names
-- ================================================================
UPDATE Amenities SET NameEn = 'Elevator' WHERE Name = N'مصعد';
UPDATE Amenities SET NameEn = 'Garage' WHERE Name = N'جراج';
UPDATE Amenities SET NameEn = 'Swimming Pool' WHERE Name = N'حمام سباحة';
UPDATE Amenities SET NameEn = 'Garden' WHERE Name = N'حديقة';
UPDATE Amenities SET NameEn = 'Security' WHERE Name = N'أمن وحراسة';
UPDATE Amenities SET NameEn = 'Central A/C' WHERE Name = N'تكييف مركزي';
UPDATE Amenities SET NameEn = 'Internet' WHERE Name = N'إنترنت';
UPDATE Amenities SET NameEn = 'Doorman' WHERE Name = N'غرفة بواب';
UPDATE Amenities SET NameEn = 'Gym' WHERE Name = N'جيم';
UPDATE Amenities SET NameEn = 'Playgrounds' WHERE Name = N'ملاعب';

-- ================================================================
-- Verify Updates
-- ================================================================
PRINT N'تم تحديث الترجمة الإنجليزية بنجاح! ✓';
GO
