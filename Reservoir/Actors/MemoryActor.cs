using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

using Akka.Actor;

using Reservoir.Messages;

namespace Reservoir.Actors
{
    public class MemoryActor : ReceiveActor
    {

        private ObjectCache _cache { get; set; }

        public MemoryActor()
        {
            _cache = MemoryCache.Default;
        }


        private void ReceiveMessage()
        {
            Receive<SetStrings>(message => SaveStrings(message));
        }



        private void SaveStrings(SetStrings message)
        {
            // TODO : save the information to the in memory cache

           Parallel.ForEach(message.Data, item =>
            {
                _cache.Set(item.Key, item.Value, new DateTimeOffset(DateTime.Now, TimeSpan.FromSeconds(10)));
            });

            Sender.Tell(new ActionComplete(true, null, message.Id));
        }

    }
}
