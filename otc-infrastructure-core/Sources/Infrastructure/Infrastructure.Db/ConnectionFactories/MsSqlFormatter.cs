using Infrastructure.Common.DI;
using Infrastructure.Db.Common;

namespace Infrastructure.Db.ConnectionFactories
{
    [InjectAsSingleton(typeof(MsSqlFormatter))]
    public class MsSqlFormatter : IConnectionFormatter
    {
        
    }
}