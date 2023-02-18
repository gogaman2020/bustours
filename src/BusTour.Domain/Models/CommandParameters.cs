using System.Collections.Generic;

namespace BusTour.Domain.Models
{
    public class CommandParameters
    {
        public string Command { get; set; }

        public List<int> Ids { get; set; }
    }
}
