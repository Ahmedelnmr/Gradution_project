using Homy.Domin.models;
using System.Globalization;

namespace Homy.Application.Extensions
{
    public static class LocalizationExtensions
    {
        public static string GetLocalizedName(this City city)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return culture == "ar" ? city.Name : (city.NameEn ?? city.Name);
        }

        public static string GetLocalizedName(this District district)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return culture == "ar" ? district.Name : (district.NameEn ?? district.Name);
        }

        public static string GetLocalizedName(this PropertyType propertyType)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return culture == "ar" ? propertyType.Name : (propertyType.NameEn ?? propertyType.Name);
        }

        public static string GetLocalizedName(this Amenity amenity)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return culture == "ar" ? amenity.Name : (amenity.NameEn ?? amenity.Name);
        }
    }
}
