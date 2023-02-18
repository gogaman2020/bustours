using System;

namespace Infrastructure.Common.Configs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
        public ConfigAttribute(string defaultConfigName = null, params string[] hiddenFields)
        {
            HiddenFields = hiddenFields;
            DefaultConfigName = defaultConfigName;
        }

        public string DefaultConfigName { get; }
        public string[] HiddenFields { get; }
    }
}