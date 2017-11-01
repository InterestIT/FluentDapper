using System;

namespace FluentDapper.Data.Predicates
{
    public abstract class BasePredicate : IBasePredicate
    {
        public Type EntityType { get; set; }
        public string PropertyName { get; set; }
    }
}