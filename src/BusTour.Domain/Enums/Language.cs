using System;

namespace BusTour.Domain.Enums
{
    public enum Language
    {
        English = 1,
        French = 2,
        Russian = 3,
        Chinese = 4
    }

    public static class LanguageExtensions
    {
        public static string ToCode(this Language language)
        {
            switch (language)
            {
                case Language.English:
                    return "en";
                case Language.French:
                    return "fr";
                case Language.Russian:
                    return "ru";
                case Language.Chinese:
                    return "zh";
                default:
                    throw new ArgumentOutOfRangeException("language", language, "Unknown language");
            }
        }
    }
}
