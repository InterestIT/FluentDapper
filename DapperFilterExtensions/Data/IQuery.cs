using System.Collections.Generic;

namespace DapperFilterExtensions.Data
{
    public interface IQuery
    {
        string Text { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}