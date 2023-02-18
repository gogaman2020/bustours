using BusTour.Domain.Entities;
using BusTour.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.Domain.Extensions
{
    public static class QueryExtensions
    {
        public static List<T> ToList<T>(this IEnumerable<T> source, Func<T,bool> predicate)
        {
            return source.Where(predicate).ToList();
        }

        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }
    }
}