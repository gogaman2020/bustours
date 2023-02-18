namespace Infrastructure.Common.Models.ListItem
{
    public class SelectedListItem
    {
        public string Text { get; set; }

        public int Value { get; set; }

        public bool CanNotBeSelected { get; set; }

        public bool Equals(SelectedListItem other)
        {
            return Value == other.Value;
        }
    }

    public class SelectedListEnumItem : SelectedListItem
    {
        public string Name { get; set; }
    }
}
