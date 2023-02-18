using System;

namespace Infrastructure.Db.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelFieldsAttribute : Attribute
    {
        public ModelFieldsAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}