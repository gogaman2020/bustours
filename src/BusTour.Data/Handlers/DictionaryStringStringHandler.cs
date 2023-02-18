using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Db.TypeHandlers;

namespace BusTour.Data.Handlers
{
    public class DictionaryStringStringHandler : DapperTypeHandler<Dictionary<string, string>>
    {
        public static readonly DictionaryStringStringHandler Default = new DictionaryStringStringHandler();

        public override Dictionary<string, string> Parse(object value)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, Dictionary<string, string> value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
