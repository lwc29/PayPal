using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Infrastructure.Cache
{
   public class CacheContext : ICacheContext
    {
        private readonly System.Web.Caching.Cache _objCache = HttpRuntime.Cache;
        public T Get<T>(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return (T)objCache[key];
        }

        public bool Set<T>(string key, T t, DateTime expire)
        {
            var obj = Get<T>(key);
            if (obj != null)
            {
                Remove(key);
            }

            _objCache.Insert(key, t, null, expire, System.Web.Caching.Cache.NoSlidingExpiration);
            return true;
        }

        public bool Remove(string key)
        {
            _objCache.Remove(key);
            return true;
        }
    }
}
