using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Db.SqlScriptManagement
{
    public class SqlScriptCache
    {
        //name -> section -> query
        private ConcurrentDictionary<string, Task<Dictionary<string, string>>> _cache = new ConcurrentDictionary<string, Task<Dictionary<string, string>>>();

        public async Task<string> GetAsync(string name, Func<string, Task<string>> sqlGetter)
        {
            var cached = await _cache.GetOrAdd(name, k => SqlParseToSections(sqlGetter(k)));
            return cached[""];
        }

        public async Task<string> GetAsync(string name, Func<string, Task<string>> sqlGetter, IEnumerable<string> sections)
        {
            var cached = await _cache.GetOrAdd(name, k => SqlParseToSections(sqlGetter(k)));

            if (sections == null)
            {
                return cached[""];
            }

            var tsections = sections?
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            var keystring = "--" + string.Join("--", tsections);
            if (!cached.TryGetValue(keystring, out var sql))
            {
                var sbcache = new StringBuilder();
                sbcache.Append(cached[""]);

                foreach (var section in tsections)
                {
                    if (cached.TryGetValue(section, out var sqlPart))
                    {
                        sbcache.Append(sqlPart);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException($"wrong key {section} allowed keys: {string.Join(",", cached.Keys)}");
                    }
                }

                sql = sbcache.ToString();
                //todo check parallel multi read - write behaviour
                lock (cached)
                {
                    cached[keystring] = sql;
                }
            }

            return sql;
        }

        private async Task<Dictionary<string, string>> SqlParseToSections(Task<string> sqlGetter)
        {
            var sql = await sqlGetter;

            var dic = new Dictionary<string, string>
            {
                [""] = SqlSectionParser.ReadMainPart(sql)
            };

            foreach (var keyValuePair in SqlSectionParser.ReadSections(sql))
            {
                dic[keyValuePair.Key] = keyValuePair.Value;
            }
            
            return dic;
        }
    }
}