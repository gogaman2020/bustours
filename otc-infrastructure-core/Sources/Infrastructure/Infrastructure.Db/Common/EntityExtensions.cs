using Infrastructure.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Db.Common
{
    public static class EntityExtensions
    {
        private static readonly string[] DomainSubstrings =
        {
            "entity", "entities", "domain",
        };

        /// <summary>
        /// Сравнивает сущность из базы с ДТО и возвращает результат сравнения.
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности.</typeparam>
        /// <typeparam name="TDto">Тип ДТО.</typeparam>
        /// <param name="entity">Сущность.</param>
        /// <param name="dto">Дто.</param>
        /// <returns>Результат сравнения.</returns>
        public static bool EqualsToDto<TEntity, TDto>(this TEntity entity, TDto dto)
            where TEntity : IEntity
        {
            var entityType = typeof(TEntity);
            var dtoType = typeof(TDto);

            var dtoPropertiesDictionary = dtoType.GetProperties().ToDictionary(p => p.Name, p => p);
            var entityPropertiesDictionary = entityType.GetProperties().ToDictionary(p => p.Name, p => p);
            var haveAnyProperty = false;

            foreach (var propertyInfo in dtoPropertiesDictionary)
            {
                if (entityPropertiesDictionary.ContainsKey(propertyInfo.Key))
                {
                    if (!haveAnyProperty)
                    {
                        haveAnyProperty = true;
                    }

                    var entityPropertyInfo = entityPropertiesDictionary[propertyInfo.Key];

                    if (entityPropertyInfo.PropertyType != propertyInfo.Value.PropertyType)
                    {
                        return false;
                    }

                    var entityPropertyValue = entityPropertyInfo.GetValue(entity);
                    var dtoPropertyValue = propertyInfo.Value.GetValue(dto);

                    if (!object.Equals(dtoPropertyValue, entityPropertyValue))
                    {
                        return false;
                    }
                }
            }

            return haveAnyProperty;
        }

        public static IEnumerable<(string PropName, object Value)> GetQueryParameters(object obj, bool ignoreEnums = false, bool ignoreCollections = true)
        {
            return GetQueryPropertiesRecursively(obj, string.Empty, ignoreEnums, ignoreCollections);
        }

        /// <summary>
        /// Рекурсивный метод обхода объекта для сбора параметров
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="parentKey">Ключ родительской сущности</param>
        /// <param name="ignoreEnums">Флаг игнорирования енамов</param>
        /// <param name="ignoreCollections">Флаг игнорирование коллекций</param>
        /// <returns>Список пар: свойство - значение</returns>
        private static IEnumerable<(string PropName, object Value)> GetQueryPropertiesRecursively(object obj, string parentKey, bool ignoreEnums = false, bool ignoreCollections = true)
        {
            parentKey = parentKey ?? string.Empty;

            var props = new List<(string, object)>();
            if (obj != null)
            {
                foreach (var prop in obj?.GetType().GetProperties())
                {
                    var iAttr = prop.GetCustomAttribute<IgnoreFieldAttribute>(false);
                    if (iAttr != null)
                    {
                        continue;
                    }

                    var mAttr = prop.GetCustomAttribute<ModelFieldsAttribute>(false);
                    if (mAttr != null)
                    {
                        //ВНИМАНИЕ! \/ Рекурсия \/
                        var innerProps = GetQueryPropertiesRecursively(prop.GetValue(obj), $"{parentKey}{mAttr.FieldName}", ignoreEnums, ignoreCollections);
                        //ВНИМАНИЕ! /\ Рекурсия /\

                        if (innerProps.IsNotEmpty())
                        {
                            props.AddRange(innerProps);
                        }

                        continue;
                    }

                    var asIsAttr = prop.GetCustomAttribute<AsIsAttribute>(false);
                    if (asIsAttr != null)
                    {
                        props.Add(($"{parentKey}{prop.Name}", prop.GetValue(obj)));
                        continue;
                    }

                    var ns = prop.PropertyType.Namespace?.ToLower();
                    var allowNs = ns != null && DomainSubstrings.Any(s => ns.Contains(s));

                    if (ignoreCollections && (IsPropertyACollection<IEntity>(prop) || IsPropertyACollection(prop)))
                    {
                        props.Add(($"{parentKey}{prop.Name}", null));
                        continue;
                    }

                    if (!allowNs && IsPropertyACollection(prop) && !IsPropertyACollection<IEntity>(prop))
                    {
                        var type = GetUnderlyingTypeOfPropertyACollection(prop);
                        if (type.IsEnum && !ignoreEnums)
                        {
                            var tobj = prop.GetValue(obj) as IEnumerable;
                            if (tobj != null)
                            {
                                var value = tobj
                                    .Cast<int>()
                                    .ToArray();
                                props.Add(($"{parentKey}{type.Name}", value));
                                continue;
                            }
                        }
                    }

                    var attr = prop.GetCustomAttribute<CustomFieldNameAttribute>(false);
                    if (allowNs && !prop.PropertyType.IsEnum)
                    {
                        var tobj = prop.GetValue(obj);
                        var type = prop.PropertyType;

                        var fieldName = attr?.FieldName ?? $"{type.Name}Id";
                        var propName = $"{parentKey}{fieldName}";

                        if (!obj.GetType().GetProperties().Any(prop => prop.Name == propName))
                        {
                            var value = tobj == null
                                ? null
                                : type.GetProperty("Id")?.GetValue(tobj);

                            props.Add(($"{parentKey}{propName}", value));
                        }
                        else
                        {
                            props.Add(($"{parentKey}{type.Name}", null));
                        }
                    }
                    else if (allowNs && prop.PropertyType.IsEnum)
                    {
                        var fieldName = attr?.FieldName ?? prop.Name;
                        var name = $"{parentKey}{fieldName}";
                        props.Add(
                            ignoreEnums
                                ? (name, null)
                                : (name, prop.GetValue(obj))
                            );
                    }
                    else
                    {
                        props.Add(($"{parentKey}{prop.Name}", prop.GetValue(obj)));
                    }
                }
            }

            return props;
        }

        /// <summary>
        /// Проверяет, что свойство - коллекция IEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsPropertyACollection<T>(PropertyInfo property)
            where T: IEntity
        {
            if (IsString(property.PropertyType))
            {
                return false;
            }

            if (property.PropertyType.IsGenericType && property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                var typeParameters = property.PropertyType.GetGenericArguments();

                return typeParameters.Any(p => p.GetInterface(typeof(T).FullName) != null);
            }

            return false;
        }

        /// <summary>
        /// Проверяет, что свойство - коллекция пользовательского типа
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsPropertyACollection(PropertyInfo property)
        {
            var type = GetUnderlyingTypeOfPropertyACollection(property);
            if (type != null && !type.Namespace.StartsWith("System"))
            {
                return true;
            }

            return false;
        }

        private static Type GetUnderlyingTypeOfPropertyACollection(PropertyInfo property)
        {
            if (IsString(property.PropertyType))
            {
                return null;
            }

            if (property.PropertyType.IsArray)
            {
                return property.PropertyType.GetInterfaces().FirstOrDefault(i=>i.IsGenericType)?.GetGenericArguments()[0];
            }

            if (property.PropertyType.IsGenericType &&
                property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                return property.PropertyType.GetGenericArguments().FirstOrDefault();
                //Костыль для определения пользовательских типов: Assembly.FullName может не совпадать
                //return typeParameters.Any(p => !p.Namespace.StartsWith("System"));
            }

            return null;
        }

        private static bool IsString(Type type)
        {
            return type == typeof(string);
        }

        public static object CopyFrom(this object target, object source)
        {
            var sourceProps = source.GetType().GetProperties().Where(x => x.CanRead).ToList();
            var targetProps = target.GetType().GetProperties().Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (targetProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = targetProps.First(x => x.Name == sourceProp.Name);
                    p.SetValue(target, sourceProp.GetValue(source, null), null);
                }
            }

            return target;
        }
    }
}