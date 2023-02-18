namespace Infrastructure.UI.KendoGridHelper
{
    /// <summary>
    /// Оберта для предиката сравнения при фильтрации.
    /// </summary>
    public static class PredicateWrapper
    {
        /// <summary>
        /// Сравнивает значение свойства объекта с этелонным значением.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="item">Объект.</param>
        /// <param name="key">Свойство.</param>
        /// <param name="value">Значение.</param>
        /// <returns></returns>
        public static bool Invoke<T>(T item, string key, object value)
        {
            var property = typeof(T).GetProperty(key);
            if (property == null)
            {
                return false;
            }

            return Equals(property.GetValue(item), value);
        }
    }
}