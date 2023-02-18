using System;

namespace Infrastructure.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly int MoscowTimeZone = 3;
        public static readonly DateTime MinSqlSmallDateValue = new DateTime(1900, 01, 01, 00, 00, 00);
        public static readonly DateTime MaxSqlSmallDateValue = new DateTime(2079, 06, 06, 23, 59, 00);

        public static readonly string TimeZoneMskText = " (время московское)";
        public static readonly string TimeZoneAstanaText = " (по времени Астаны)";

        public static readonly string DefaultDateTimeFormat = "dd.MM.yyyy HH:mm:ss";       

        public static bool IsCorrectSqlSmallDate(this DateTime dateTime)
        {
            return dateTime > MinSqlSmallDateValue && dateTime < MaxSqlSmallDateValue;
        }

        public static bool IsCorrectSqlSmallDate(this DateTime? dateTime)
        {
            if (dateTime == null)
                return true;

            return IsCorrectSqlSmallDate(dateTime.Value);
        }

        public static DateTime MonthBegin(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static string ToMoscowTime(this DateTime date, bool withPrefix = false, string dateTimeFormat = null)
        {
            var dateTime = ToMoscowDateTime(date);

            // Отображаем время по Москве
            return dateTime.ToString(dateTimeFormat != null ? dateTimeFormat : DefaultDateTimeFormat) + (withPrefix ? TimeZoneMskText : string.Empty);
        }

        public static string ToMoscowDateTime(this DateTime date, bool withPrefix = false)
        {
            var dateTime = ToMoscowDateTime(date);

            // Отображаем время по Москве
            return dateTime.ToString("dd.MM.yyyy HH:mm") + (withPrefix ? TimeZoneMskText : string.Empty);
        }

        public static string ToAstanaTime(this DateTime date, bool withPrefix = false)
        {
            TimeZoneInfo atanaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");

            // Преобразуем врея из UTC в Астана
            DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(date, atanaTimeZone);

            // Отображаем время по Москве
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss") + (withPrefix ? TimeZoneAstanaText : string.Empty);
        }

        public static string ToAstanaDateTime(this DateTime date, bool withPrefix = false)
        {
            TimeZoneInfo atanaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");

            // Преобразуем врея из UTC в Астана
            DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(date, atanaTimeZone);

            // Отображаем время по Астана
            return dateTime.ToString("dd.MM.yyyy") + (withPrefix ? TimeZoneAstanaText : string.Empty);
        }

        public static DateTime SetUtcKind(this DateTime date)
        {
            return new DateTime(date.Ticks, DateTimeKind.Utc);
        }

        public static DateTime? SetUtcKind(this DateTime? date)
        {
            return date.HasValue
                ? (DateTime?)date.Value.SetUtcKind()
                : (DateTime?)null;
        }

        public static DateTime? ToMoscowDateTime(this DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            };

            return ToMoscowDateTime(date.Value);
        }

        public static DateTime ToMoscowDateTime(this DateTime date)
        {
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

            // Преобразуем врея из UTC в московское
            return TimeZoneInfo.ConvertTimeFromUtc(date, moscowTimeZone);
        }

        public static DateTime? ToUtcDateTime(this string dateTimeStr)
        {
            if (string.IsNullOrEmpty(dateTimeStr))
                return null;

            var dateTime = Convert.ToDateTime(dateTimeStr);

            return SetUtcKind(dateTime); 
        }
    }
}
