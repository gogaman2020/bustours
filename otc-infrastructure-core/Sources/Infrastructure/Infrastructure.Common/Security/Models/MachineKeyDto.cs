using Infrastructure.Common.Configs;

namespace Infrastructure.Security.Models
{
    [Config("MachineKeyConfig")]
    public class MachineKeyDto
    {
        public string ValidationKey { get; set; }
        public string DecryptionKey { get; set; }
        public string DecryptionAlgorithm { get; set; }
        public string ValidationAlgorithm { get; set; }
        public string PrimaryPurpose { get; set; }
    }
}
