using DapperExtensions.Mapper;

namespace DapperFilterExtensions.Data
{
    public interface IClassMapperFactory
    {
        IClassMapper Get<TData>();
    }

    public class ClassMapperFactory : IClassMapperFactory
    {
        public IClassMapper Get<TData>()
        {
            throw new System.NotImplementedException();
        }
    }
}