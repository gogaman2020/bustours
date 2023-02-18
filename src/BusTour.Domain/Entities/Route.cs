using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Route : BaseEntity
    {
        public Dictionary<string, string> Name { get; set; }

        public Dictionary<string, string> Description { get; set; }

        public TimeSpan Duration { get; set; }

        public string TitleImgPath { get; set; }

        public string MapImgPath { get; set; }

        public int CityId { get; set; }

        public Dictionary<string, string> CityName { get; set; }

        public Dictionary<string, string> DepartureAddress { get; set; }

        public Dictionary<string, string> DepartureHowToGet { get; set; }

        public List<string> ImgPaths { get; set; }
    }
}
