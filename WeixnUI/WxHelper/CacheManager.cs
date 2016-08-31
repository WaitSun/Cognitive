using System;
using System.Web;
using System.Web.Caching;

namespace SrpAPI.Caching
{
    public class CacheManager
    {
        public static CacheManager Instance { get { return new CacheManager(); } }
        public T Get<T>(string key)
        {
            return (T)HttpRuntime.Cache.Get(key);
        }

        public void Insert<T>(string key, T value, string DependencyFile)
        {
            CacheDependency cd = new CacheDependency(DependencyFile);
            HttpRuntime.Cache.Insert(key, value, cd);
        }

        public void Insert<T>(string key, T value, DateTime absoluteExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration);
        }

        public void Insert<T>(string key, T value, TimeSpan slidingExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration);
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
