using System;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Infrastructure.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Регулярное выражение соответствующее email адресу.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Примеры поддерживаемых форматов:
        ///     user@domain.com
        ///     user@sub.domain.com
        ///     first.last@domain.com
        ///     first.last+filter@domain.com
        ///     user@домен.рф
        ///     
        /// 
        /// В рамках задачи OTC-8361 было измененно регулярное выражение для поддержания следующих форматов:
        /// user.@domain.com
        /// u.s.e.r@domain.com
        /// 
        /// так как в email us_e.r.@domain.main.com считался невалидным.
        /// </para>
        /// </remarks>
        public static readonly string EmailRegexpExpression =
            @"^([a-zA-Z0-9_!#\$`{|}~%&'*+\.+-])+\@((([a-zA-Z0-9-])|([а-яА-Я0-9]))+\.)+(([a-zA-Z0-9])|([а-яА-Я0-9]){2,4})+$";

        /// <summary>
        /// Если cтрока не null и имеет длину больше 2, и заключена в кавычки - "text ""text""",
        /// удаляем кавычки спереди и сзади, доблирующиеся кавычки удаляем - text "text"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteStartsEndsAndDoubleQuotes(this string  str)
        {
            if (!string.IsNullOrEmpty(str) && 
                str.Length > 2 && 
                str.StartsWith("\"") && 
                str.EndsWith("\""))
            {
                str = str.Substring(1, str.Length - 1);
                str = str.Substring(0, str.Length - 2);
                str = str.Replace("\"\"", "\"");
            }

            return str;
        }

        /// <summary>
        /// Извлекает текст тела странички без тегов из разметки HTML
        /// </summary>
        /// <param name="notificationText"></param>
        /// <returns></returns>
        public static string ExtractBodyTextFromHtml(this string notificationText)
        {
            if (string.IsNullOrEmpty(notificationText))
            {
                return notificationText;
            }

            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            Regex regx = new Regex("<body>(?<theBody>.*)</body>", options);
            Match match = regx.Match(notificationText);

            if (match.Success)
            {
                //извлекаем тело сообщения и удаляем из него все теги
                string theBody = match.Groups["theBody"].Value;
                return Regex.Replace(theBody, "<.*?>", string.Empty);
            }

            return notificationText;
        }

        /// <summary>
        /// Преобразование строки к числу.
        /// </summary>
        /// <param name="value">Входная строка.</param>
        /// <param name="ignoreSpaces">Игнорировать пробелы в строке.</param>
        /// <returns>Число.</returns>
        public static int? ToInt(this string value, bool ignoreSpaces = false)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            int result;
            string str = value;

            if (ignoreSpaces) str = str.Replace(" ", string.Empty);

            if (!int.TryParse(str, out result))
                return null;

            return result;
        }

        public static T ConvertValue<T>(this string value)
        {
            if (value.IsEmpty())
                return default(T);
            return (T) Convert.ChangeType(value, typeof (T));
        }

        public static string ToMoneyStringCommonServices(this decimal value)
        {
            var culture = CultureInfo.CreateSpecificCulture("ru-RU");
            culture.NumberFormat.NumberGroupSeparator = " ";

            return value.ToString("n", culture);
        }

        public static string ToMoneyStringCommonServices(this decimal? value)
        {
            if (value.HasValue)
                return value.Value.ToMoneyStringCommonServices();
            else
                return string.Empty;
        }

        public static bool ContainsIgnoreCase(this string text, string value)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(text, value, CompareOptions.IgnoreCase) >= 0;
        }

        public static string RemoveSubstring(this string text, params string[] substrings)
        {
            if (!string.IsNullOrEmpty(text))
            {
                foreach (var substring in substrings)
                {
                    text = text.Replace(substring, string.Empty);
                }
            }

            return text;
        }

        public static string RemoveWords(this string text, params string[] words)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = string.Join(" ", text.Split(' ')
                    .Where(w => !string.IsNullOrWhiteSpace(w))
                    .Where(w => !words.Contains(w)));
            }

            return text;
        }
    }
}
