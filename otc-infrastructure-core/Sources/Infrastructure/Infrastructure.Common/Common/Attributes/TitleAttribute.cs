using System;

namespace Infrastructure.Common.Attributes
{
    /// <summary>
    /// Атрибут у перечислений, используется для описания элементов перечисления.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TitleAttribute : Attribute
    {
        public string Title { get; set; }
        public string AdditionalTitle { get; set; }

        public TitleAttribute(string title, string additionalTitle)
        {
            Title = title;
            AdditionalTitle = additionalTitle;
        }

        public TitleAttribute(string title)
            : this(title, string.Empty)
        {
        }
    }
}