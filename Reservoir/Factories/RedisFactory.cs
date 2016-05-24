using StackExchange.Redis;
using System;

namespace Reservoir.Factories
{

    public interface IRedisFactory
    {
        ConnectionMultiplexer Multiplexer { get; }

        IDatabase Database { get; }
    }


    public class RedisFactory : IRedisFactory, IDisposable
    {

        public ConnectionMultiplexer Multiplexer { get; private set; }

        public IDatabase Database { get; private set; }

        public RedisFactory(string serverConnection)
        {
            // TODO : handle connection errors etc
            // TODO : read configuration properties
            Multiplexer = ConnectionMultiplexer.Connect(serverConnection);
            Database = Multiplexer.GetDatabase();
        }

        public void Dispose()
        {
            Multiplexer.Close();
        }
    }
}
