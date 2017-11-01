namespace FluentDapper.Data
{
    public interface IQueryParameter
    {
        string Name { get; }
        object Value { get; }
    }
}