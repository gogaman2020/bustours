using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Attributes;
using Infrastructure.Common.Extensions;
using Infrastructure.Common.Helpers;
using Infrastructure.Common.Models.ListItem;

namespace Infrastructure.UI.ListItem
{
    public class SelectedListItemHelper
    {
        public static IEnumerable<SelectedListEnumItem> GetEnumItems<T>(params T[] exceptItems) where T : Enum
        {
            var enumerable = EnumHelper.GetEnumValues<T>() as IEnumerable<T>;
            if (exceptItems != null)
            {
                enumerable = enumerable.Except(exceptItems);
            }

            return enumerable.Select(x => new SelectedListEnumItem { Value = Convert.ToInt32(x), Text = x.GetTitle(), Name = x.ToString() })
                .ToList();
        }

        public static IEnumerable<SelectedListEnumItem> GetEnumItemsWithSorts<T>(params T[] exceptItems) where T : Enum
        {
            var enumerable = EnumHelper.GetEnumValues<T>() as IEnumerable<T>;
            if (exceptItems != null)
            {
                enumerable = enumerable.Except(exceptItems);
            }

            var sortable = new SortedList<int, SelectedListEnumItem>();
            foreach (var item in enumerable)
            {
                var value = new SelectedListEnumItem
                {
                    Value = Convert.ToInt32(item),
                    Text = item.GetTitle(),
                    Name = item.ToString()
                };

                var attr = item.GetAttributeOfType<OrderByAttribute>();
                var key = attr?.OrderBy ?? value.Value;
                sortable.Add(key, value);
            }

            return sortable.Select(p => p.Value)
                .ToList();
        }

        public static IEnumerable<SelectedListEnumItem> GetEnumItemsFrom<T>(IEnumerable<T> items) where T : Enum
        {
            return items.Select(x => new SelectedListEnumItem { Value = Convert.ToInt32(x), Text = x.GetTitle(), Name = x.ToString() })
                .ToList();
        }
    }
}