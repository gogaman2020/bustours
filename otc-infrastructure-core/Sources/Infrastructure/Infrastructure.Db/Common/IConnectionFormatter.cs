namespace Infrastructure.Db.Common
{
    public interface IConnectionFormatter
    {
        public string Table(string tableName)
        {
            return tableName;
        }

        public string Column(string columnName)
        {
            return columnName;
        }
    }
}