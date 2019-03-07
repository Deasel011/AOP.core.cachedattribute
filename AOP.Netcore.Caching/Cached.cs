using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace AOP.Netcore.Caching
{
    public class Cached<T> : DispatchProxy
    {
        private IMemoryCache _memoryCache;
        private IDistributedCache _distributedCache;
        private CacheType _cacheType;
        private T _decorated;
        private Type _implementationType;


        public static T CreateProxy(IMemoryCache memoryCache, T decorated, Type implementationType)
        {
            object proxy = Create<T, Cached<T>>();
            ((Cached<T>) proxy).SetParameters(memoryCache, decorated, implementationType);
            return (T) proxy;
        }

        public static T CreateProxy(IDistributedCache distributedCache, T decorated, Type implementationType)
        {
            object proxy = Create<T, Cached<T>>();
            ((Cached<T>) proxy).SetParameters(distributedCache, decorated, implementationType);
            return (T) proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            switch (_cacheType)
            {
                case CacheType.Memory: return InvokeMemoryCache(targetMethod, args);
                case CacheType.Distributed: return InvokeDistributedCache(targetMethod, args);
                default:
                    throw new Exception("Unknown CacheType has been set. Proxy cannot operate.");
            }
        }

        private object InvokeMemoryCache(MethodInfo targetMethod, object[] args)
        {
            if (_implementationType.GetMethods().First(x => x.Name.Equals(targetMethod.Name))
                .GetCustomAttribute(typeof(CachedAttribute)) is null)
                return targetMethod.Invoke(_decorated, args);

            var key = FormatMethodInvokeKey(targetMethod, args);
            if (_memoryCache.TryGetValue(key, out var result))
            {
            }
            else
            {
                result = targetMethod.Invoke(_decorated, args);
                _memoryCache.Set(key, result,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = ObtainCacheExpiryTimeFromAttribute(targetMethod)
                    });
            }

            return result;
        }

        [Obsolete("This method cannot cache at the moment.")]
        private object InvokeDistributedCache(MethodInfo targetMethod, object[] args)
        {
          return targetMethod.Invoke(_decorated, args);
        }

        private static string FormatMethodInvokeKey(MethodInfo targetMethod, object[] args)
        {
            var funcName = targetMethod.Name;
            var parameters = String.Join(',', targetMethod.GetParameters().ToList().Select(x => x.Name));
            var parameterTypes = String.Join(',', targetMethod.GetParameters().ToList().Select(x => x.ParameterType));
            var parameterValues = String.Join(',', args.ToList().Select(x =>
            {
                switch (x)
                {
                    case string s when x is string:
                        return x.ToString();
                    case int i when x is int:
                        return x.ToString();
                    case decimal d when x is decimal:
                        return x.ToString();
                    case char c when x is char:
                        return x.ToString();
                    case long l when x is long:
                        return x.ToString();
                    default:
                        throw new Exception(
                            "Using Cache on a method having an parameter other than [string,int,decimal,long,char] is not yet supported.");
                }
            }));
            return new StringBuilder().Append(funcName).Append('-').Append(parameters).Append(':').Append(parameterTypes).Append(':')
                .Append(parameterValues).ToString();
        }

        private DateTimeOffset ObtainCacheExpiryTimeFromAttribute(MethodInfo targetMethod)
        {
            var cachedAttribute = _implementationType.GetMethods().First(x => x.Name.Equals(targetMethod.Name)).GetCustomAttributes(true).OfType<CachedAttribute>().FirstOrDefault();
            return DateTime.Now.AddSeconds(cachedAttribute?.SecondsTillExpiry ?? 86400);
        }

        private void SetParameters(IMemoryCache memoryCache, T decorated, Type implementationType)
        {
            _cacheType = CacheType.Memory;
            _memoryCache = memoryCache;
            _decorated = decorated;
            _implementationType = implementationType;
        }

        private void SetParameters(IDistributedCache distributedCache, T decorated, Type implementationType)
        {
            _cacheType = CacheType.Distributed;
            _distributedCache = distributedCache;
            _decorated = decorated;
            _implementationType = implementationType;
        }

        private enum CacheType
        {
            Memory = 1,
            Distributed = 2
        }
    }
}