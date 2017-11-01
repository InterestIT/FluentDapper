using System;
using System.Collections.Generic;
using DapperExtensions;
using DapperExtensions.Sql;

namespace FluentDapper.Data.Predicates
{
    /// <inheritdoc />
    public class ExistsPredicateQueryBuilder : IPredicateQueryBuilder
    {
        private readonly ISqlDialect _sqlDialect;
        private readonly IPredicateQueryBuilderFactory _predicateQueryBuilderFactory;

        /// <summary>Creates a new predicate query builder instance.</summary>
        public ExistsPredicateQueryBuilder(
            ISqlDialect sqlDialect,
            IPredicateQueryBuilderFactory predicateQueryBuilderFactory)
        {
            _sqlDialect = sqlDialect;
            _predicateQueryBuilderFactory = predicateQueryBuilderFactory;
        }

        /// <inheritdoc />
        public string GetSql(IPredicate predicate, IDictionary<string, object> parameters)
        {
            return GetSql((IExistsPredicate)predicate, parameters);
        }

        private string GetSql(IExistsPredicate predicate, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}