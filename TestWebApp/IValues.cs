using AOP.Netcore.Caching;

namespace TestWebApp
{
    public interface IValues
    {
        string getOne();
        string[] getMany();
    }
    class Values:IValues
    {
        public string getOne()
        {
            return "value";
        }

        public string[] getMany()
        {
            return new string[] {"value1", "value2"};
        }
    }
}
