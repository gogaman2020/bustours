using System;
using System.Collections.Generic;

namespace Infrastructure.Db.Repositories
{
    public delegate bool Predicate<in T, in TFilter>(T obj, TFilter filter);

    public class PredicateBuilder<T, TFilter>
    {
        public enum PredicateOp
        {
            And,
            Or
        }

        public class Item
        {
            public Predicate<TFilter> IsUse { get; set; }
            public PredicateOp Operator { get; set; }
            public Predicate<T, TFilter> FilterPredicate { get; set; }
        }

        private List<Item> _list = new List<Item>();

        public static implicit operator PredicateBuilder<T, TFilter>(Predicate<T, TFilter> predicate)
        {
            var builder = new PredicateBuilder<T, TFilter>();
            builder._list.Add(new Item {Operator = PredicateOp.And, FilterPredicate = predicate, IsUse = t => true});
            return builder;
        }

        public void Add(Item item)
        {
            _list.Add(item);
        }

        public Predicate<T, TFilter> Build(TFilter filter)
        {
            if (_list.Count == 0)
            {
                return (t, f) => true;
            }

            Predicate<T, TFilter> predicate = null;
            foreach (var item in _list)
            {
                if (!item.IsUse(filter))
                {
                    continue;
                }

                if (predicate == null)
                {
                    predicate = item.FilterPredicate;
                    continue;
                }

                switch (item.Operator)
                {
                    case PredicateOp.And:
                        predicate = predicate.And(item.FilterPredicate);
                        break;
                    case PredicateOp.Or:
                        predicate = predicate.Or(item.FilterPredicate);
                        break;
                }
            }

            return predicate;
        }
    }

    public static class PredicateExtensions
    {
        public static Predicate<T, TFilter> And<T, TFilter>(this Predicate<T, TFilter> source,
            Predicate<T, TFilter> extension)
        {
            return (t, f) => source(t, f) && extension(t, f);
        }

        public static Predicate<T, TFilter> Or<T, TFilter>(this Predicate<T, TFilter> source,
            Predicate<T, TFilter> extension)
        {
            return (t, f) => source(t, f) || extension(t, f);
        }

        public static PredicateBuilder<T, TFilter> AndIf<T, TFilter>(this PredicateBuilder<T, TFilter> source,
            Predicate<TFilter> predicate,
            Predicate<T, TFilter> extension)
        {
            return WithIf(PredicateBuilder<T, TFilter>.PredicateOp.And, source, predicate, extension);
        }

        public static PredicateBuilder<T, TFilter> AndIf<T, TFilter, TValue>(this PredicateBuilder<T, TFilter> source,
            Func<TFilter, TValue> predicate,
            Predicate<T, TFilter> extension)
            where TValue : class
        {
            return WithIf(PredicateBuilder<T, TFilter>.PredicateOp.And, source, f => predicate(f) != null, extension);
        }

        public static PredicateBuilder<T, TFilter> AndIf<T, TFilter, TValue>(this PredicateBuilder<T, TFilter> source,
            Func<TFilter, TValue?> predicate,
            Predicate<T, TFilter> extension)
            where TValue : struct
        {
            return WithIf(PredicateBuilder<T, TFilter>.PredicateOp.And, source, f => predicate(f).HasValue, extension);
        }

        public static PredicateBuilder<T, TFilter> OrIf<T, TFilter>(this PredicateBuilder<T, TFilter> source,
            Predicate<TFilter> predicate,
            Predicate<T, TFilter> extension)
        {
            return WithIf(PredicateBuilder<T, TFilter>.PredicateOp.Or, source, predicate, extension);
        }

        public static PredicateBuilder<T, TFilter> OrIf<T, TFilter, TValue>(this PredicateBuilder<T, TFilter> source,
            Func<TFilter, TValue> predicate,
            Predicate<T, TFilter> extension)
            where TValue : class
        {
            return WithIf(PredicateBuilder<T, TFilter>.PredicateOp.Or, source, f => predicate(f) != null, extension);
        }

        public static PredicateBuilder<T, TFilter> OrIf<T, TFilter, TValue>(this PredicateBuilder<T, TFilter> source,
            Func<TFilter, TValue?> predicate,
            Predicate<T, TFilter> extension)
            where TValue : struct
        {
            return WithIf(PredicateBuilder<T, TFilter>.PredicateOp.Or, source, f => predicate(f).HasValue, extension);
        }

        private static PredicateBuilder<T, TFilter> WithIf<T, TFilter>(PredicateBuilder<T, TFilter>.PredicateOp op,
            PredicateBuilder<T, TFilter> source,
            Predicate<TFilter> predicate,
            Predicate<T, TFilter> extension)
        {
            var item = new PredicateBuilder<T, TFilter>.Item
            {
                Operator = op,
                FilterPredicate = extension,
                IsUse = predicate,
            };
            source.Add(item);
            return source;
        }
    }
}