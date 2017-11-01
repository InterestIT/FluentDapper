using System;
using System.Collections.Generic;

namespace FluentDapper.Data.Predicates
{
    /// <summary>
    ///
    /// </summary>
    public interface IPredicateQueryBuilderFactory
    {
        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="predicate"></param>
        /// <param name="sqlBuilder"></param>
        ///  <returns></returns>
        IPredicateQueryBuilder GetQueryBuilder(IPredicate predicate, ISqlBuilder sqlBuilder);
    }

    /// <inheritdoc />
    public class PredicateQueryBuilderFactory : IPredicateQueryBuilderFactory
    {
        private readonly Dictionary<string, IPredicateQueryBuilder> _predicateQueryBuilders;

        /// <summary>
        /// Creates a new predicate query builder instance.
        /// </summary>
        public PredicateQueryBuilderFactory()
        {
            _predicateQueryBuilders = new Dictionary<string, IPredicateQueryBuilder>();
        }

        /// <inheritdoc />
        public IPredicateQueryBuilder GetQueryBuilder(IPredicate predicate, ISqlBuilder sqlBuilder)
        {
            var key = $"{predicate.GetType().FullName}{sqlBuilder.Dialect.GetType().FullName}";

            if (_predicateQueryBuilders.TryGetValue(key, out var predicateQueryBuilder))
                return predicateQueryBuilder;

            switch (predicate)
            {
                case IPredicateGroup _:
                    _predicateQueryBuilders[key] = predicateQueryBuilder = new PredicateGroupQueryBuilder(sqlBuilder, this);
                    return predicateQueryBuilder;
                case IFieldPredicate _:
                    _predicateQueryBuilders[key] = predicateQueryBuilder = new FieldPredicateQueryBuilder(sqlBuilder, this);
                    return predicateQueryBuilder;
                //case IExistsPredicate _:
                //    _predicateQueryBuilders[key] = predicateQueryBuilder = new ExistsPredicateQueryBuilder(sqlDialect, this);
                //    return predicateQueryBuilder;
                default:
                    throw new NotSupportedException($"Predicate type '{predicate.GetType()}' is not supported.");
            }
        }
    }
}