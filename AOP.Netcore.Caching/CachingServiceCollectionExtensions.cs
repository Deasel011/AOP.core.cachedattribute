using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AOP.Netcore.Caching
{
    public static class CachingServiceCollectionExtensions
    {
        public static void AddCaching<T>(this IServiceCollection services,Type implementationClass)
        {
            services.AddSingleton(typeof(T), (provider) =>
            {
                var proxy = Cached<T>.CreateProxy((IMemoryCache)provider.GetRequiredService(typeof(IMemoryCache)),
                    (T)provider.GetRequiredService(implementationClass),
                    implementationClass);
                return proxy;
            });
        }

        public static void AddCaching<T>(this IServiceCollection services, IDistributedCache distributedCachecache, Type implementationClass)
        {

            services.AddSingleton(typeof(T), (provider) =>
            {
                var proxy = Cached<T>.CreateProxy((IDistributedCache)provider.GetRequiredService(typeof(IDistributedCache)),
                    (T)provider.GetRequiredService(implementationClass),
                    implementationClass);
                return proxy;
            });
        }

    }
}
