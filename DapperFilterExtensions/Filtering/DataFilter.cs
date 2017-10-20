namespace DapperFilterExtensions.Filtering
{
    public interface IDataFilter<TFilter, TData> where TFilter : IDataFilter<TFilter, TData>
    {
    }

    public class DataFilter<TFilter, TData> : IDataFilter<TFilter, TData> where TFilter : IDataFilter<TFilter, TData>
    {
        //internal List<FilterMetadata<TFilter, TData>> Metadata { get; } = new List<FilterMetadata<TFilter, TData>>();
    }
}