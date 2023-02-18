using Infrastructure.Db.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Db.Common
{
    public abstract class FilterQueryObject : IQueryObject
    {

        protected FilterQueryObject()
        {
        }

        public abstract string GetQuery();

        public abstract object GetParams();

        public IEnumerable<string> FilterParamNames(bool ignoreEnums = false, bool ignoreCollections = false)
        {
            var names = EntityExtensions.GetQueryParameters(GetParams(), ignoreEnums, ignoreCollections)
                .Where(kv => kv.Value != null)
                .Select(kv => kv.PropName);

            return names;
        }

        public static FilterQueryObject<T> For<T>(Func<IEnumerable<string>, string> getSql = null, T filter = default(T))
            where T: new()
        {
            return new FilterQueryObject<T>(filter ?? new T(), c => getSql?.Invoke(c.FilterParamNames()));
        }

        public static FilterQueryObject<T> For<T>(T filter, Func<FilterQueryObject, string> getSql = null)
        {
            return new FilterQueryObject<T>(filter, getSql);
        }

        public static FilterQueryObject<T> For<T>(T filter, Func<IEnumerable<string>, string> getSql = null, bool ignoreEnums = false)
        {
            return new FilterQueryObject<T>(filter, c => getSql?.Invoke(c.FilterParamNames(ignoreEnums: ignoreEnums)));
        }

        public static DataSourceQueryObject<T> For<T>(DataSourceRequest<T> filter, Func<IEnumerable<string>, Dictionary<string, string>, string> getSql = null)
            where T : class
        {
            return new DataSourceQueryObject<T>(filter, getSql);
        }
    }

    public class FilterQueryObject<T> : FilterQueryObject
    {
        private Func<FilterQueryObject, string> _getSql = null;

        protected FilterQueryObject()
        {
        }

        public FilterQueryObject(T filter, Func<FilterQueryObject, string> getSql = null)
        {
            Filter = filter;
            _getSql = getSql;
        }

        public T Filter { get; set; }


        public override object GetParams() => Filter;

        public override string GetQuery()
        {
            return _getSql?.Invoke(this);
        }
    }

    public class DataSourceQueryObject<T> : FilterQueryObject
        where T : class
    {
        private readonly Func<IEnumerable<string>, Dictionary<string, string>, string> _getSql = null;

        protected DataSourceQueryObject()
        {
        }

        public DataSourceQueryObject(DataSourceRequest<T> filter, Func<IEnumerable<string>, Dictionary<string, string>, string> getSql = null)
        {
            Filter = filter;
            _getSql = getSql;
        }

        public DataSourceRequest<T> Filter { get; set; }


        public override object GetParams() => Filter.GetFilter();

        public override string GetQuery()
        {
            return _getSql?.Invoke(FilterParamNames(), Filter.GetPagingAndOrder());
        }
    }
}