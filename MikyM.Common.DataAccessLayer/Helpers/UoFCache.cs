using System.Collections.Generic;

namespace MikyM.Common.DataAccessLayer.Helpers
{
    public static class UoFCache
    {
        static UoFCache()
        {
            CachedTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t => t.GetInterface(nameof(IBaseRepository)) is not null))
                .ToList();
        }

        public static IEnumerable<Type> CachedTypes { get; }
    }
}