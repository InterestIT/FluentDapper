using System;

namespace DapperFilterExtensions.Data.Predicates
{
    public interface IBasePredicate : IPredicate
    {
        Type EntityType { get; }
        string PropertyName { get; }
    }
}