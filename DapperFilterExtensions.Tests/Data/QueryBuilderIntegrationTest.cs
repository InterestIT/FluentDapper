using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
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
    public class QueryBuilderIntegrationTest
    {
        #region Variables

        private IClassMapperFactory _classMapperFactory;
        private IFilterMetadataProvider[] _metadataProviders;
        private IPredicateFactory _predicateFactory;
        private IPredicateQueryBuilderFactory _predicateQueryBuilderFactory;

        private QueryBuilder _queryBuilder;

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void TestInitialize()
        {
            _classMapperFactory = new ClassMapperFactory(typeof(AutoClassMapper<>), new List<Assembly>());//, new SqlServerDialect());
            _metadataProviders = new IFilterMetadataProvider[] { new ArticleFilterMetadataProvider() };
            _predicateFactory = new PredicateFactory(_metadataProviders);
            _predicateQueryBuilderFactory = new PredicateQueryBuilderFactory();

            _queryBuilder = new QueryBuilder(_classMapperFactory, _predicateFactory, _predicateQueryBuilderFactory);
        }

        #endregion

        // Execute
        #region ExecuteShouldExecuteSuccessfullyWithoutFilter

        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteShouldExecuteSuccessfullyWithoutFilter()
        {
            // Arrange
            const string expectedSql = "SELECT [Articles].[Id] FROM [Articles]";

            #region Cool, hence utterly useless

            //var columns = new Dictionary<string, Type>
            //{
            //    { nameof(Article.Id), typeof(int) },
            //    { nameof(Article.ArticleTypeId), typeof(int) },
            //    { nameof(Article.Name), typeof(string) }
            //};
            //
            //var values = new List<List<object>>
            //{
            //    new List<object> {1, 1, "Number 1"},
            //    new List<object> {2, 1, "Number 2"}
            //};
            //
            //var dataReaderMock = CreateDataReaderMock(columns, values);

            #endregion

            var dataReaderMock = CreateDataReaderMock();
            var commandMock = CreateCommandMock(dataReaderMock, expectedSql);
            var connectionMock = CreateConnectionMock(commandMock);

            var selectQuery = _queryBuilder.Select<Article>(a => a.Id);

            // Act
            //var articles = selectQuery.Execute(connectionMock.Object);
            selectQuery.Execute(connectionMock.Object);

            // Assert
            commandMock.VerifySet(c => c.CommandText = expectedSql, Times.Once);
            commandMock.Verify(c => c.CreateParameter(), Times.Never);
        }

        #endregion
        #region ExecuteShouldExecuteSuccessfullyWithFilter

        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteShouldExecuteSuccessfullyWithFilter()
        {
            // Arrange
            const string expectedSql = "SELECT [Articles].[Id] FROM [Articles] WHERE (([Articles].[Id] = @Id_0))";
            var filter = new ArticleFilter { ArticleId = 1 };

            var dataReaderMock = CreateDataReaderMock();

            var parameters = new List<IDbDataParameter>();
            var parametersMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);
            parametersMock.Setup(p => p.Contains("Id_0")).Returns(() => parameters.Count == 1);
            parametersMock.Setup(p => p.Add(It.IsAny<IDbDataParameter>())).Returns((object p) => { parameters.Add((IDbDataParameter)p); return parameters.Count - 1; });
            parametersMock.SetupGet(p => p.Count).Returns(() => parameters.Count);
            parametersMock.Setup(p => p.GetEnumerator()).Returns(() => parameters.GetEnumerator());

            var commandMock = CreateCommandMock(dataReaderMock, expectedSql, parametersMock);

            var connectionMock = CreateConnectionMock(commandMock);

            var selectQuery = _queryBuilder.Select<Article>(a => a.Id);

            // Act
            selectQuery.Execute(filter, connectionMock.Object);

            // Assert
            commandMock.VerifySet(c => c.CommandText = expectedSql, Times.Once);
            commandMock.Verify(c => c.CreateParameter(), Times.Once);
            parametersMock.Verify(p => p.Add(It.IsAny<IDbDataParameter>()), Times.Once);
        }

        #endregion

        // GetQuery
        #region GetQueryShouldGetQuery

        [TestMethod]
        [TestCategory("Integration")]
        public void GetQueryShouldGetQuery()
        {
            // Arrange

            // Act
            var selectQuery = _queryBuilder.Select<Article>(a => a.Id);
            var query = selectQuery.GetQuery();

            // Assert
            query.Text.Should().Be("SELECT [Articles].[Id] FROM [Articles]");
            query.Parameters.Should().BeEmpty();
        }

        #endregion
        #region GetQueryShouldGetQueryWithFilter

        [TestMethod]
        [TestCategory("Integration")]
        public void GetQueryShouldGetQueryWithFilter()
        {
            // Arrange
            var filter = new ArticleFilter { ArticleId = 1 };

            // Act
            var selectQuery = _queryBuilder.Select<Article>(a => a.Id);
            var query = selectQuery.GetQuery(filter);

            // Assert
            query.Text.Should().Be("SELECT [Articles].[Id] FROM [Articles] WHERE (([Articles].[Id] = @Id_0))");
            query.Parameters.Should().HaveCount(1);
        }

        #endregion
        #region GetQueryShouldGetQueryWithJoin

        [TestMethod]
        [TestCategory("Integration")]
        public void GetQueryShouldGetQueryWithJoin()
        {
            // Arrange

            // Act
            var selectQuery = _queryBuilder
                .Select<Article>(a => a.Id)
                .Join<ArticleType>(at => at.Name);

            var query = selectQuery.GetQuery();

            // Assert
            query.Text.Should().Be("SELECT [Articles].[Id], [ArticleTypes].[Name] AS [ArticleTypeName] FROM [Articles] INNER JOIN [ArticleTypes] ON [Articles].[ArticleTypeId] = [ArticleTypes].[Id]");
            query.Parameters.Should().HaveCount(0);
        }

        #endregion
        #region GetQueryShouldGetQueryWithJoinAndFilter

        [TestMethod]
        [TestCategory("Integration")]
        public void GetQueryShouldGetQueryWithJoinAndFilter()
        {
            // Arrange
            var filter = new ArticleFilter { ArticleId = 1 };

            // Act
            var selectQuery = _queryBuilder
                .Select<Article>(a => a.Id)
                .Join<ArticleType>(at => at.Name);
            var query = selectQuery.GetQuery(filter);

            // Assert
            query.Text.Should().Be("SELECT [Articles].[Id], [ArticleTypes].[Name] AS [ArticleTypeName] FROM [Articles] INNER JOIN [ArticleTypes] ON [Articles].[ArticleTypeId] = [ArticleTypes].[Id] WHERE (([Articles].[Id] = @Id_0))");
            query.Parameters.Should().HaveCount(1);
        }

        #endregion

        // Private methods
        #region CreateConnectionMock

        private static Mock<IDbConnection> CreateConnectionMock(IMock<IDbCommand> commandMock)
        {
            var connectionMock = new Mock<IDbConnection>(MockBehavior.Strict);
            connectionMock.SetupGet(c => c.ConnectionString).Returns("");
            connectionMock.SetupGet(c => c.State).Returns(ConnectionState.Closed);
            connectionMock.Setup(c => c.Open());
            connectionMock.Setup(c => c.Close());
            connectionMock.Setup(c => c.CreateCommand()).Returns(commandMock.Object);
            return connectionMock;
        }

        #endregion
        #region CreateCommandMock

        private static Mock<IDbCommand> CreateCommandMock(IMock<IDataReader> dataReaderMock, string sql, IMock<IDataParameterCollection> parametersMock = null)
        {
            var commandMock = new Mock<IDbCommand>(MockBehavior.Strict);
            commandMock.SetupSet(c => c.CommandText = sql);
            commandMock.SetupSet(c => c.CommandType = CommandType.Text);
            commandMock.Setup(c => c.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection)).Returns(dataReaderMock.Object);
            commandMock.Setup(c => c.Dispose());

            if (parametersMock == null)
                return commandMock;

            var parameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);
            parameterMock.SetupAllProperties();

            commandMock.SetupGet(c => c.Parameters).Returns(parametersMock.Object);
            commandMock.Setup(c => c.CreateParameter()).Returns(parameterMock.Object);

            return commandMock;
        }

        #endregion
        #region CreateDataReaderMock

        private static Mock<IDataReader> CreateDataReaderMock(/*Dictionary<string, Type> columns = null, List<List<object>> values = null*/)
        {
            var dataReaderMock = new Mock<IDataReader>(MockBehavior.Strict);

            #region Cool version (hence totally useless)

            //if (columns == null)
            //    columns = new Dictionary<string, Type>();

            //if (values == null)
            //    values = new List<List<object>>();

            //dataReaderMock.SetupGet(r => r.IsClosed).Returns(false);
            //dataReaderMock.SetupGet(r => r.FieldCount).Returns(columns.Count);
            //dataReaderMock.Setup(r => r.GetFieldType(It.IsAny<int>())).Returns((int i) => columns.Values.ToList()[i]);
            //dataReaderMock.Setup(r => r.GetName(It.IsAny<int>())).Returns((int i) => columns.Keys.ToList()[i]);
            //var rowIndex = 0;
            //dataReaderMock.Setup(r => r.Read()).Returns(() => ++rowIndex <= values.Count);
            //dataReaderMock.Setup(r => r[It.IsAny<int>()]).Returns((int i) => values[rowIndex - 1][i] ?? DBNull.Value);
            //dataReaderMock.Setup(r => r.NextResult()).Returns(false);
            //dataReaderMock.Setup(r => r.Dispose());
            //return dataReaderMock;

            #endregion

            dataReaderMock.SetupGet(r => r.IsClosed).Returns(false);
            dataReaderMock.SetupGet(r => r.FieldCount).Returns(0);
            dataReaderMock.Setup(r => r.Read()).Returns(false);
            dataReaderMock.Setup(r => r.NextResult()).Returns(false);
            dataReaderMock.Setup(r => r.Dispose());
            return dataReaderMock;
        }

        #endregion
    }
}