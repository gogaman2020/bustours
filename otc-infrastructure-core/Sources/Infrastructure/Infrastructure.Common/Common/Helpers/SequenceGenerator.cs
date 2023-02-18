using System;
using System.Collections.Generic;

namespace Infrastructure.Common.Helpers
{
    public class SequenceGenerator
    {
        private Dictionary<Type, int> _indexes = new Dictionary<Type, int>();

        public int Next<T>()
        {
            if (!_indexes.ContainsKey(typeof(T)))
            {
                _indexes[typeof(T)] = 0;
            }

            return ++_indexes[typeof(T)];
        }
    }
}
