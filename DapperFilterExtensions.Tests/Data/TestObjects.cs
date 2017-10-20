using DapperExtensions.Mapper;
using DapperFilterExtensions.Filtering;

namespace DapperFilterExtensions.Tests.Data
{
    internal class Article
    {
        public int Id { get; set; }
        public int ArticleTypeId { get; set; }
        public string Name { get; set; }
    }

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

    public class ArticleFilter : IDataFilter<ArticleFilter, Article>
    {
        public int? ArticleId { get; set; }
    }

    internal class ArticleType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}