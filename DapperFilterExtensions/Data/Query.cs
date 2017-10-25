using System.Collections.Generic;
using System.Linq;

namespace DapperFilterExtensions.Data
{
    public class Query : IQuery
    {
        public string Text { get; }
        public IList<IQueryParameter> Parameters { get; }

        public Query(string text)
        {
            Text = text;
            Parameters = new List<IQueryParameter>();
        }

        public Query(string text, Dictionary<string, object> parameters)
        {
            Text = text;
            Parameters = parameters?.Select(p => (IQueryParameter)new QueryParameter(p.Key, p.Value)).ToList();
        }
    }
}