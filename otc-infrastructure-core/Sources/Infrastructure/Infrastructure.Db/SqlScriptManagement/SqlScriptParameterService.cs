using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Infrastructure.Db.SqlScriptManagement
{
    /// <summary>
    /// Расширения работы со sql запросами
    /// </summary>
    public static class SqlScriptParameterService
    {
        /// <summary>
        /// Шаблон поиска параметров
        /// </summary>
        private static string ParamRegexTemplate = @"(?>--\s*@{0})+?\b";

        /// <summary>
        /// Шаблон поиска значений
        /// </summary>
        private static string ValueRegexTemplate = @"(?>%{0}%)+?";

        /// <summary>
        /// Удаляет из запроса все комментарии вида --@ParameterName
        /// </summary>
        /// <param name="query">Исходный запрос</param>
        /// <param name="paramNames">Список параметров</param>
        /// <returns>Результирующий запрос</returns>
        public static string UseQueryParameters(this string query, IEnumerable<string> paramNames)
        {
            foreach (var paramName in paramNames)
            {
                query = query.ReplaceParameter(ParamRegexTemplate, paramName, string.Empty);
            }

            return query;
        }

        /// <summary>
        /// Удаляет из запроса все комментарии вида --@ParameterName
        /// и заменяет все подстроки вида %ParameterName% на ParameterValue
        /// </summary>
        /// <param name="query">Исходный запрос</param>
        /// <param name="paramData">Словарь параметров [ParameterName, ParameterValue]</param>
        /// <returns>Результирующий запрос</returns>
        public static string UseQueryData(this string query, Dictionary<string, string> paramData)
        {
            foreach (var kvp in paramData)
            {
                query = query.ReplaceParameter(ParamRegexTemplate, kvp.Key, string.Empty)
                    .ReplaceParameter(ValueRegexTemplate, kvp.Key, kvp.Value);
            }

            return query;
        }

        /// <summary>
        /// Заменяет в запросе по шаблону подстроки на нужное значение
        /// </summary>
        /// <param name="query">Исходный запрос</param>
        /// <param name="patternTemplate">Шаблон регулярного выражения</param>
        /// <param name="parameter">Параметр регулярного выражения</param>
        /// <param name="value">Заменяемое значение</param>
        /// <returns>Результирующий запрос</returns>
        private static string ReplaceParameter(this string query, string patternTemplate, string parameter, string value)
        {
            var pattern = string.Format(patternTemplate, parameter);
            return Regex.Replace(query, pattern, value);
        }
    }
}