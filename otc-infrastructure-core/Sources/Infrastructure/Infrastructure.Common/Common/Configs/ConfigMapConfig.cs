using System;
using System.Collections.Generic;

namespace Infrastructure.Common.Configs
{
    public static class ConfigMapConfig
    {
        private static Dictionary<Type, string> _dictionaryDefault = new Dictionary<Type, string>();

        public static IReadOnlyDictionary<Type, string> TypeNameMapping => _dictionaryDefault;

        public static void Configure<T>(string name)
        {
            SetName(typeof(T), name);
        }
        
        public static void SetName(Type type, string name)
        {
            _dictionaryDefault[type] = name;
        }
        
        public static string GetName(Type type, bool useTypeNameAsDefault = true)
        {
            return _dictionaryDefault.TryGetValue(type, out var result)
                ? result
                : useTypeNameAsDefault
                    ? type.Name
                    : null;
        }
    }
}