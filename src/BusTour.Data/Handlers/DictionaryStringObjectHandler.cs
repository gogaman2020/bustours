using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Db.TypeHandlers;

namespace BusTour.Data.Handlers
{
    public class DictionaryStringObjectHandler : DapperTypeHandler<Dictionary<string, object>>
    {
        public static readonly DictionaryStringObjectHandler Default = new DictionaryStringObjectHandler();

        public override Dictionary<string, object> Parse(object value)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, Dictionary<string, object> value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
