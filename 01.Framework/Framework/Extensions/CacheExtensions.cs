using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Caching
{
    public static class CacheExtensions
    {
        private static readonly object _syncObject = new object();
        public static T GetOrSet<T>(this ICacheManager cache, string key, Func<T> acquire, int cacheTimeByMinute = 60)
        {
            if (cache.IsSet(key))
            {
                return cache.Get<T>(key);
            }
            lock (_syncObject)
            {
                var result = acquire();
                if (cacheTimeByMinute > 0)
                    cache.Set(key, result, cacheTimeByMinute);
                return result;
            }
        }
    }
}
