﻿using System;
using System.Collections.Generic;
using System.Linq;
using MikyM.Common.DataAccessLayer.Repositories;

namespace MikyM.Common.DataAccessLayer.Helpers
{
    public static class UoFCache
    {
        public static IEnumerable<Type> CachedTypes { get; }

        static UoFCache()
        {
            CachedTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()
                    .Where(t => t.GetInterface(nameof(IBaseRepository)) is not null))
                .ToList();
        }
    }
}
