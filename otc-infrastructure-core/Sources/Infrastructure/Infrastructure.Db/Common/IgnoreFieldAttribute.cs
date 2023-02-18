using System;

namespace Infrastructure.Db.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreFieldAttribute : Attribute
    {
    }
}