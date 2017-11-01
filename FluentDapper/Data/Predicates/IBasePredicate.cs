using System;

namespace FluentDapper.Data.Predicates
{
    public interface IBasePredicate : IPredicate
    {
        Type EntityType { get; }
        string PropertyName { get; }
    }
}