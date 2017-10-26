using System.Collections.Generic;

namespace DapperFilterExtensions.Data.Predicates
{
    public interface IPredicateGroup : IPredicate
    {
        GroupOperator Operator { get; set; }
        IList<IPredicate> Predicates { get; set; }
    }
}