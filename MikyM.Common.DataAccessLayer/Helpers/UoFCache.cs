using System;
using System.Collections.Generic;
using System.Linq;
using MikyM.Common.DataAccessLayer.Repositories;

namespace MikyM.Common.DataAccessLayer.Helpers
{
    public static class UoFCache
    {
        public static List<Type> CachedTypes { get; } = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes()
                .Where(t => t.BaseType == typeof(Repository<>) || t.BaseType == typeof(ReadOnlyRepository<>) ||
                            t == typeof(ReadOnlyRepository<>)))
            .ToList();
    }
}
