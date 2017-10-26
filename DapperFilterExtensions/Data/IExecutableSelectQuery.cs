using System.Collections.Generic;
using System.Data;
using DapperFilterExtensions.Filtering;

namespace DapperFilterExtensions.Data
{
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