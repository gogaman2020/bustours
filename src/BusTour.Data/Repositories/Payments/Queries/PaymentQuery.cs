using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Data.Repositories.Payments.Queries
{
    public class PaymentQuery : CrudQuery<Payment, PaymentQuery>
    {
        public static string SelectByFilter(IEnumerable<string> fields) => Getter.Get(SelectName, null, fields);
    }
}
