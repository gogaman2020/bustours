using System;

namespace Infrastructure.Db.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TableFieldAttribute : Attribute
    {
        public TableFieldAttribute(string name)
        {
            Name = name;
        }

        public TableFieldAttribute(string name, bool isFilter)
            : this(name)
        {
            IsFilter = isFilter;
        }

        public string Name { get; private set; }

        public bool IsFilter { get; private set; }
    }
}