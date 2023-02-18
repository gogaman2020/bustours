using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Db.SqlScriptManagement
{
    public static class SqlScriptManager<T>
    {
        private static SqlScriptCache _cache = new SqlScriptCache();

        public static async ValueTask<string> GetAsync(string name, IEnumerable<string> sectionNames = null,
            IEnumerable<string> paramNames = null, Dictionary<string, string> values = null)
        {
            if (sectionNames == null && paramNames == null)
            {
                return await _cache.GetAsync(name, QueryGetter<T>.GetFileContentAsync);
            }

            var sql = await _cache.GetAsync(name, QueryGetter<T>.GetFileContentAsync, sectionNames);
            if (paramNames != null && paramNames.Count() > 0)
            {
                sql = sql.UseQueryParameters(paramNames);
            }

            if (values != null && values.Count > 0)
            {
                sql = sql.UseQueryData(values);
            }

            return sql;
        }
    }

    public interface ISqlScriptGetter
    {
        string GetFileContent(string fileName);

        string Get(string name, IEnumerable<string> sectionNames = null, IEnumerable<string> paramNames = null,
            Dictionary<string, string> values = null);
    }

    public class SqlScriptGetter<T> : ISqlScriptGetter
    {
        public string GetFileContent(string fileName)
        {
            return SqlScriptManager<T>.GetAsync(fileName).Result;
        }

        public string Get(string name, IEnumerable<string> sectionNames = null, IEnumerable<string> paramNames = null,
            Dictionary<string, string> values = null)
        {
            return SqlScriptManager<T>.GetAsync(name, sectionNames, paramNames, values).Result;
        }
    }
}