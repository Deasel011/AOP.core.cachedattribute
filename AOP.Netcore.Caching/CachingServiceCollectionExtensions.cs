using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AOP.Netcore.Caching
{
    public static class CachingServiceCollectionExtensions
    {
        public static void AddCaching<T>(this IServiceCollection services,Type Interface)
        {
            services.AddTransient(Interface, (provider) =>
            {
                return new Cached<T>(provider.GetRequiredService(typeof(IMemoryCache)), provider.GetRequiredService<T>());
            });
        }

        public static void AddCaching<T>(this IServiceCollection services, IDistributedCache distributedCachecache, Type Interface)
        {

            services.AddTransient(Interface, (provider) =>
            {
                return new Cached<T>(distributedCachecache, provider.GetRequiredService<T>());
            });
        }

    }
}
