namespace Infrastructure.Db.Common
{
    public interface IQueryObject
    {
        string GetQuery();
        object GetParams();
    }
}