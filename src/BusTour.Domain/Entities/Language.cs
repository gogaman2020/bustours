using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    public class Language: BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
