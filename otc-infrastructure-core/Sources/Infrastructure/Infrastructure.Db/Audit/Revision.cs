using System;
using Infrastructure.Db.Common;

namespace Infrastructure.Db.Audit
{
    /// <summary>
    /// Ревизия операций изменения сущностей.
    /// </summary>
    public class Revision : BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя, выполнившего операцию.
        /// </summary>
        public int? CommonUserId { get; set; }

        /// <summary>
        /// Время выполнения операции.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
