using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using DapperFilterExtensions.Data.Predicates;
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
        private readonly IPredicateQueryBuilderFactory _predicateQueryBuilderFactory;

        //private readonly Expression<Func<TData, object>>[] _propertyExpressions;
        private readonly Dictionary<Type, List<Property>> _properties = new Dictionary<Type, List<Property>>();
        private readonly List<Type> _joins = new List<Type>();

        private string _query;
        private bool _compiled;

        /// <inheritdoc />
        public ISqlDialect Dialect { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of the <see cref="T:DapperFilterExtensions.Data.SelectQueryBuilder`2" /> class.
        /// </summary>
        /// <param name="classMapperFactory"></param>
        /// <param name="predicateFactory"></param>
        /// <param name="predicateQueryBuilderFactory"></param>
        /// <param name="propertyExpressions">The field set to return; or all propertyExpressions if <c>null</c> is provided.</param>
        /// <inheritdoc />
        public SelectQueryBuilder(
            IClassMapperFactory classMapperFactory,
            IPredicateFactory predicateFactory,
            IPredicateQueryBuilderFactory predicateQueryBuilderFactory,
            params Expression<Func<TData, object>>[] propertyExpressions)
            : this(classMapperFactory, predicateFactory, predicateQueryBuilderFactory, new SqlServerDialect(), propertyExpressions)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="SelectQueryBuilder{TData,TViewData}"/> class.
        /// </summary>
        /// <param name="classMapperFactory"></param>
        /// <param name="predicateFactory"></param>
        /// <param name="predicateQueryBuilderFactory"></param>
        /// <param name="sqlDialect">The SQL dialect to use.</param>
        /// <param name="propertyExpressions">The field set to return; or all propertyExpressions if <c>null</c> is provided.</param>
        /// <inheritdoc />
        public SelectQueryBuilder(
            IClassMapperFactory classMapperFactory,
            IPredicateFactory predicateFactory,
            IPredicateQueryBuilderFactory predicateQueryBuilderFactory,
            ISqlDialect sqlDialect,
            params Expression<Func<TData, object>>[] propertyExpressions)
        {
            _classMapperFactory = classMapperFactory;
            _predicateFactory = predicateFactory;
            _predicateQueryBuilderFactory = predicateQueryBuilderFactory;

            Dialect = sqlDialect;

            //_propertyExpressions = propertyExpressions;
            _properties[typeof(TData)] = GetProperties(propertyExpressions);
        }

        #endregion

        // Public methods
        #region Join

        /// <inheritdoc />
        public ISelectQueryBuilder<TData, TViewData> Join<TJoinedData>(params Expression<Func<TJoinedData, object>>[] fields)
        {
            var joinedType = typeof(TJoinedData);

            _joins.Add(joinedType);
            _properties.Add(joinedType, GetProperties(fields, joinedType.Name));

            return this;
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
            _query = $"SELECT {GetColumnNames()} FROM {GetTableName()} {CompileJoins()} {CompileSorts()}".Trim();
            _compiled = true;

            return this;
        }

        #endregion
        #region Execute

        /// <inheritdoc />
        public IEnumerable<TViewData> Execute(IDbConnection connection)
        {
            if (!_compiled)
                Compile();

            var transaction = default(IDbTransaction);
            var buffered = true;
            int? commandTimeout = null;

            return connection.Query<TViewData>(_query, null, transaction, buffered, commandTimeout, CommandType.Text);
        }

        /// <inheritdoc />
        public IEnumerable<TViewData> Execute<TDataFilter>(IDataFilter<TDataFilter, TData> filter, IDbConnection connection) where TDataFilter : IDataFilter<TDataFilter, TData>
        {
            if (!_compiled)
                Compile();

            var parameters = new Dictionary<string, object>();
            var transaction = default(IDbTransaction);
            var buffered = true;
            int? commandTimeout = null;

            var filterPredicate = _predicateFactory.GetPredicate<TDataFilter, TData>(filter);

            var filterSql = _predicateQueryBuilderFactory
                .GetQueryBuilder(filterPredicate, this)
                .GetSql(filterPredicate, parameters);

            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
                dynamicParameters.Add(parameter.Key, parameter.Value);

            return connection.Query<TViewData>($"{_query} WHERE {filterSql}", dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text);
        }

        #endregion
        #region GetQuery

        /// <inheritdoc />
        public IQuery GetQuery()
        {
            if (!_compiled)
                Compile();

            return new Query(_query);
        }

        /// <inheritdoc />
        public IQuery GetQuery<TDataFilter>(IDataFilter<TDataFilter, TData> filter) where TDataFilter : IDataFilter<TDataFilter, TData>
        {
            if (!_compiled)
                Compile();

            var parameters = new Dictionary<string, object>();

            var filterPredicate = _predicateFactory.GetPredicate<TDataFilter, TData>(filter);

            var filterSql = _predicateQueryBuilderFactory
                .GetQueryBuilder(filterPredicate, this)
                .GetSql(filterPredicate, parameters);

            return new Query($"{_query} WHERE {filterSql}", parameters);
        }

        #endregion

        // Underlying interface implementations
        #region ISqlBuilder

        #region IdentitySql

        /// <inheritdoc />
        public string IdentitySql(IClassMapper classMapper)
        {
            return Dialect.GetIdentitySql(GetTableName(classMapper));
        }

        #endregion
        #region GetTableName

        /// <inheritdoc />
        public string GetTableName(IClassMapper classMapper)
        {
            return Dialect.GetTableName(classMapper.SchemaName, classMapper.TableName, null);
        }

        #endregion
        #region GetColumnName

        /// <inheritdoc />
        public string GetColumnName(Type type, string propertyName, bool includeAlias)
        {
            return GetColumnName(type, propertyName, includeAlias, null);
        }

        /// <inheritdoc />
        public string GetColumnName(Type type, string propertyName, bool includeAlias, string alias)
        {
            var classMapper = _classMapperFactory.Get(type);
            return GetColumnName(classMapper, propertyName, includeAlias, alias);
        }

        /// <inheritdoc />
        public string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias)
        {
            return GetColumnName(classMapper, propertyName, includeAlias, null);
        }

        /// <inheritdoc />
        public string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias, string alias)
        {
            var propertyMap =
                classMapper.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)) ??
                throw new ArgumentException($"Could not find '{propertyName}' in class mapper '{classMapper.GetType().Name}'.");

            return GetColumnName(classMapper, propertyMap, includeAlias);
        }

        /// <inheritdoc />
        public string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias)
        {
            return GetColumnName(classMapper, property, includeAlias, null);
        }

        /// <inheritdoc />
        public string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias, string alias)
        {
            if (!includeAlias)
                alias = null;
            else if (string.IsNullOrEmpty(alias) && property.ColumnName != property.Name)
                alias = property.Name;

            return Dialect.GetColumnName(GetTableName(classMapper), property.ColumnName, alias);
        }

        #endregion

        #endregion

        // Private methods
        #region CompileJoins

        private string CompileJoins()
        {
            if (_joins.Count == 0)
                return string.Empty;

            var joinSqlStatements = new List<string>();

            foreach (var join in _joins)
            {
                var sourceClassMapper = _classMapperFactory.Get<TData>();
                var targetClassMapper = _classMapperFactory.Get(join);

                var target = GetTableName(targetClassMapper);

                joinSqlStatements.Add($"INNER JOIN {target} ON {GetForeignKeyColumnName(sourceClassMapper, targetClassMapper)} = {GetColumnName(targetClassMapper, "Id", false)}");
            }

            return string.Join(" ", joinSqlStatements);
        }

        #endregion
        #region CompileJoins

        private string CompileSorts()
        {
            // TODO Implement.
            return string.Empty;
        }

        #endregion
        #region GetTableName

        private string GetTableName()
        {
            var classMapper = _classMapperFactory.Get<TData>();
            return GetTableName(classMapper);
        }

        #endregion
        #region GetColumnNames

        private string GetColumnNames()
        {

            var columns = new List<string>();

            foreach (var typeWithProperties in _properties)
            {
                var classMapper = _classMapperFactory.Get(typeWithProperties.Key);
                foreach (var property in typeWithProperties.Value)
                {
                    var propertyMap = classMapper.Properties.SingleOrDefault(p => p.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                        ?? throw new ArgumentException($"Could not find '{property.Name}' in class mapper '{classMapper.GetType().Name}'.");

                    columns.Add(GetColumnName(classMapper, propertyMap, true, property.Alias));
                }
            }

            return string.Join(", ", columns);
        }

        #endregion
        #region GetForeignKeyColumnName

        private string GetForeignKeyColumnName(IClassMapper sourceClassMapper, IClassMapper targetClassMapper)
        {
            var sourcePropertyName = $"{targetClassMapper.EntityType.Name}Id";
            var sourceColumnName = GetColumnName(sourceClassMapper, sourcePropertyName, false);
            return sourceColumnName;
        }

        #endregion

        #region GetProperties

        /// <summary>
        /// Gets property names to be used for selection.
        /// </summary>
        /// <returns>Specified property names, or all <typeparamref name="TData"/> properties if none specified.</returns>
        private static List<Property> GetProperties<TEntityType>(IReadOnlyCollection<Expression<Func<TEntityType, object>>> propertyExpressions, string prefix = null)
        {
            var propertyNames = new List<Property>();

            string GetAlias(string propertyNamePrefix, string propertyName)
            {
                return string.IsNullOrEmpty(propertyNamePrefix) || string.IsNullOrEmpty(propertyName)
                    ? null
                    : $"{propertyNamePrefix}{propertyName}";
            }

            if (propertyExpressions != null && propertyExpressions.Count != 0)
            {
                // Return specified property names only.
                propertyNames.AddRange(propertyExpressions.Select(p =>
                {
                    var propertyName = GetPropertyNameFromExpression(p);
                    return new Property
                    {
                        Name = propertyName,
                        Alias = GetAlias(prefix, propertyName)
                    };
                }));
            }
            else
            {
                // Return all public writable properties from TEntityType.
                propertyNames.AddRange(
                    typeof(TEntityType)
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(pi => pi.CanWrite)
                        .Select(propertyInfo => new Property
                        {
                            Name = propertyInfo.Name,
                            Alias = GetAlias(prefix, propertyInfo.Name)
                        }));
            }

            return propertyNames;
        }

        #endregion
        #region GetPropertyNameFromExpression

        private static string GetPropertyNameFromExpression<TEntityType>(Expression<Func<TEntityType, object>> expression)
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