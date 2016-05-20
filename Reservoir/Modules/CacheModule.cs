using System.Configuration;

using Autofac;
using Reservoir.Factories;

namespace Reservoir.Modules
{
    public class CacheModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<RedisFactory>()
                .As<IRedisFactory>()
                .WithParameter("serverConnection", ConfigurationManager.AppSettings["RedisConnection"]) 
                .SingleInstance();

        }


    }
}
