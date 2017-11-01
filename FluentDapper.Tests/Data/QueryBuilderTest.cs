using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentDapper.Data;
using FluentDapper.Data.Predicates;
using FluentDapper.Filtering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FluentDapper.Tests.Data
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QueryBuilderTest
    {
        #region Variables

        private Mock<IClassMapperFactory> _classMapperFactoryMock;
        private Mock<IPredicateFactory> _predicateFactoryMock;
        private Mock<IPredicateQueryBuilderFactory> _predicateQueryBuilderFactoryMock;

        private QueryBuilder _queryBuilder;

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void TestInitialize()
        {
            _classMapperFactoryMock = new Mock<IClassMapperFactory>(MockBehavior.Strict);
            _predicateFactoryMock = new Mock<IPredicateFactory>(MockBehavior.Strict);
            _predicateQueryBuilderFactoryMock = new Mock<IPredicateQueryBuilderFactory>(MockBehavior.Strict);

            // System Under Test (SUT)
            _queryBuilder = new QueryBuilder(_classMapperFactoryMock.Object, _predicateFactoryMock.Object, _predicateQueryBuilderFactoryMock.Object);
        }

        #endregion

        // Select
        #region SelectShouldReturnSelectQueryBuilder

        [TestMethod]
        public void SelectShouldReturnSelectQueryBuilder()
        {
            // Arrange

            // Act
            var query = _queryBuilder.Select<Article>(a => a.Id);

            // Assert
            query
                .Should().NotBeNull()
                .And.BeOfType<SelectQueryBuilder<Article, Article>>();
        }

        #endregion
    }
}
