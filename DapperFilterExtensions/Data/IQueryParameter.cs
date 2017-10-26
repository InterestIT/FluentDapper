namespace DapperFilterExtensions.Data
{
    public interface IQueryParameter
    {
        string Name { get; }
        object Value { get; }
    }
}