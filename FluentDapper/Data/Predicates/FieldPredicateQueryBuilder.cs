using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentDapper.Data.Predicates
{
    public class FieldPredicateQueryBuilder : ComparePredicateQueryBuilder, IPredicateQueryBuilder
    {
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IPredicateQueryBuilderFactory _predicateQueryBuilderFactory;

        /// <summary>Creates a new predicate query builder instance.</summary>
        public FieldPredicateQueryBuilder(
            ISqlBuilder sqlBuilder,
            IPredicateQueryBuilderFactory predicateQueryBuilderFactory)
        {
            _sqlBuilder = sqlBuilder;
            _predicateQueryBuilderFactory = predicateQueryBuilderFactory;
        }

        /// <inheritdoc />
        public string GetSql(IPredicate predicate, IDictionary<string, object> parameters)
        {
            return GetSql((IFieldPredicate)predicate, parameters);
        }

        private string GetSql(IFieldPredicate predicate, IDictionary<string, object> parameters)
        {
            var columnName = _sqlBuilder.GetColumnName(predicate.EntityType, predicate.PropertyName, false);

            if (predicate.Value == null)
                return $"({columnName} IS {(predicate.Negate ? "NOT " : string.Empty)}NULL)";

            if (predicate.Value is IEnumerable enumerable && !(enumerable is string))
            {
                if (predicate.Operator != Operator.Eq)
                {
                    throw new ArgumentException("Operator must be set to Eq for Enumerable types");
                }

                var @params = new List<string>();
                foreach (var value in enumerable)
                {
                    var valueParameterName = parameters.SetParameterName(predicate.PropertyName, value, _sqlBuilder.Dialect.ParameterPrefix);
                    @params.Add(valueParameterName);
                }

                var paramStrings = @params.Aggregate(new StringBuilder(), (sb, s) => sb.Append((sb.Length != 0 ? ", " : string.Empty) + s), sb => sb.ToString());
                return $"({columnName} {(predicate.Negate ? "NOT " : string.Empty)}IN ({paramStrings}))";
            }

            var parameterName = parameters.SetParameterName(predicate.PropertyName, predicate.Value, _sqlBuilder.Dialect.ParameterPrefix);
            return $"({columnName} {GetOperatorString(predicate)} {parameterName})";
        }
    }

    public class ComparePredicateQueryBuilder
    {
        public virtual string GetOperatorString(IComparePredicate predicate)
        {
            switch (predicate.Operator)
            {
                case Operator.Gt:
                    return predicate.Negate ? "<=" : ">";
                case Operator.Ge:
                    return predicate.Negate ? "<" : ">=";
                case Operator.Lt:
                    return predicate.Negate ? ">=" : "<";
                case Operator.Le:
                    return predicate.Negate ? ">" : "<=";
                case Operator.Like:
                    return predicate.Negate ? "NOT LIKE" : "LIKE";
                default:
                    return predicate.Negate ? "<>" : "=";
            }
        }
    }
}