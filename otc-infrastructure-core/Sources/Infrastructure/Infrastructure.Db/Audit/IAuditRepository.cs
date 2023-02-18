using Infrastructure.Db.Repositories;

namespace Infrastructure.Db.Audit
{
    public interface IAuditRepository : ICrudRepository<Audit>
    {
    }
}