using System;

namespace Infrastructure.Db.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomFieldNameAttribute : Attribute
    {
        public CustomFieldNameAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}