using BusTour.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusTour.Domain.Helpers
{
    public static class ProcessHelper
    {
        private const string StepsAssemblyName = "BusTour.AppServices";

        public static Enum GetEnumItemByStepName(string stepName)
        {
            if (string.IsNullOrEmpty(stepName))
            {
                return null;
            }

            var stepClass = GetEntitiesProcessSteps()?.FirstOrDefault(type => type.Name == stepName);

            if (stepClass == null)
            {
                throw new ArgumentException($"The class representing step = \"{stepName}\" does not exists");
            }

            var attr = stepClass.GetCustomAttribute<StepEnumItemRelationAttribute>();

            if (attr == null)
            {
                throw new CustomAttributeFormatException($"The \"{stepClass}\" does not have the \"StepEnumItemRelationAttribute\" attribute specified");
            }
            
            return attr.State;
        }

        public static List<string> GetStepNamesByEnumItems<TEnum>(IEnumerable<TEnum> enumItems)
            where TEnum : Enum
        {
            if (enumItems?.Any() != true)
            {
                return null;
            }

            var stepClasses = GetEntitiesProcessSteps();

            List<string> stepNames = new List<string>();

            foreach (var enumItem in enumItems)
            {
                var classAnalogue = stepClasses?.FirstOrDefault(type =>
                {
                    var attr = type.GetCustomAttribute<StepEnumItemRelationAttribute>();

                    if (attr != null)
                    {
                        return Enum.Equals(attr.State, (Enum)enumItem);
                    }

                    return false;
                });

                if (classAnalogue != null)
                {
                    stepNames.Add(classAnalogue.Name);
                }
            }

            return stepNames;
        }

        private static IEnumerable<Type> GetEntitiesProcessSteps()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(x => x.GetName().Name == StepsAssemblyName)
                ?.GetTypes()
                ?.Where(type => !string.IsNullOrEmpty(type.Namespace) && type.Namespace.StartsWith(StepsAssemblyName) && type.IsClass);
        }
    }
}
