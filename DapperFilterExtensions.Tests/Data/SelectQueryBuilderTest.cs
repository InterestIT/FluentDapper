using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using DapperFilterExtensions.Data;
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
        private Mock<IClassMapperFactory> _classMapperFactoryMock;
        private Mock<IPredicateFactory> _predicateFactoryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _classMapperFactoryMock = new Mock<IClassMapperFactory>(MockBehavior.Strict);
            _predicateFactoryMock = new Mock<IPredicateFactory>(MockBehavior.Strict);
        }

        #region CompileShouldCompileSimpleSelect

        [TestMethod]
        public void CompileShouldCompileSimpleSelect()
        {
            // Arrange
            const string expectedQuery = "SELECT [Articles].[Id], [Articles].[ArticleTypeId], [Articles].[Name] FROM [Articles]";

            _classMapperFactoryMock.Setup(f => f.Get<Article>()).Returns(new ArticleClassMapper());

            var queryBuilder = new SelectQueryBuilder<Article, Article>(_classMapperFactoryMock.Object, _predicateFactoryMock.Object);

            // Act
            var query = queryBuilder.Compile();

            // Assert
            GetPrivate<bool>(query, "_compiled").Should().BeTrue();
            GetPrivate<string>(query, "_query").Should().Be(expectedQuery);
        }

        #endregion
        #region CompileShouldCompileSimpleSelectUsingColumnSpecification

        [TestMethod]
        public void CompileShouldCompileSimpleSelectUsingColumnSpecification()
        {
            // Arrange
            Expression<Func<Article, object>>[] fields = { a => a.Id };
            const string expectedQuery = "SELECT [Articles].[Id] FROM [Articles]";

            _classMapperFactoryMock.Setup(f => f.Get<Article>()).Returns(new ArticleClassMapper());

            var queryBuilder = new SelectQueryBuilder<Article, Article>(_classMapperFactoryMock.Object, _predicateFactoryMock.Object, fields);

            // Act
            var query = queryBuilder.Compile();

            // Assert
            GetPrivate<bool>(query, "_compiled").Should().BeTrue();
            GetPrivate<string>(query, "_query").Should().Be(expectedQuery);
        }

        #endregion

        // Private methods
        #region GetPrivate

        private static T GetPrivate<T>(object instance, string fieldName)
        {
            var fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)(fieldInfo?.GetValue(instance) ?? default(T));
        }

        #endregion
    }
}