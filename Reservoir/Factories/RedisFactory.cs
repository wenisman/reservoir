using StackExchange.Redis;
using System;

namespace Reservoir.Factories
{

    public interface IRedisFactory
    {
        ConnectionMultiplexer Multiplexer { get; }
    }


    public class RedisFactory : IRedisFactory, IDisposable
    {

        public ConnectionMultiplexer Multiplexer { get; private set; }

        public RedisFactory(string serverConnection)
        {
            // TODO : handle connection errors etc
            // TODO : read configuration properties
            Multiplexer = ConnectionMultiplexer.Connect(serverConnection);
        }

        public void Dispose()
        {
            Multiplexer.Close();
        }
    }
}
