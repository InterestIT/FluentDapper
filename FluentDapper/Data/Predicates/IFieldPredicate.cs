namespace FluentDapper.Data.Predicates
{
    public interface IFieldPredicate : IComparePredicate
    {
        object Value { get; set; }
    }
}