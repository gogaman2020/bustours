using System.Data;

namespace Infrastructure.Db.Common.Transactions
{
    public interface ITransactionContext
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
    }
}