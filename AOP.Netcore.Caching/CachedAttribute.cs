using System;

namespace AOP.Netcore.Caching
{
    public class CachedAttribute:Attribute
    {
        protected internal readonly int SecondsTillExpiry;
        public CachedAttribute(int secondsTillExpiry)
        {
            SecondsTillExpiry = secondsTillExpiry;
        }
    }
}
