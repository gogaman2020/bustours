namespace Infrastructure.Common.Models.ListItem
{
    public class SelectedListItemString
    {
        public string Text { get; set; }

        public string Value { get; set; }

        public bool CanNotBeSelected { get; set; }

        public bool Equals(SelectedListItemString other)
        {
            return Value == other.Value;
        }
    }
}