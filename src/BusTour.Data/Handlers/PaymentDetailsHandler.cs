using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Db.TypeHandlers;
using BusTour.Domain.Entities;
using System.Linq;

namespace BusTour.Data.Handlers
{
    public class PaymentDetailsHandlerHandler : DapperTypeHandler<PaymentDetails>
    {
        public static readonly DictionaryStringStringHandler Default = new DictionaryStringStringHandler();

        public override PaymentDetails Parse(object value)
        {
            return JsonConvert.DeserializeObject<PaymentDetails>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, PaymentDetails value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
