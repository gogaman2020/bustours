using Infrastructure.Common.Configs;

namespace Infrastructure.Db.ConnectionFactories
{
    public enum DbConnectionFactoryType
    {
        MsSql,
        PostgresSql,
        MySql
    }
    
    [Config]
    public class DbConfig
    {
        public DbConnectionFactoryType? Type { get; set; }
        public string ConnectionString { get; set; }

        public static bool UseAudit { get; set; } = true;
    }
}
