using System;
using DapperExtensions.Mapper;

namespace DapperFilterExtensions.Data
{
    public interface IClassMapperFactory
    {
        IClassMapper Get<TData>();
        IClassMapper Get(Type type);
    }
}