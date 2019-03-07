using AOP.Netcore.Caching;

namespace TestWebApp
{
    public interface IValues
    {
        string getOne();
        string[] getMany();
    }
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
}
