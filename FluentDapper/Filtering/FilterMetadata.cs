using System;
using System.Linq.Expressions;
using FluentDapper.Data.Predicates;

namespace FluentDapper.Filtering
{
    public class FilterMetadata
    {
    }

    public class FilterMetadata<TFilter, TData> : FilterMetadata where TFilter : IDataFilter<TFilter, TData>
    {
        public Expression<Func<TData, object>> FilterExpression { get; set; }

        public Operator FilterType { get; set; }

        public Func<TFilter, object> FilterValue { get; set; }
        public object DefaultValue { get; set; }
    }
}