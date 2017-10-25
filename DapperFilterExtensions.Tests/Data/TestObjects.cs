using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DapperExtensions.Mapper;
using DapperFilterExtensions.Data.Predicates;
using DapperFilterExtensions.Filtering;

namespace DapperFilterExtensions.Tests.Data
{
    [ExcludeFromCodeCoverage]
    internal class Article
    {
        public int Id { get; set; }
        public int ArticleTypeId { get; set; }
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal sealed class ArticleClassMapper : ClassMapper<Article>
    {
        public ArticleClassMapper()
        {
            TableName = "Articles";

            ////have a custom primary key
            //Map(x => x.Id).Key(KeyType.Assigned);

            // auto map all other columns
            AutoMap();
        }
    }

    [ExcludeFromCodeCoverage]
    public class ArticleFilter : IDataFilter<ArticleFilter, Article>
    {
        public int? ArticleId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class ArticleFilterMetadataProvider : IFilterMetadataProvider
    {
        Type IFilterMetadataProvider.Type => typeof(ArticleFilter);

        IList<FilterMetadata> IFilterMetadataProvider.Metadata { get; } = new List<FilterMetadata>
        {
            new FilterMetadata<ArticleFilter, Article>
            {
                FilterExpression = data => data.Id,
                FilterType = Operator.Eq,
                FilterValue = dataFilter => dataFilter.ArticleId,
                DefaultValue = default(int?)
            }
        };
    }

    [ExcludeFromCodeCoverage]
    internal class ArticleType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal sealed class ArticleTypeClassMapper : ClassMapper<ArticleType>
    {
        public ArticleTypeClassMapper()
        {
            TableName = "ArticleTypes";

            ////have a custom primary key
            //Map(x => x.Id).Key(KeyType.Assigned);

            // auto map all other columns
            AutoMap();
        }
    }

}