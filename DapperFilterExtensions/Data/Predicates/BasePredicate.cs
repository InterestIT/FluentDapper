using System;

namespace DapperFilterExtensions.Data.Predicates
{
    public abstract class BasePredicate : IBasePredicate
    {
        public Type EntityType { get; set; }
        public string PropertyName { get; set; }
    }
}