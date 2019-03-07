# AOP.core.cachedattribute


How to use Method Caching:
From your .net core startup file, you have to setup the caching service as follow:

1 - Add a memory cache to the service collection with an appropriate expiration scan frequency
```CSHARP
services.AddMemoryCache(mco => new MemoryCacheOptions { ExpirationScanFrequency = new TimeSpan(0, 0, 0, 5) });
```
2 - Add to the service collection the base item you want to proxy with the CachedAttribute
```CSHARP
services.AddSingleton<Values>();
```
3 - Add the cache to the base item's interface while giving the base object type as a parameter!
```CSHARP
services.AddCaching<IValues>(typeof(Values));
```

Once all three steps are done, you can start placing the Cached(SecondsTillExpiry) attribute on any method with simple passed arguments as currently, the key to get a cached value is the direct method call toString!

```CSHARP
public class Values:IValues
    {
        [Cached(10)]
        public string getOne()
        {
            return "value";
        }

        [Cached(30)]
        public string[] getMany()
        {
            return new string[] {"value1", "value2"};
        }
    }
```

Anything other than string,int,decimal,long,char is not supported as of release 0.1.0 D: !!

:)
