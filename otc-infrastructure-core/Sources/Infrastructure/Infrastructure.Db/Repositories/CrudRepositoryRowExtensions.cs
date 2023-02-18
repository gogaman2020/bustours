using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Db.Repositories
{
    public static class CrudRepositoryRowExtensions
    {
        public static int FirstAsInt(this IDictionary<string, object> result)
        {
            return Convert.ToInt32(result?.Values.First());
        }
    }
}