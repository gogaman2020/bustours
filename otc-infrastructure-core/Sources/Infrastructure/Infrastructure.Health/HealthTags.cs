namespace Infrastructure.Health
{
    public static class HealthTags
    {
        public static readonly string[] DB = new[] { "DataBase" };
        public static readonly string[] CLIENT = new[] { "Client" };
        public static readonly string[] RABBITMQ = new[] { "RabbitMq" };
    }
}
