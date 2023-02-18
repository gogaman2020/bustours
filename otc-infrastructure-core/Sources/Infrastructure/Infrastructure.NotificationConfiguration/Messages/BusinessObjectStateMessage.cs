using Infrastructure.NotificationConfiguration.Enums;

namespace Infrastructure.NotificationConfiguration.Messages
{
    public class BusinessObjectStateMessage
    {
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string State { get; set; }
        public StateDirection Direction { get; set; }
        public DirectionMode Mode { get; set; }
    }
}
