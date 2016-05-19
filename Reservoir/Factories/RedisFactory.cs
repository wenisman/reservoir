using StackExchange.Redis;


namespace Reservoir.Factories
{
    public class RedisFactory
    {

        public ConnectionMultiplexer Multiplexer;
        private static RedisFactory _instance;

        public static RedisFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RedisFactory();
                }

                return _instance;
            }
        }


        protected RedisFactory()
        {
            // TODO : handle connection errors etc
            // TODO : read configuration properties
            Multiplexer = ConnectionMultiplexer.Connect("192.168.33.20:7000");
        }


    }
}
