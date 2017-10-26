namespace DapperFilterExtensions.Data.Predicates
{
    public interface IComparePredicate : IBasePredicate
    {
        Operator Operator { get; }
        bool Negate { get; }
    }
}