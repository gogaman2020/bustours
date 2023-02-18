using System;

namespace Infrastructure.Db.Common
{
    public struct ObjectId<T>
        where T : IEntity
    {
        private int _id;

        public int Get(T obj)
        {
            return obj?.Id ?? _id;
        }

        public void Set(int id, T obj)
        {
            _id = id;
            if (obj != null && obj.Id != id)
            {
                //нельзя ставить другой id если обьект есть, либо меняй в обьекте либо очищай ссылку и меняй
                throw new ArgumentException($"id:{id} is different from object.id:{obj.Id}");
            }
        }
    }

    public struct ObjectIdNullable<T>
        where T : IEntity
    {
        private int? _id;

        public int? Get(T obj)
        {
            return obj?.Id ?? _id;
        }

        public void Set(int? id, T obj)
        {
            _id = id;
            if (obj != null && obj.Id != id)
            {
                //нельзя ставить другой id если обьект есть, либо меняй в обьекте либо очищай ссылку и меняй
                throw new ArgumentException($"id:{id} is different from object.id:{obj.Id}");
            }
        }
    }

    /// <summary>
    /// Базовый объект.
    /// </summary>
    public class BaseEntity : BaseEntity<int>, IEntity
    {
    }

    /// <summary>
    /// Базовый объект.
    /// </summary>
    public class BaseEntity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
}