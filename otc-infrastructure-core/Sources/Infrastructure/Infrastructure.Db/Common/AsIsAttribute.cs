using System;

namespace Infrastructure.Db.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AsIsAttribute : Attribute
    {
    }
}