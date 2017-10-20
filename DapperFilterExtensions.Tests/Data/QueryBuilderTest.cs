using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using DapperExtensions.Mapper;
using DapperFilterExtensions.Data;
using DapperFilterExtensions.Filtering;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DapperFilterExtensions.Tests.Data
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QueryBuilderTest
    {
        #region Variables

        private Mock<IClassMapperFactory> _classMapperFactoryMock;
        private Mock<IPredicateFactory> _predicateFactoryMock;
        private QueryBuilder _queryBuilder;

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void TestInitialize()
        {
            _classMapperFactoryMock = new Mock<IClassMapperFactory>(MockBehavior.Strict);
            _predicateFactoryMock = new Mock<IPredicateFactory>(MockBehavior.Strict);

            // System Under Test (SUT)
            _queryBuilder = new QueryBuilder(_classMapperFactoryMock.Object, _predicateFactoryMock.Object);
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

        private void Todo()
        {
            var connectionMock = new Mock<IDbConnection>(MockBehavior.Strict);

            var queryBuilder = Mock.Of<IQueryBuilder>();
            var query = queryBuilder
                    .Select<Article>(a => a.Id)
                    .Join<ArticleType>(at => at.Name)
                    //.Join<Article, ArticleType>(a => a.ArticleTypeId, at => at.Id, at => at.Name)
                    .Sort(a => a.Name)
                //.Sort<Article>(SortDirection.Descending, a => a.Id)
                ;
            //.Filter<ArticleFilter, Article>(); // TODO Maybe definition only, but always filter later.

            var articles = query.Execute(connectionMock.Object);

            var filter = new ArticleFilter { ArticleId = 1 };
            var filteredArticles = query.Execute(filter, connectionMock.Object);

        }

    }
}
