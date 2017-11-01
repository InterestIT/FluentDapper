using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DapperExtensions.Mapper;

namespace FluentDapper.Data
{
    public class ClassMapperFactory : IClassMapperFactory
    {
        private readonly Type _defaultMapperType;
        private readonly IList<Assembly> _mappingAssemblies;
        //private readonly ISqlDialect _sqlDialect;

        private readonly Dictionary<Type, IClassMapper> _classMappers = new Dictionary<Type, IClassMapper>();

        public ClassMapperFactory(Type defaultMapperType, IList<Assembly> mappingAssemblies)//, ISqlDialect sqlDialect)
        {
            _defaultMapperType = defaultMapperType;
            _mappingAssemblies = mappingAssemblies;
            //_sqlDialect = sqlDialect;
        }

        public IClassMapper Get<TData>()
        {
            return Get(typeof(TData));
        }

        public IClassMapper Get(Type entityType)
        {
            if (_classMappers.TryGetValue(entityType, out var classMapper))
                return classMapper;

            var mapType = GetMapType(entityType) ?? _defaultMapperType.MakeGenericType(entityType);

            classMapper = Activator.CreateInstance(mapType) as IClassMapper;
            _classMappers[entityType] = classMapper;

            return classMapper;
        }

        protected virtual Type GetMapType(Type entityType)
        {
            Type GetType(Assembly a)
            {
                var types = a.GetTypes();
                return (from type in types let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName) where interfaceType != null && interfaceType.GetGenericArguments()[0] == entityType select type).SingleOrDefault();
            }

            var result = GetType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in _mappingAssemblies)
            {
                result = GetType(mappingAssembly);
                if (result != null)
                {
                    return result;
                }
            }

            return GetType(entityType.Assembly);
        }

    }
}