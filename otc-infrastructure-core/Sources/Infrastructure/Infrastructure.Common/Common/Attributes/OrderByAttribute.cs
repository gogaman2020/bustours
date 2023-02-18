using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common.Attributes
{
    /// <summary>
    /// Атрибут у перечислений, используется для сортировки.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OrderByAttribute : Attribute
    {
        public int OrderBy { get; set; }

        public OrderByAttribute(int orderBy)
        {
            OrderBy = orderBy;
        }
    }
}
