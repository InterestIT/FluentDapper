using System;
using System.Linq.Expressions;

namespace FluentDapper.Data
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
}