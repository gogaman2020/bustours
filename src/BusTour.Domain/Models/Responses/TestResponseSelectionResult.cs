using BusTour.Domain.Models.Selection;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    public class TestResponseSelectionResult
    {
        public bool IsAutoSelect { get; set; }
        public List<TestPathStep> Path { get; set; }
    }
}

