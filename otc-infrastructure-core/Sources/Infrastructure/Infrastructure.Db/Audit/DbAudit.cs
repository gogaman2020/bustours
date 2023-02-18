using System;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Db.Audit.AuditQueries;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;
using Newtonsoft.Json;

namespace Infrastructure.Db.Audit
{
    public class DbAudit<TEntity> : DbAudit<int, TEntity>
        where TEntity : IEntity
    {
        public DbAudit(IDb db, Scoped<IRevisionManager> revisionManager)
            : base(db, revisionManager)
        {
        }
    }

    public class DbAudit<TId, TEntity>
        where TEntity : IEntity<TId>
    {
        private readonly IDb _db;
        private readonly Scoped<IRevisionManager> _revisionManager;
        private readonly bool _enabled;
        private readonly string _entityType;

        public DbAudit(IDb db, Scoped<IRevisionManager> revisionManager)
        {
            _db = db;
            _revisionManager = revisionManager;
            var attr = typeof(TEntity).GetCustomAttributes(typeof(EntityAuditAttribute), true).FirstOrDefault();
            _enabled = (attr is EntityAuditAttribute) && _revisionManager != null;
            _entityType = typeof(TEntity).Name;
        }

        /// <summary>
        /// Сохранение истории изменения сущности в БД.
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности.</typeparam>
        /// <param name="entity">Сущность.</param>
        /// <param name="crudOperation">Операция.</param>
        /// <returns></returns>
        public async Task SaveHistoryAsync(TEntity entity, CrudOperation crudOperation)
        {
            if (!_enabled) return;
            var revisionId = await _revisionManager.ServiceRequired.GetCurrentRevisionNumber();
            await SaveHistoryAsync(entity, crudOperation, revisionId);
        }

        public int? GetCurrentUserId()
        {
            return _revisionManager.ServiceRequired.CurrentUserId;
        }

        private async Task SaveHistoryAsync(TEntity entity, CrudOperation crudOperation, int revisionId)
        {
            if (!_enabled || typeof(TId) != typeof(int)) return;

            var audit = new Audit
            {
                RevisionId = revisionId,
                EntityType = _entityType,
                EntityId = Convert.ToInt32(entity.Id),
                Operation = crudOperation.ToString(),
                Data = JsonConvert.SerializeObject(entity)
            };

            await _db.ExecuteAsync(new CrudQueryObject<Audit, AuditQuery>(audit, CrudOperation.Insert));
        }

        public class Revision
        {
            public Revision(DbAudit<TId, TEntity> audit, int revisionId)
            {
                Audit = audit;
                RevisionId = revisionId;
            }

            private DbAudit<TId, TEntity> Audit { get; }
            private int RevisionId { get; }

            public Task SaveHistoryAsync(TEntity entity, CrudOperation crudOperation)
            {
                return Audit == null 
                    ? Task.CompletedTask 
                    : Audit.SaveHistoryAsync(entity, crudOperation, RevisionId);
            }
        }

        public async Task<Revision> WithRevisionAsync()
        {
            if (!_enabled)
            {
                return null;
            }
            var revisionId = await _revisionManager.ServiceRequired.GetCurrentRevisionNumber();
            return new Revision(this, revisionId);
        }
    }
}