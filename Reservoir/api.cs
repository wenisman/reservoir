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

        public api()
        {
            CreateSystem();
        }

        public bool Set(IDictionary<string, string> keyValues)
        {
            _redisActor.Tell(new SetStrings(keyValues));

            return true;
        }

        private void CreateSystem()
        {
            _cacheSystem = ActorSystem.Create("ReservoirSystem");
            _redisActor = _cacheSystem.ActorOf(_cacheSystem.DI().Props<RedisActor>(), "redisActor");
        }


    }
}
