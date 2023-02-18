namespace Infrastructure.Mediator
{
    public static class CommandExtension
    {
        public static string Key(this IMediatorIdentity mediatorCommand)
        {
            return $"{mediatorCommand.Name}_{mediatorCommand.Id}";
        }
    }
}
