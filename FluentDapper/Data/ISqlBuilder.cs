using System;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;

namespace FluentDapper.Data
{
    public interface ISqlBuilder
    {
        string IdentitySql(IClassMapper classMapper);
        string GetTableName(IClassMapper classMapper);
        string GetColumnName(Type type, string propertyName, bool includeAlias);
        string GetColumnName(Type type, string propertyName, bool includeAlias, string alias);
        string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias);
        string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias, string alias);
        string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias);
        string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias, string alias);
        ISqlDialect Dialect { get; }
    }
}