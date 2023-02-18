using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Languages.Queries
{
    public class GetLanguagesQuery : CrudQuery<Language, GetLanguagesQuery>
    {
        public static string SelectByFilter(IEnumerable<string> fields) =>
            Getter.Get("SelectLanguages", null, fields);
    }
}
