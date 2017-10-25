using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DapperFilterExtensions.Data.Predicates;

namespace DapperFilterExtensions.Filtering
{
    public class PredicateFactory : IPredicateFactory
    {
        private readonly Dictionary<Type, IList<FilterMetadata>> _metadataByFilter = new Dictionary<Type, IList<FilterMetadata>>();

        public PredicateFactory(IEnumerable<IFilterMetadataProvider> metadataProviders)
        {
            foreach (var metadataProvider in metadataProviders)
            {
                if (metadataProvider.Metadata == null || metadataProvider.Metadata.Count == 0)
                    continue;

                if (_metadataByFilter.ContainsKey(metadataProvider.Type))
                {
                    foreach (var filterMetadata in metadataProvider.Metadata)
                    {
                        _metadataByFilter[metadataProvider.Type].Add(filterMetadata);
                    }
                }
                else
                    _metadataByFilter.Add(metadataProvider.Type, metadataProvider.Metadata);
            }
        }

        public IPredicate GetPredicate<TFilter, TData>(IDataFilter<TFilter, TData> filter) where TFilter : IDataFilter<TFilter, TData> where TData : class
        {
            if (filter == null)
                return null;

            if (!_metadataByFilter.TryGetValue(typeof(TFilter), out var metadataForFilter))
                return null;

            var predicatesGroup = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>()
            };

            foreach (var untypedMetadata in metadataForFilter)
            {
                var metadata = (FilterMetadata<TFilter, TData>)untypedMetadata;

                var filterValue = metadata.FilterValue?.Invoke((TFilter)filter);
                if (filterValue == null || filterValue == metadata.DefaultValue)
                    continue;

                var fieldPredicate = GetFieldPredicate(metadata.FilterExpression, metadata.FilterType, filterValue, false);
                predicatesGroup.Predicates.Add(fieldPredicate);
            }

            return predicatesGroup.Predicates.Count > 0
                ? predicatesGroup
                : null;
        }

        private static IPredicate GetFieldPredicate<T>(Expression<Func<T, object>> filterExpression, Operator op, object value, bool negate) where T: class
        {
            var memberInfo = ReflectionHelper.GetProperty(filterExpression);
            return new FieldPredicate<T>
            {
                EntityType = typeof(T),
                PropertyName = memberInfo.Name,
                Operator = op,
                Value = value,
                Negate = negate
            };
        }
    }
}