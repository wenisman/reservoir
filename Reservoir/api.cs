using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Akka.Actor;
using Akka.DI.Core;
using Reservoir.Actors;
using Reservoir.Messages;

namespace Reservoir
{
    public class api
    {

        private ActorSystem _cacheSystem;
        private IActorRef _redisActor;
        private IActorRef _memoryActor;

        public api()
        {
            CreateSystem();
        }

        public bool Set(IDictionary<string, string> keyValues)
        {
            var message = new SetStrings(keyValues);

            var redisTask = _redisActor.Ask(message);
            var memoryTask = _memoryActor.Ask(message);

            Task.WaitAll(redisTask, memoryTask);

            return true;
        }

        private void CreateSystem()
        {
            _cacheSystem = ActorSystem.Create("ReservoirSystem");
            _redisActor = _cacheSystem.ActorOf(_cacheSystem.DI().Props<RedisActor>(), "redisActor");
            _memoryActor = _cacheSystem.ActorOf(Props.Create<MemoryActor>(), "memoryActor");
        }


    }
}
