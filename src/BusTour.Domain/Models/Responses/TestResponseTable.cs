using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    public class TestResponseTable
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public bool Locked { get; set; }

        public bool Selected { get; set; }

        public bool IsRecommended { get; set; }

        public bool Available { get; set; }

        public Dictionary<string, TestResponseSeat> Seats { get; set; }
    }
}

