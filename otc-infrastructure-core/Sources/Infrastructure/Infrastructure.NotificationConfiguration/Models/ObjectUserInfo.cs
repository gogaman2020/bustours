using Infrastructure.NotificationConfiguration.Enums;

namespace Infrastructure.NotificationConfiguration.Clients
{
    public class ObjectUserInfo
    {
        public int? UserId { get; set; }

        public int? WorkGroupId { get; set; }

        public string CathegoryCode { get; set; }

        public UserMarker Marker { get; set; }
    }
}
