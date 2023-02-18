using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Db.Audit.AuditQueries;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;

namespace Infrastructure.Db.Audit
{
    [InjectAsSingleton]
    public class AuditRepository : CrudRepository<Audit, AuditQuery>, IAuditRepository
    {
    }
}
