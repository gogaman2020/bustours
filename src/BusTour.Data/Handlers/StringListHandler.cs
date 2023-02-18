using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Db.TypeHandlers;
using BusTour.Domain.Entities;
using System.Linq;

namespace BusTour.Data.Handlers
{
    public class StringListHandler : DapperTypeHandler<List<string>>
    {
        public static readonly DictionaryStringStringHandler Default = new DictionaryStringStringHandler();

        public override List<string> Parse(object value)
        {
            return JsonConvert.DeserializeObject<List<string>>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, List<string> value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
