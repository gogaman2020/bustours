namespace Infrastructure.UI.KendoGridHelper
{
    public static class KendoGroupHelper
    {
        ///// <summary>
        ///// Собирает на основе данных модель для отображения в KendoGrid.
        ///// </summary>
        ///// <typeparam name="TItem">Тип элемента в коллекции.</typeparam>
        ///// <typeparam name="TAggregate">Тип описателя агрегаций.</typeparam>
        ///// <param name="reportData">Данные отчета.</param>
        ///// <param name="toModelfunc">Функция создания ViewModel по Dto.</param>
        ///// <param name="filter">Функция фильтрации коллекции (Элемент, Имя свойства, Значение) => bool.</param>
        ///// <returns>Модель для Kendo.</returns>
        //public static List<KendoGroup> BuildReportGrouppedCollection<TItem, TAggregate>(AggregatedPageSearchResult<TItem> reportData, Func<TItem, dynamic> toModelfunc)
        //    where TItem : class
        //    where TAggregate : class, new()
        //{
        //    return BuildKendoGroup<TItem, TAggregate>(reportData.Aggregates, reportData.Items, toModelfunc) ?? new List<KendoGroup>(0);
        //}

        ///// <summary>
        ///// Создание структуры данных для грида с группировкой. 
        ///// Метод рекурсивный.
        ///// </summary>
        ///// <typeparam name="TItem">Тип элемента в коллекции.</typeparam>
        ///// <typeparam name="TAggregate">Тип описателя агрегаций.</typeparam>
        ///// <param name="aggregates">Коллекция агрегаций.</param>
        ///// <param name="items">Коллекция элементов типа T.</param>
        ///// <param name="toModelfunc">Функция создания ViewModel по Dto.</param>
        ///// <returns>Коллекция групп.</returns>
        //public static List<KendoGroup> BuildKendoGroup<TItem, TAggregate>(ReportAggregatesCollectionDto aggregates, IEnumerable<TItem> items, Func<TItem, dynamic> toModelfunc)
        //    where TItem : class
        //    where TAggregate : class, new()
        //{
        //    if (string.IsNullOrEmpty(aggregates.DataKey))
        //    {
        //        //это верхний уровень - ИТОГО
        //        if (aggregates.InnerAggregates == null || !aggregates.InnerAggregates.Any())
        //        {
        //            return null;
        //        }

        //        var innerResult = new List<KendoGroup>();
        //        foreach (var subGroup in aggregates.InnerAggregates)
        //        {
        //            var kgroups = BuildKendoGroup<TItem, TAggregate>(subGroup, items, toModelfunc);
        //            if (kgroups != null)
        //            {
        //                innerResult.AddRange(kgroups.Where(p => p != null));
        //            }
        //        }

        //        return innerResult;
        //    }

        //    //это все, что ниже
        //    var groupped = items.Where(p => PredicateWrapper.Invoke(p, aggregates.DataKey, aggregates.DataValue));
        //    if (!groupped.Any())
        //    {
        //        return null;
        //    }

        //    var hasSubGroups = aggregates.InnerAggregates != null && aggregates.InnerAggregates.Any();
        //    var kGroup = new KendoGroup
        //    {
        //        Field = aggregates.DataKey,
        //        Value = aggregates.DataValue,
        //        HasSubgroups = hasSubGroups,
        //        //если у группы есть подгруппы, то будем складывать их, иначе - сами элементы
        //        Items = hasSubGroups
        //            ? new List<dynamic>()
        //            : groupped.Select(p => (object)toModelfunc(p)).ToList(),
        //        Aggregates = FillAggregates<TAggregate>(aggregates.Aggregates, () => string.Format("Итого")),
        //    };

        //    if (hasSubGroups)
        //    {
        //        //создание подгрупп
        //        foreach (var subGroup in aggregates.InnerAggregates)
        //        {
        //            var kgroups = BuildKendoGroup<TItem, TAggregate>(subGroup, groupped.ToList(), toModelfunc);
        //            if (kgroups != null)
        //            {
        //                kGroup.Items.AddRange(kgroups.Where(p => p != null));
        //            }
        //        }
        //    }

        //    return new List<KendoGroup> { kGroup };
        //}

        ///// <summary>
        ///// Создает и заполняет агрегации для произвольного типа.
        ///// </summary>
        ///// <typeparam name="T">Тип объекта агрегации.</typeparam>
        ///// <param name="aggregations">Коллекция агрегаций.</param>
        ///// <param name="getAggregationName">Функция получения имени агрегаций.</param>
        ///// <returns>Агрегации.</returns>
        //public static T FillAggregates<T>(Dictionary<string, ReportAggregatesDto> aggregations, Func<string> getAggregationName)
        //    where T : class, new()
        //{
        //    var result = new T();

        //    var type = typeof(T);
        //    var properties = type.GetProperties();
        //    foreach (var property in properties)
        //    {
        //        if (property.PropertyType != typeof(KendoAggregate)) continue;

        //        var propertyAttributes = property.GetCustomAttributes(typeof(ReportAggregateAttribute), true);
        //        var attribute = propertyAttributes == null ? null : propertyAttributes.OfType<ReportAggregateAttribute>().FirstOrDefault();
        //        if (attribute == null) continue;

        //        KendoAggregate aggregate;
        //        if (attribute.IsAggregationName)
        //        {
        //            var name = getAggregationName();
        //            aggregate = new KendoAggregate
        //            {
        //                Sum = name,
        //                Min = name,
        //                Max = name,
        //                Average = name,
        //                Count = name
        //            };
        //        }
        //        else
        //        {
        //            aggregate = Get(aggregations, property.Name, attribute.ConvertType, attribute.IsThousand);
        //        }

        //        property.SetValue(result, aggregate);
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Строит динамические агрегации.
        ///// </summary>
        ///// <param name="aggregations">Источник.</param>
        ///// <returns>Результат.</returns>
        //public static dynamic FillDinamicAggregates(Dictionary<string, ReportAggregatesDto> aggregations)
        //{
        //    dynamic result = new ExpandoObject();

        //    foreach (var kvp in aggregations)
        //    {
        //        ((IDictionary<string, object>)result)[kvp.Key] = new KendoAggregate
        //        {
        //            Sum = kvp.Value.Sum.ToMoneyString(),
        //            Min = kvp.Value.Min.ToMoneyString(),
        //            Max = kvp.Value.Max.ToMoneyString(),
        //            Average = kvp.Value.Average.ToMoneyString(),
        //            Count = kvp.Value.Count.ToMoneyString()
        //        };
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Конвертит dto в модель для kendo
        ///// </summary>
        ///// <param name="aggregations">Словарь агрегаций.</param>
        ///// <param name="key">Свойство.</param>
        ///// <param name="convertType">Тип для конвертации значения агрегации.</param>
        ///// <param name="isThousand">Флаг, что необходимо значение разделить на 1000.</param>
        ///// <returns>Модель.</returns>
        //private static KendoAggregate Get(Dictionary<string, ReportAggregatesDto> aggregations, string key, Type convertType = null, bool isThousand = false)
        //{
        //    var result = new KendoAggregate();
        //    ReportAggregatesDto item;
        //    if (aggregations.TryGetValue(key, out item))
        //    {
        //        if (convertType == null)
        //        {
        //            result.Sum = isThousand ? item.Sum.ToThousandMoneyString() : item.Sum.ToMoneyString();
        //            result.Min = isThousand ? item.Min.ToThousandMoneyString() : item.Min.ToMoneyString();
        //            result.Max = isThousand ? item.Max.ToThousandMoneyString() : item.Max.ToMoneyString();
        //            result.Average = isThousand ? item.Average.ToThousandMoneyString() : item.Average.ToMoneyString();
        //            result.Count = isThousand ? item.Count.ToThousandMoneyString() : item.Count.ToMoneyString();
        //        }
        //        else
        //        {
        //            Type tProp = convertType.IsGenericType && convertType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
        //                ? new NullableConverter(typeof(decimal)).UnderlyingType
        //                : convertType;

        //            result.Sum = Convert.ChangeType(item.Sum ?? 0m, tProp);
        //            result.Min = Convert.ChangeType(item.Min ?? 0m, tProp);
        //            result.Max = Convert.ChangeType(item.Max ?? 0m, tProp);
        //            result.Average = Convert.ChangeType(item.Average ?? 0m, tProp);
        //            result.Count = Convert.ChangeType(item.Count ?? 0m, tProp);
        //        }
        //    }

        //    return result;
        //}
    }
}