using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.CommonConfigs.Queries
{
    public class GetCommonConfigQuery : CrudQuery<CommonConfig, GetCommonConfigQuery>
    {
        public static string SelectByFilter(IEnumerable<string> fields) =>
            Getter.Get("SelectCommonConfig", null, fields);
    }
}
