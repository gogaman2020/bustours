namespace Infrastructure.Queue.Configs.Old
{
    public class BackCompatibilitySettings
    {
        public ExchangeType ExchangeType { get; set; }
        public string RoutingKey { get; set; }
        public string Topic { get; set; }
        public string Header { get; set; }
    }
}
