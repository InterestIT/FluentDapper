using System.Collections.Generic;
using System.Linq;

namespace FluentDapper.Data.Predicates
{
    /// <inheritdoc />
    public class PredicateGroupQueryBuilder : IPredicateQueryBuilder
    {
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IPredicateQueryBuilderFactory _predicateQueryBuilderFactory;

        /// <summary>Creates a new predicate query builder instance.</summary>
        public PredicateGroupQueryBuilder(
            ISqlBuilder sqlBuilder,
            IPredicateQueryBuilderFactory predicateQueryBuilderFactory)
        {
            _sqlBuilder = sqlBuilder;
            _predicateQueryBuilderFactory = predicateQueryBuilderFactory;
        }

        /// <inheritdoc />
        public string GetSql(IPredicate predicate, IDictionary<string, object> parameters)
        {
            return GetSql((IPredicateGroup)predicate, parameters);
        }

        private string GetSql(IPredicateGroup predicateGroup, IDictionary<string, object> parameters)
        {
            var separator = predicateGroup.Operator == GroupOperator.And ? " AND " : " OR ";

            var predicateSqlStatements = predicateGroup.Predicates
                .Select(predicate => _predicateQueryBuilderFactory.GetQueryBuilder(predicate, _sqlBuilder).GetSql(predicate, parameters))
                .Where(sql => !string.IsNullOrEmpty(sql))
                .ToArray();

            return predicateSqlStatements.Length == 0
                ? _sqlBuilder.Dialect.EmptyExpression
                : $"({string.Join(separator, predicateSqlStatements)})";
        }
    }
}