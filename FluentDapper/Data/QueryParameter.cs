namespace FluentDapper.Data
{
    public class QueryParameter : IQueryParameter
    {
        public string Name { get; }
        public object Value { get; }

        public QueryParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}