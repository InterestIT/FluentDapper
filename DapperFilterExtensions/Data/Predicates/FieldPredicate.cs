namespace DapperFilterExtensions.Data.Predicates
{
    public class FieldPredicate<T> : ComparePredicate, IFieldPredicate
        where T : class
    {
        public object Value { get; set; }
    }
}