using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Process.Helpers
{
    internal static class PcocessStepsGetter
    {
        public static IEnumerable<Type> GetStepTypes<T>(this IProcess<T> process)
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .ToArray();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetExportedTypes();
                foreach (var type in types)
                {
                    if (type.IsPublic && !type.IsAbstract && type.IsClass)
                    {
                        if (type.GetInterfaces().Any(p => p == typeof(IProcessStep<T>)))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IProcessStep<T>> GetSteps<T>(this IProcess<T> process)
        {
            return process.GetStepTypes()
                .Select(p => (IProcessStep<T>)Activator.CreateInstance(p, process))
                .ToArray();
        }
    }
}
