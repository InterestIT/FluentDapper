using System;
using System.Linq.Expressions;
using DapperFilterExtensions.Filtering;

namespace DapperFilterExtensions.Data
{
    /// <summary>
    /// Query builder.
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>
        /// Creates a select query.
        /// </summary>
        /// <param name="fields">Fields to return, or all fields if <c>null</c> is provided.</param>
        /// <typeparam name="TData">The data type to create a select query for.</typeparam>
        /// <returns>An <see cref="ISelectQueryBuilder"/> instance.</returns>
        ISelectQueryBuilder<TData, TData> Select<TData>(params Expression<Func<TData, object>>[] fields) where TData : class;

        /// <summary>
        /// Creates a select query.
        /// </summary>
        /// <param name="fields">Fields to return, or all fields if <c>null</c> is provided.</param>
        /// <typeparam name="TData">The data type to create a select query for.</typeparam>
        /// <typeparam name="TViewData">The data type to return.</typeparam>
        /// <returns>An <see cref="ISelectQueryBuilder"/> instance.</returns>
        ISelectQueryBuilder<TData, TViewData> Select<TData, TViewData>(params Expression<Func<TData, object>>[] fields) where TData : class where TViewData : TData;
    }

    /// <summary>
    /// Query builder.
    /// </summary>
    /// <inheritdoc />
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IClassMapperFactory _classMapperFactory;
        private readonly IPredicateFactory _predicateFactory;

        public QueryBuilder(IClassMapperFactory classMapperFactory, IPredicateFactory predicateFactory)
        {
            _classMapperFactory = classMapperFactory;
            _predicateFactory = predicateFactory;
        }

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TData> Select<TData>(params Expression<Func<TData, object>>[] fields) where TData : class
        {
            return new SelectQueryBuilder<TData, TData>(_classMapperFactory, _predicateFactory, fields);
        }

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Select<TData, TViewData>(params Expression<Func<TData, object>>[] fields) where TData : class where TViewData : TData
        {
            return new SelectQueryBuilder<TData, TViewData>(_classMapperFactory, _predicateFactory, fields);
        }
    }
}