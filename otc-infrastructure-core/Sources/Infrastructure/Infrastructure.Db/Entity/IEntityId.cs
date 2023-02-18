using System;

namespace Infrastructure.Db.Entity
{
    //todo
    public interface IEntityId
    {
        EId AsEntityId();
    }

    public struct EId
    {
        public string Value { get; }

        public bool Is<T>(T id)
            where T : IEntityId
        {
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EntityIdAttribute : Attribute
    {
        public string IdPrefix { get; }
        public Type StoreType { get; }
    }

    public interface IEntityIdTypeConverter
    {
        
    }
}