using Infrastructure.Common.Exceptions;
using Infrastructure.Db.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Process.Repositories
{
    public class FilterObject<T> : FilterQueryObject<T>
        where T : ProcessData, IEntity, new()
    {
        private static ConcurrentDictionary<Type, Dictionary<string, string>> _typeFields = new ConcurrentDictionary<Type, Dictionary<string,string>>();

        private readonly Func<IEnumerable<string>, Dictionary<string, string>, string> _getQuery;
        public FilterObject(T filter, Func<IEnumerable<string>, Dictionary<string,string>, string> getQuery)
        {
            Filter = filter;
            _getQuery = getQuery;
        }

        public override string GetQuery()
        {
            var type = typeof(T);
            if (!_typeFields.TryGetValue(type, out var values))
            {
                var tableName = type.GetCustomAttributes(typeof(TableNameAttribute))
                    .OfType<TableNameAttribute>()
                    .FirstOrDefault()
                    ?.TableName;

                if (string.IsNullOrEmpty(tableName))
                {
                    throw new BusinessLogicException($"Can't resolve table name of type {type.FullName}");
                }

                var properties = new List<string>();
                var filterProperties = new List<string>();
                foreach (var property in type.GetProperties())
                {
                    var propName = property.GetCustomAttribute<TableFieldAttribute>();
                    if (!string.IsNullOrEmpty(propName?.Name))
                    {
                        properties.Add(propName.Name);
                        if (propName.IsFilter)
                        {
                            filterProperties.Add(propName.Name);
                        }
                    }
                }

                values = new Dictionary<string, string>
                {
                    { nameof(TableNameAttribute.TableName), tableName },
                    { "Fields", string.Join(", ", properties) },
                    { "FieldValues", string.Join(", ", properties.Select(p => $"@{p}")) },
                    { "UpdateFieldValues", string.Join(", ", properties.Select(p => $"{p} = @{p}")) },
                    { "Filter", string.Join(" and ", filterProperties.Select(p => $"t.{p} = @{p}")) }
                };

                _typeFields.AddOrUpdate(type, (t) => values, (t, v1) => values);
            }

            var fields = FilterParamNames();
            return _getQuery?.Invoke(fields, values);
        }

        public override object GetParams()
        {
            return Filter;
        }
    }
}
