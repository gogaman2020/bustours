using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    public class TestResponseBus
    {
        public TestResponseSelectionResult SelectionResult { get; set; }

        public SelectionInfo Selection { get; set; }

        public Dictionary<string, TestResponseTable> Tables { get; set; }
    }
}

