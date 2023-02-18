using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    public class TestResponseDebugInfo
    {
        public TestResponseRulesTable Table { get; set; }
        public TestResponseRulesTable Legend { get; set; }
    }

    public class TestResponseRulesTable
    {
        public List<TestResponseRulesRow> Rows { get; } = new List<TestResponseRulesRow>();
    }

    public class TestResponseRulesRow
    {
        public string Code { get; set; }
        public List<TestResponseRulesCell> Cells { get; } = new List<TestResponseRulesCell>();
    }

    public class TestResponseRulesCell
    {
        public string Code { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }
        public bool IsBold { get; set; }
        public bool IsLeftAlign { get; set; }
        public string Style => $"color: {Color}; background: {Background}; text-align: {(IsLeftAlign ? "left" : "center")}; font-weight: {(IsBold ? "bold" : "normal")}";
    }
}

