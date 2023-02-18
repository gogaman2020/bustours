using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Users.Queries
{
    public class GetUserQuery : CrudQuery<User, GetUserQuery>
    {
        public static string SelectByFilter(IEnumerable<string> fields) =>
            Getter.Get("SelectUser", null , fields);

        public object GetParams() => this;

        public int? Id { get; set; }

        public string UserName { get; set; }
    }
}