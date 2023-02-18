namespace Infrastructure.Mediator
{
    public interface IMediatorIdentity
    {
        public string Name => GetType().Name;
        public string Id { get; }
        string Log();
    }
}
