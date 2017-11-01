using System.Collections.Generic;

namespace FluentDapper.Data
{
    public interface IQuery
    {
        string Text { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}