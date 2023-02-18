using System.Collections.Generic;

namespace Infrastructure.Db.Common.Crud
{
    public interface ICrudQuery
    {
        string GetInsert(IEnumerable<string> paramNames);
        string GetUpdate(IEnumerable<string> paramNames);
        string GetDelete(IEnumerable<string> paramNames);
        string GetSelect(IEnumerable<string> paramNames);
    }
}