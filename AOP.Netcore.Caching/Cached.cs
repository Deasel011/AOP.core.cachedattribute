using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace AOP.Netcore.Caching
{
    public class Cached<T>: DispatchProxy
    {
        private IMemoryCache _memoryCache;
        private IDistributedCache _distributedCache;
        private CacheType _cacheType;
        private T _decorated;


        public static T CreateProxy(IMemoryCache memoryCache, T decorated)
        {
            object proxy = Create<T,Cached<T>>();
            ((Cached<T>)proxy).SetParameters(memoryCache, decorated);
            return (T)proxy;
        }

        public static T CreateProxy(IDistributedCache distributedCache, T decorated)
        {
            object proxy = Create<T, Cached<T>>();
            ((Cached<T>)proxy).SetParameters(distributedCache, decorated);
            return (T)proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.GetCustomAttribute(typeof(CachedAttribute)) is null)
                return targetMethod.Invoke(_decorated, args);

            var key = FormatMethodInvokeKey(targetMethod, args);
            if (_memoryCache.TryGetValue(key, out var result))
            {
            }
            else
            {
                result = targetMethod.Invoke(_decorated, args);
                _memoryCache.Set(key, result, ObtainCacheExpiryTimeFromAttribute(targetMethod));
            }

            return result;
        }

        private static Tuple<MethodInfo, object[]> FormatMethodInvokeKey(MethodInfo targetMethod, object[] args)
        {
            return new Tuple<MethodInfo, object[]>(targetMethod, args);
        }

        private static DateTimeOffset ObtainCacheExpiryTimeFromAttribute(ICustomAttributeProvider targetMethod)
        {
            var cachedAttribute = targetMethod.GetCustomAttributes(true).OfType<CachedAttribute>().FirstOrDefault();
            return DateTime.Now.AddSeconds(cachedAttribute?.SecondsTillExpiry ?? 86400);
        }

        private void SetParameters(IMemoryCache memoryCache, T decorated)
        {
            _cacheType = CacheType.Memory;
            _memoryCache = memoryCache;
            _decorated = decorated;
        }

        private void SetParameters(IDistributedCache distributedCache, T decorated)
        {
            _cacheType = CacheType.Distributed;
            _distributedCache = distributedCache;
            _decorated = decorated;
        }

        private enum CacheType
        {
            Memory = 1,
            Distributed = 2
        }
    }
}