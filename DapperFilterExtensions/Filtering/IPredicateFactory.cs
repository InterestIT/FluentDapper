using DapperExtensions;

namespace DapperFilterExtensions.Filtering
{
    public interface IPredicateFactory
    {
        IPredicate GetPredicate<TFilter, TData>(IDataFilter<TFilter, TData> filter)
            where TFilter : IDataFilter<TFilter, TData>
            where TData : class;
    }
}