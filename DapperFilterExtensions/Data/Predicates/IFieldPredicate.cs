namespace DapperFilterExtensions.Data.Predicates
{
    public interface IFieldPredicate : IComparePredicate
    {
        object Value { get; set; }
    }
}