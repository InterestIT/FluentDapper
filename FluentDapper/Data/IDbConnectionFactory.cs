using System.Data;

namespace FluentDapper.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}