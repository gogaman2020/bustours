using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Attributes;

namespace Infrastructure.Common.Helpers
{
    public static class  EnumHelper
    {
        private static readonly ConcurrentDictionary<Type, object> OrderedEnums =
            new ConcurrentDictionary<Type, object>();
        
        public static T[] GetEnumValues<T>()
            where T : Enum
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new Exception("Метод принимает в качестве типа только Enum");
            }

            return (T[])OrderedEnums.GetOrAdd(typeof(T), GetOrderedArray<T>());
        }

        private static T[] GetOrderedArray<T>()
            where T : Enum
        {
            var items = Enum.GetValues(typeof(T))
                .OfType<T>()
                .OrderBy(item => item.GetOrderBy())
                .ToArray();
            return items;
        }

        /// <summary>
        /// Получить значение OrderBy атрибута OrderByAttribute у перечисления.
        /// </summary>
        /// <param name="obj">Перечисление.</param>
        /// <returns>Значение OrderBy атрибут OrderByAttribute у перечисления.</returns>
        public static int GetOrderBy<T>(this T obj)
            where T : Enum
        {
            var attribute = obj.GetType().GetField(obj.ToString())
                ?.GetCustomAttribute<OrderByAttribute>(false);
            return attribute?.OrderBy ?? default;
        }

        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
            {
                return null;
            }
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static IEnumerable<T> GetAttributesOfType<T>(this Enum enumVal)
            where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
            {
                return null;
            }
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.OfType<T>()
                .ToArray();
        }

        public static IEnumerable<TAttribute> GetAttributesOfType<TEnum, TAttribute>()
            where TEnum : Enum
            where TAttribute : Attribute
        {
            var type = typeof(TEnum);
            var members = type.GetMembers();
            var result = new List<TAttribute>();
            foreach (var member in members)
            {
                var attributes = member.GetCustomAttributes(typeof(TAttribute), false);
                result.AddRange(attributes.OfType<TAttribute>());
            }

            return result.Distinct()
                .ToArray();
        }
    }
}
