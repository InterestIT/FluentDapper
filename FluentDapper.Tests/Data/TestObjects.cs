using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DapperExtensions.Mapper;
using FluentDapper.Data.Predicates;
using FluentDapper.Filtering;

namespace FluentDapper.Tests.Data
{
    [ExcludeFromCodeCoverage]
    public class Article
    {
        public int Id { get; set; }
        public int ArticleTypeId { get; set; }
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class ArticleClassMapper : ClassMapper<Article>
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
    public class ArticleFilterMetadataProvider : IFilterMetadataProvider
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
    public class ArticleType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class ArticleTypeClassMapper : ClassMapper<ArticleType>
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