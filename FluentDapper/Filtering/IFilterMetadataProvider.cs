using System;
using System.Collections.Generic;

namespace FluentDapper.Filtering
{
    public interface IFilterMetadataProvider
    {
        Type Type { get; }
        IList<FilterMetadata> Metadata { get; }
    }
}