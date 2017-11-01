using System.Collections.Generic;

namespace FluentDapper.Data.Predicates
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPredicateQueryBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters">The parameter name/value combinations to be used by Dapper when executing the query.</param>
        /// <returns></returns>
        string GetSql(IPredicate predicate, IDictionary<string, object> parameters);
    }
}