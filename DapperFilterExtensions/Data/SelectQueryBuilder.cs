using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DapperExtensions.Sql;
using DapperFilterExtensions.Filtering;

namespace DapperFilterExtensions.Data
{
    /// <summary>
    /// Select query builder.
    /// </summary>
    /// <typeparam name="TData">The data type to build a select query for.</typeparam>
    /// <typeparam name="TViewData">The view data type to return when executing.</typeparam>
    /// <inheritdoc />
    public class SelectQueryBuilder<TData, TViewData> : ISelectQueryBuilder<TData, TViewData> where TData : class where TViewData : TData
    {
        #region Variables

        private readonly IClassMapperFactory _classMapperFactory;
        private readonly IPredicateFactory _predicateFactory;

        private readonly Expression<Func<TData, object>>[] _propertyExpressions;
        private List<string> _propertyNames;

        private string _query;
        private bool _compiled;
        private readonly ISqlDialect _sqlDialect;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of the <see cref="SelectQueryBuilder{TData,TViewData}"/> class.
        /// </summary>
        /// <param name="classMapperFactory"></param>
        /// <param name="predicateFactory"></param>
        /// <param name="propertyExpressions">The field set to return; or all propertyExpressions if <c>null</c> is provided.</param>
        public SelectQueryBuilder(IClassMapperFactory classMapperFactory, IPredicateFactory predicateFactory, params Expression<Func<TData, object>>[] propertyExpressions)
        {
            _classMapperFactory = classMapperFactory;
            _predicateFactory = predicateFactory;
            _propertyExpressions = propertyExpressions;
            _sqlDialect = new SqlServerDialect();
        }

        /// <summary>
        /// Create a new instance of the <see cref="SelectQueryBuilder{TData,TViewData}"/> class.
        /// </summary>
        /// <param name="classMapperFactory"></param>
        /// <param name="predicateFactory"></param>
        /// <param name="sqlDialect">The SQL dialect to use.</param>
        /// <param name="propertyExpressions">The field set to return; or all propertyExpressions if <c>null</c> is provided.</param>
        public SelectQueryBuilder(IClassMapperFactory classMapperFactory, IPredicateFactory predicateFactory, ISqlDialect sqlDialect, params Expression<Func<TData, object>>[] propertyExpressions)
        {
            _classMapperFactory = classMapperFactory;
            _propertyExpressions = propertyExpressions;
            _sqlDialect = sqlDialect;
            _predicateFactory = predicateFactory;
        }

        #endregion

        // Public methods
        #region Join

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Join<TJoinedData>(params Expression<Func<TJoinedData, object>>[] fields)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Join<TJoinedData>(
            Expression<Func<TData, object>> sourceField,
            Expression<Func<TJoinedData, object>> destinationField,
            params Expression<Func<TJoinedData, object>>[] fields)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Sort

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Sort(params Expression<Func<TData, object>>[] fields)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Sort(SortDirection sortDirection, params Expression<Func<TData, object>>[] fields)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Compile

        /// <inheritdoc />
        public IExecutableSelectQuery<TData, TViewData> Compile()
        {
            _query = $"SELECT {GetColumnNames()} FROM {GetTableName()} {GetJoins()}".Trim();
            _compiled = true;

            return this;
        }

        #endregion
        #region Execute

        /// <inheritdoc />
        public IEnumerable<TViewData> Execute(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<TViewData> Execute<TDataFilter>(IDataFilter<TDataFilter, TData> filter, IDbConnection connection) where TDataFilter : IDataFilter<TDataFilter, TData>
        {
            var filterPredicate = _predicateFactory.GetPredicate<TDataFilter, TData>(filter);

            var parameters = new Dictionary<string, object>();

            var filterSql = filterPredicate.GetSql(null, parameters);

            throw new NotImplementedException();
        }

        #endregion

        // Private methods
        private string GetJoins()
        {
            return string.Empty; // TODO Implement!
        }

        private string GetTableName()
        {
            var map = _classMapperFactory.Get<TData>();
            return _sqlDialect.GetTableName(map.SchemaName, map.TableName, null);
        }

        #region GetColumnNames

        private string GetColumnNames()
        {
            var map = _classMapperFactory.Get<TData>();

            var columns = new List<string>();

            foreach (var propertyName in GetPropertyNames())
            {
                var propertyMap =
                    map.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)) ??
                    throw new ArgumentException($"Could not find '{propertyName}' in class mapper '{map.GetType().Name}'.");

                columns.Add(_sqlDialect.GetColumnName(map.TableName, propertyMap.ColumnName, null));
            }

            return string.Join(", ", columns);
        }

        #endregion
        #region GetPropertyNames

        /// <summary>
        /// Gets property names to be used for selection.
        /// </summary>
        /// <returns>Specified property names, or all <typeparamref name="TData"/> properties if none specified.</returns>
        private IEnumerable<string> GetPropertyNames()
        {
            if (_propertyNames != null && _propertyNames.Count > 0)
                return _propertyNames;

            var propertyNames = new List<string>();

            if (_propertyExpressions != null && _propertyExpressions.Length != 0)
            {
                // Return specified property names.
                propertyNames.AddRange(_propertyExpressions.Select(GetPropertyNameFromExpression));
            }
            else
            {
                // Return all public writable properties from TData.
                propertyNames.AddRange(
                    typeof(TData)
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(pi => pi.CanWrite)
                        .Select(propertyInfo => propertyInfo.Name));
            }

            _propertyNames = propertyNames;
            return propertyNames;
        }

        #endregion
        #region GetPropertyNameFromExpression

        private static string GetPropertyNameFromExpression(Expression<Func<TData, object>> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression memberExpression:
                    return memberExpression.Member.Name;
                case UnaryExpression unaryExpression:
                    return GetPropertyNameFromUnaryExpression(unaryExpression);
                default:
                    throw new NotSupportedException($"Expression type \'{expression.Body.GetType().Name}\' is not implemented.");
            }
        }

        #endregion
        #region GetPropertyNameFromUnaryExpression

        private static string GetPropertyNameFromUnaryExpression(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            //var methodCallExpression = unaryExpression.Operand as MethodCallExpression;
            //if (methodCallExpression != null)
            //{
            //    // e.g. for handling: x => (string)x.CustomProperties["zoeknaam"]
            //    return ((MemberExpression)methodCallExpression.Object).Member;
            //}

            throw new NotSupportedException($"Unary expression type \'{unaryExpression.Operand.GetType().Name}\' is not implemented.");
        }

        #endregion
    }
}