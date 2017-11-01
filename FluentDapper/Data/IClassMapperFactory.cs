using System;
using DapperExtensions.Mapper;

namespace FluentDapper.Data
{
    public interface IClassMapperFactory
    {
        IClassMapper Get<TData>();
        IClassMapper Get(Type type);
    }
}