using System.Collections.Generic;

namespace BusTour.Domain.Models.Selection
{
    public class TestSelectionResult : SelectionResult
    {
        public bool IsAutoSelect { get; set; }
        public List<TestPathStep> Path { get; set; }
    }
}

