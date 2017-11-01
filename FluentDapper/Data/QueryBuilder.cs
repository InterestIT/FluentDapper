using System;
using System.Linq.Expressions;
using FluentDapper.Data.Predicates;
using FluentDapper.Filtering;

namespace FluentDapper.Data
{
    /// <summary>
    /// Query builder.
    /// </summary>
    /// <inheritdoc />
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IClassMapperFactory _classMapperFactory;
        private readonly IPredicateFactory _predicateFactory;
        private readonly IPredicateQueryBuilderFactory _predicateQueryBuilderFactory;

        public QueryBuilder(IClassMapperFactory classMapperFactory, IPredicateFactory predicateFactory, IPredicateQueryBuilderFactory predicateQueryBuilderFactory)
        {
            _classMapperFactory = classMapperFactory;
            _predicateFactory = predicateFactory;
            _predicateQueryBuilderFactory = predicateQueryBuilderFactory;
        }

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TData> Select<TData>(params Expression<Func<TData, object>>[] fields) where TData : class
        {
            return new SelectQueryBuilder<TData, TData>(_classMapperFactory, _predicateFactory, _predicateQueryBuilderFactory, fields);
        }

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Select<TData, TViewData>(params Expression<Func<TData, object>>[] fields) where TData : class where TViewData : TData
        {
            return new SelectQueryBuilder<TData, TViewData>(_classMapperFactory, _predicateFactory, _predicateQueryBuilderFactory, fields);
        }
    }
}