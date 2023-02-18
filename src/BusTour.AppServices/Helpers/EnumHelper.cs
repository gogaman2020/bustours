using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BusTour.AppServices.Helpers
{
    public static class EnumHelper
    {
        public static T GetAttributeOfType<T>(this Enum enumVal) 
            where T : Attribute
        {
            var type = enumVal.GetType();

            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return null;

            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static IEnumerable<T> GetAttributesOfType<T>(this Enum enumVal)
            where T : Attribute
        {
            var type = enumVal.GetType();

            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return null;

            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return attributes.OfType<T>().ToArray();
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

            return result.Distinct().ToArray();
        }

        public static string GetDescription(this Enum enumVal)
        {
            var description = enumVal.GetAttributeOfType<DescriptionAttribute>();

            return description?.Description;
        }
    }
}
