using Infrastructure.Common.DI;
using Infrastructure.Db.Common;

namespace Infrastructure.Db.ConnectionFactories
{
    [InjectAsSingleton(typeof(MySqlFormatter))]
    public class MySqlFormatter : IConnectionFormatter
    {
        
    }
}