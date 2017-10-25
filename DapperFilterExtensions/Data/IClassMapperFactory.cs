using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;

namespace DapperFilterExtensions.Data
{
    public interface IClassMapperFactory
    {
        IClassMapper Get<TData>();
        IClassMapper Get(Type type);
    }

    public class ClassMapperFactory : IClassMapperFactory
    {
        private readonly Type _defaultMapperType;
        private readonly IList<Assembly> _mappingAssemblies;
        private readonly ISqlDialect _sqlDialect;

        private readonly Dictionary<Type, IClassMapper> _classMappers = new Dictionary<Type, IClassMapper>();

        public ClassMapperFactory(Type defaultMapperType, IList<Assembly> mappingAssemblies, ISqlDialect sqlDialect)
        {
            _defaultMapperType = defaultMapperType;
            _mappingAssemblies = mappingAssemblies;
            _sqlDialect = sqlDialect;
        }

        public IClassMapper Get<TData>()
        {
            return Get(typeof(TData));
        }

        public IClassMapper Get(Type entityType)
        {
            IClassMapper map;
            if (!_classMappers.TryGetValue(entityType, out map))
            {
                Type mapType = GetMapType(entityType);
                if (mapType == null)
                {
                    mapType = _defaultMapperType.MakeGenericType(entityType);
                }

                map = Activator.CreateInstance(mapType) as IClassMapper;
                _classMappers[entityType] = map;
            }

            return map;
        }

        protected virtual Type GetMapType(Type entityType)
        {
            Func<Assembly, Type> getType = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                    let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                    where
                        interfaceType != null &&
                        interfaceType.GetGenericArguments()[0] == entityType
                    select type).SingleOrDefault();
            };

            Type result = getType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in _mappingAssemblies)
            {
                result = getType(mappingAssembly);
                if (result != null)
                {
                    return result;
                }
            }

            return getType(entityType.Assembly);
        }

    }
}