using System;

namespace Infrastructure.Web.InfrastructurePlugin
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebPluginOrderAttribute : Attribute
    {
        public WebPluginOrderAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}