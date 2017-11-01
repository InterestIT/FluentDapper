using System;
using System.Linq.Expressions;

namespace FluentDapper.Data
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewData"></typeparam>
    /// <inheritdoc />
    public interface ISelectQueryBuilder<TData, out TViewData> : IExecutableSelectQuery<TData, TViewData>, ISqlBuilder where TViewData : TData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <typeparam name="TJoinedData"></typeparam>
        /// <returns></returns>
        ISelectQueryBuilder<TData, TViewData> Join<TJoinedData>(params Expression<Func<TJoinedData, object>>[] fields);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceField"></param>
        /// <param name="destinationField"></param>
        /// <param name="fields"></param>
        /// <typeparam name="TJoinedData"></typeparam>
        /// <returns></returns>
        ISelectQueryBuilder<TData, TViewData> Join<TJoinedData>(Expression<Func<TData, object>> sourceField, Expression<Func<TJoinedData, object>> destinationField, params Expression<Func<TJoinedData, object>>[] fields);

        //ISelectQueryBuilder Filter<TDataFilter, TData>() where TDataFilter : IDataFilter<TDataFilter, TData>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        ISelectQueryBuilder<TData, TViewData> Sort(params Expression<Func<TData, object>>[] fields);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortDirection"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        ISelectQueryBuilder<TData, TViewData> Sort(SortDirection sortDirection, params Expression<Func<TData, object>>[] fields);

        /// <summary>
        /// Compiles the select query to be fast when reusing.
        /// </summary>
        /// <returns>An <see cref="IExecutableSelectQuery{TData,TViewData}"/> instance.</returns>
        IExecutableSelectQuery<TData, TViewData> Compile();
    }
}