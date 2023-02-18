using System.Collections.Generic;

namespace Infrastructure.NotificationConfiguration.Clients
{
    public class BusinessObjectState
    {
        public int Id { get; set; }
        
        public string ObjectType { get; set; }

        public int OrganizationId { get; set; }

        public ObjectUserInfo[] UserInfos { get; set; }

        public Dictionary<string, string> ObjectParameters { get; set; }
    }
}
