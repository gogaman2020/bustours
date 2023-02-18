using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common.Configs;
using Infrastructure.Db.ConnectionFactories;

namespace Infrastructure.Db.SqlScriptManagement
{
    public static class QueryGetter<T>
    {
        public static readonly string _msSqlTemplate = "{0}.{1}.sql";
        public static readonly string _pgSqlTemplate = "{0}.{1}.pg.sql";
        public static readonly string _mySqlTemplate = "{0}.{1}-my.sql";

        private static readonly TypeInfo TypeInfo = typeof(T).GetTypeInfo();

        public static Task<string> GetFileContentAsync(string fileName)
        {
            var streamName = string.Format(GetTemplate(), TypeInfo.Namespace, fileName);
            using (var stream = TypeInfo.Assembly.GetManifestResourceStream(streamName))
            {
                if(stream == null)
                {
                    throw new Exception($"Файл {streamName} не найден");
                }

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEndAsync();
                }
            }
        }

        private static string GetTemplate()
        {
            switch (Config.Get<DbConfig>().Type)
            {
                case DbConnectionFactoryType.PostgresSql:
                    return _pgSqlTemplate;
                case DbConnectionFactoryType.MsSql:
                    return _msSqlTemplate;
                case DbConnectionFactoryType.MySql:
                    return _mySqlTemplate;
                default:
                    throw new ArgumentOutOfRangeException("DbConfig.Type");
            }
        }
    }
}