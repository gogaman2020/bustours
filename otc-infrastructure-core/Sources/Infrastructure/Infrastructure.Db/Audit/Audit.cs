using Infrastructure.Db.Common;

namespace Infrastructure.Db.Audit
{
    /// <summary>
    /// История изменения сущности.
    /// </summary>
    public class Audit : BaseEntity
    {
        /// <summary>
        /// Идентификатор ревизии.
        /// </summary>
        public int RevisionId { get; set; }

        /// <summary>
        /// Тип сущности.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Операция.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Сериализованная сущность.
        /// </summary>
        public string Data { get; set; }
    }
}
