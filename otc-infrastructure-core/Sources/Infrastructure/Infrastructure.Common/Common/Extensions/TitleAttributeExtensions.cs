using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Attributes;

namespace Infrastructure.Common.Extensions
{
    public static class TitleAttributeExtensions
    {
        public static string GetTitle(this Enum obj)
        {
            if (obj == null)
                return string.Empty;

            FieldInfo fi = obj.GetType().GetField(obj.ToString());

            if (fi == null)
            {
                return string.Empty;
            }

            var attribute = fi.GetCustomAttributes(typeof(TitleAttribute), false).Cast<TitleAttribute>()
                .FirstOrDefault();

            string title;
            if (attribute == null)
            {
                var description = fi.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>().FirstOrDefault();
                title = description != null ? description.Description : obj.ToString();
            }
            else
            {
                title = attribute.Title;
            }

            return title;
        }

        public static string GetTitle(this Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(TitleAttribute), false).Cast<TitleAttribute>()
                .FirstOrDefault();

            string title;
            if (attribute != null)
            {
                title = attribute.Title;
            }
            else
            {
                var description = type.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>().FirstOrDefault();
                title = description != null ? description.Description : type.Name;
            }

            return title;
        }
    }
}