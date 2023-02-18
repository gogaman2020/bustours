using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.DI;

namespace Infrastructure.Common.Plugins
{
    public class Types
    {
        public static Type[] AvailableTypes => _types?.Value ?? Array.Empty<Type>();

        public static void Cleanup()
        {
            _types = null;
        }

        private static Lazy<Type[]> _types = new Lazy<Type[]>(GetTypes);

        private static Type[] GetTypes()
        {
            var path = AppContext.BaseDirectory;
            var files = Directory.EnumerateFiles(path, "*.dll");

            var pairs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(a =>
                {
                    var fullPath = Path.GetFullPath(a.Location, path);
                    if (!fullPath.Contains(path))
                    {
                        return false;
                    }

                    var file = Path.GetFileName(fullPath) ?? string.Empty;
                    return CheckAllowFile(file) && CheckAllowAssembly(a);
                })
                .Select(a => (a, Path.GetFullPath(a.Location, path)))
                .ToArray();

            var types = pairs
                .SelectMany(p => p.a.GetExportedTypes())
                .ToList();
            foreach (var file in files)
            {
                try
                {
                    if (!CheckAllowFile(Path.GetFileName(file)) || pairs.Any(p => p.Item2 == file))
                    {
                        continue;
                    }

                    var assembly = Assembly.LoadFrom(file);
                    if (CheckAllowAssembly(assembly))
                    {
                        types.AddRange(assembly.GetExportedTypes());
                    }
                }
                catch (Exception)
                {
                    //skip
                }
            }

            return types.ToArray();
        }

        private static readonly string[] SkipFilePrefixes = new[]
        {
            "System",
            "Microsoft",
            "NLog",
            "Dapper",
            "RabbitMQ",
            "HealthChecks",
            "Swashbuckle",
            "Newtonsoft",
            "Npgsql",
        };

        private static bool CheckAllowFile(string name)
        {
            return !SkipFilePrefixes.Any(p => name?.StartsWith(p) ?? true);
        }

        private static bool CheckAllowAssembly(Assembly assembly)
        {
            try
            {
                var testarray = assembly.GetExportedTypes()
                    .Select(t => t.GetCustomAttribute<InjectAsAttribute>(false))
                    .ToArray();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}