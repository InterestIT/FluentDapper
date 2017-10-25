using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using DapperFilterExtensions.Filtering;

namespace DapperFilterExtensions.Data
{
    public interface ISqlBuilder
    {
        string IdentitySql(IClassMapper classMapper);
        string GetTableName(IClassMapper classMapper);
        string GetColumnName(Type type, string propertyName, bool includeAlias);
        string GetColumnName(Type type, string propertyName, bool includeAlias, string alias);
        string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias);
        string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias, string alias);
        string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias);
        string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias, string alias);
        ISqlDialect Dialect { get; }
    }

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

    public interface IQueryParameter
    {
        string Name { get; }
        object Value { get; }
    }

    public interface IQuery
    {
        string Text { get; }
        IList<IQueryParameter> Parameters { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewData"></typeparam>
    public interface IExecutableSelectQuery<TData, out TViewData> where TViewData : TData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IEnumerable<TViewData> Execute(IDbConnection connection);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="connection"></param>
        /// <typeparam name="TDataFilter"></typeparam>
        /// <returns></returns>
        IEnumerable<TViewData> Execute<TDataFilter>(IDataFilter<TDataFilter, TData> filter, IDbConnection connection) where TDataFilter : IDataFilter<TDataFilter, TData>;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IQuery GetQuery();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="TDataFilter"></typeparam>
        /// <returns></returns>
        IQuery GetQuery<TDataFilter>(IDataFilter<TDataFilter, TData> filter) where TDataFilter : IDataFilter<TDataFilter, TData>;

    }
}