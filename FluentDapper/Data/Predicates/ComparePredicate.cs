namespace FluentDapper.Data.Predicates
{
    public abstract class ComparePredicate : BasePredicate
    {
        public Operator Operator { get; set; }
        public bool Negate { get; set; }
    }
}