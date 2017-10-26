using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using DapperFilterExtensions.Data;
using DapperFilterExtensions.Data.Predicates;
using DapperFilterExtensions.Filtering;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DapperFilterExtensions.Tests.Data
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SelectQueryBuilderTest
    {
        #region Variables

        private Mock<IClassMapperFactory> _classMapperFactoryMock;
        private Mock<IPredicateFactory> _predicateFactoryMock;
        private Mock<IPredicateQueryBuilderFactory> _predicateQueryBuilderFactoryMock;

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void TestInitialize()
        {
            _classMapperFactoryMock = new Mock<IClassMapperFactory>(MockBehavior.Strict);
            _predicateFactoryMock = new Mock<IPredicateFactory>(MockBehavior.Strict);
            _predicateQueryBuilderFactoryMock = new Mock<IPredicateQueryBuilderFactory>(MockBehavior.Strict);
        }

        #endregion

        // Constructor
        #region ConstructorShouldSetDefaultDialectAndAllPropertiesIfNoneProvided

        [TestMethod]
        public void ConstructorShouldSetDefaultDialectAndAllPropertiesIfNoneProvided()
        {
            // Arrange
            var expectedProperties = new Dictionary<Type, List<Property>>
            {
                {
                    typeof(Article), new List<Property>
                    {
                        new Property{Name=nameof(Article.Id)},
                        new Property{Name=nameof(Article.ArticleTypeId)},
                        new Property{Name=nameof(Article.Name)}
                    }
                }
            };

            // Act
            var queryBuilder = new SelectQueryBuilder<Article, Article>(
                _classMapperFactoryMock.Object,
                _predicateFactoryMock.Object,
                _predicateQueryBuilderFactoryMock.Object);

            // Assert
            queryBuilder.Dialect.Should().NotBeNull().And.BeOfType<SqlServerDialect>();
            GetPrivate<Dictionary<Type, List<Property>>>(queryBuilder, "_properties").ShouldBeEquivalentTo(expectedProperties);
        }

        #endregion
        #region ConstructorShouldSetDefaultDialectAndProvidedPropertiesIfProvided

        [TestMethod]
        public void ConstructorShouldSetDefaultDialectAndProvidedPropertiesIfProvided()
        {
            // Arrange
            var expectedProperties = new Dictionary<Type, List<Property>>
            {
                {
                    typeof(Article), new List<Property>
                    {
                        new Property{Name=nameof(Article.Name)}
                    }
                }
            };

            // Act
            var queryBuilder = new SelectQueryBuilder<Article, Article>(
                _classMapperFactoryMock.Object,
                _predicateFactoryMock.Object,
                _predicateQueryBuilderFactoryMock.Object,
                a => a.Name);

            // Assert
            queryBuilder.Dialect.Should().NotBeNull().And.BeOfType<SqlServerDialect>();
            GetPrivate<Dictionary<Type, List<Property>>>(queryBuilder, "_properties").ShouldBeEquivalentTo(expectedProperties);
        }

        #endregion

        // GetColumnName
        #region GetColumnNameShouldGetColumnNameForTypeAndPropertyName

        [TestMethod]
        public void GetColumnNameShouldGetColumnNameForTypeAndPropertyName()
        {
            // Arrange
            const string articleTableName = "Articles";

            var articleClassMapperMock = GetClassMapperMock<Article>(articleTableName, nameof(Article.Name));

            _classMapperFactoryMock.Setup(f => f.Get(typeof(Article))).Returns(articleClassMapperMock.Object);

            var queryBuilder = new SelectQueryBuilder<Article, Article>(
                _classMapperFactoryMock.Object,
                _predicateFactoryMock.Object,
                _predicateQueryBuilderFactoryMock.Object);

            // Act
            var columnName = queryBuilder.GetColumnName(typeof(Article), nameof(Article.Name), false);

            // Assert
            columnName.Should().Be($"[{articleTableName}].[{nameof(Article.Name)}]");

            _classMapperFactoryMock.Verify(f => f.Get(typeof(Article)), Times.Once);
        }

        #endregion

        // Compile
        #region CompileShouldCompileSelect

        [TestMethod]
        public void CompileShouldCompileSelect()
        {
            // Arrange
            const string expectedQuery = "SELECT [Articles].[Id], [Articles].[ArticleTypeId], [Articles].[Name] FROM [Articles]";

            var articleClassMapper = new ArticleClassMapper();
            _classMapperFactoryMock.Setup(f => f.Get<Article>()).Returns(articleClassMapper);
            _classMapperFactoryMock.Setup(f => f.Get(typeof(Article))).Returns(articleClassMapper);

            var queryBuilder = new SelectQueryBuilder<Article, Article>(_classMapperFactoryMock.Object, _predicateFactoryMock.Object, _predicateQueryBuilderFactoryMock.Object);

            // Act
            var query = queryBuilder.Compile();

            // Assert
            GetPrivate<bool>(query, "_compiled").Should().BeTrue();
            GetPrivate<string>(query, "_query").Should().Be(expectedQuery);

            _classMapperFactoryMock.Verify(f => f.Get<Article>(), Times.Once);
            _classMapperFactoryMock.Verify(f => f.Get(typeof(Article)), Times.Once);
        }

        #endregion
        #region CompileShouldSelectWithColumnSpecification

        [TestMethod]
        public void CompileShouldSelectWithColumnSpecification()
        {
            // Arrange
            Expression<Func<Article, object>>[] fields = { a => a.Id };
            const string expectedQuery = "SELECT [Articles].[Id] FROM [Articles]";

            var articleClassMapper = new ArticleClassMapper();
            _classMapperFactoryMock.Setup(f => f.Get<Article>()).Returns(articleClassMapper);
            _classMapperFactoryMock.Setup(f => f.Get(typeof(Article))).Returns(articleClassMapper);

            var queryBuilder = new SelectQueryBuilder<Article, Article>(_classMapperFactoryMock.Object, _predicateFactoryMock.Object, _predicateQueryBuilderFactoryMock.Object, fields);

            // Act
            var query = queryBuilder.Compile();

            // Assert
            GetPrivate<bool>(query, "_compiled").Should().BeTrue();
            GetPrivate<string>(query, "_query").Should().Be(expectedQuery);

            _classMapperFactoryMock.Verify(f => f.Get<Article>(), Times.Once);
            _classMapperFactoryMock.Verify(f => f.Get(typeof(Article)), Times.Once);
        }

        #endregion
        #region CompileShouldSelectWithColumnSpecificationAndJoins

        [TestMethod]
        public void CompileShouldSelectWithColumnSpecificationAndJoins()
        {
            // Arrange
            Expression<Func<Article, object>>[] fields = { a => a.Id };
            const string expectedQuery = "SELECT [Articles].[Id], [ArticleTypes].[Name] AS [ArticleTypeName] FROM [Articles] INNER JOIN [ArticleTypes] ON [Articles].[ArticleTypeId] = [ArticleTypes].[Id]";

            var articleClassMapper = new ArticleClassMapper();
            var articleTypeClassMapper = new ArticleTypeClassMapper();
            _classMapperFactoryMock.Setup(f => f.Get<Article>()).Returns(articleClassMapper);
            _classMapperFactoryMock.Setup(f => f.Get(typeof(Article))).Returns(articleClassMapper);
            _classMapperFactoryMock.Setup(f => f.Get(typeof(ArticleType))).Returns(articleTypeClassMapper);

            var queryBuilder = new SelectQueryBuilder<Article, Article>(_classMapperFactoryMock.Object, _predicateFactoryMock.Object, _predicateQueryBuilderFactoryMock.Object, fields);
            queryBuilder.Join<ArticleType>(at => at.Name);

            // Act
            var query = queryBuilder.Compile();

            // Assert
            GetPrivate<bool>(query, "_compiled").Should().BeTrue();
            GetPrivate<string>(query, "_query").Should().Be(expectedQuery);

            _classMapperFactoryMock.Verify(f => f.Get<Article>(), Times.Exactly(2));
            _classMapperFactoryMock.Verify(f => f.Get(typeof(Article)), Times.Once);
            _classMapperFactoryMock.Verify(f => f.Get(typeof(ArticleType)), Times.Exactly(2));
        }

        #endregion

        // Private methods
        #region GetClassMapperMock

        private static Mock<IClassMapper<TData>> GetClassMapperMock<TData>(string articleTableName, params string[] properties) where TData : class
        {
            IList<IPropertyMap> propertyMaps = new List<IPropertyMap>();
            foreach (var property in properties)
                propertyMaps.Add(new PropertyMap(typeof(Article).GetProperty(property)));

            var classMapperMock = new Mock<IClassMapper<TData>>(MockBehavior.Strict);
            classMapperMock.SetupGet(m => m.SchemaName).Returns(default(string));
            classMapperMock.SetupGet(m => m.TableName).Returns(articleTableName);
            classMapperMock.SetupGet(m => m.Properties).Returns(propertyMaps);
            return classMapperMock;
        }

        #endregion

        #region GetPrivate

        private static T GetPrivate<T>(object instance, string fieldName)
        {
            var fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)(fieldInfo?.GetValue(instance) ?? default(T));
        }

        #endregion
    }
}