using System;

namespace Infrastructure.Db.Audit
{
    /// <summary>
    /// Атрибут, указывающий, что история изменения сущности должна сохраняться в БД.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAuditAttribute : Attribute
    {
    }
}
