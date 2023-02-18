namespace Infrastructure.Db.Common
{
    public interface IEntity : IEntity<int>
    {
    }

    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}