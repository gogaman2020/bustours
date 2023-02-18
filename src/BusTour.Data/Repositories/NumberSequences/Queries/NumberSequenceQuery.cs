using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Data.Repositories.NumberSequences.Queries
{
    public class NumberSequenceQuery : CrudQuery<NumberSequence, NumberSequenceQuery>
    {
        public static string Increment(IEnumerable<string> fields) =>
            Getter.Get("Increment", null, fields);
    }
}
