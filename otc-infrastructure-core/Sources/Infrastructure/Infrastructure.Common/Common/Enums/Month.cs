using System.Globalization;

namespace Infrastructure.Common.Enums
{
    /// <summary>
    /// Месяцы
    /// </summary>
    public enum Month
    {
        Unknown = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12,
    }

    public static class MonthExtension
    {
        public static string AsName(this Month month)
        {
            return AsName(month, CultureInfo.CurrentCulture);
        }
        public static string AsName(this Month month, CultureInfo cultureInfo)
        {
            var names = cultureInfo.DateTimeFormat.MonthNames;
            var index = (int) month - 1;
            if (index < 0)
            {
                index = names.Length - 1;
            }
            return cultureInfo.DateTimeFormat.MonthNames[index];
        }
    }
}