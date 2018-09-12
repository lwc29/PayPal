using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Infrastructure.Cache
{
   public static class CacheHelper 
    {
        private static readonly System.Web.Caching.Cache _objCache = HttpRuntime.Cache;
        public static T Get<T>(string key)
        {
            //System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return (T)_objCache[key];
        }

        public static bool Set<T>(string key, T t, DateTime expire)
        {
            //var obj = Get<T>(key);
            //if (obj != null)
            //{
            //    Remove(key);
            //}

            _objCache.Insert(key, t, null, expire, System.Web.Caching.Cache.NoSlidingExpiration);
            return true;
        }

        public static void ASPCacheRemoveCallBack(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            expensiveObject = null;
            dependency = null;
            absoluteExpiration = DateTime.Now;
            slidingExpiration = TimeSpan.Zero;
            CompanyPay.LogHelper.WriteLine("Key:" + key + "被删除，原因：" + reason + "，时间：" + DateTime.Now.ToString()
            + "可用数:" + HttpRuntime.Cache.EffectivePrivateBytesLimit);
        }

        public static bool Remove(string key)
        {
            _objCache.Remove(key);
            return true;
        }

        public static int GetWhere(string key)
        {
            var result = 0;
            IDictionaryEnumerator di = _objCache.GetEnumerator();
            while (di.MoveNext())
            {
                if (di.Key.ToString().IndexOf(key) > -1) { 
                    result++;
                }
            }
            return result;
        }

        public static string GetAll()
        {
            var str = new StringBuilder();
            IDictionaryEnumerator di = _objCache.GetEnumerator();
            while (di.MoveNext())
            {
                str.AppendLine(string.Format("Key【{0}】Value【{1}】\r\n",di.Key,Newtonsoft.Json.JsonConvert.SerializeObject(di.Value)));
            }
            return str.ToString();
        }
    }
}
