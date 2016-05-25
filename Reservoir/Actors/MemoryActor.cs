using System;
using System.Collections.Generic;
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
            // TODO : get the default timeout from the configuration.
            
            Parallel.ForEach(message.Data, item =>
            {
                _cache.Set(item.Key, item.Value, new DateTimeOffset(DateTime.Now, TimeSpan.FromSeconds(10)));
            });

            Sender.Tell(new ActionComplete(true, null, message.Id));
        }



        private void RetrieveStrings(GetStrings message)
        {
            IDictionary<string, string> output = new Dictionary<string, string>();

            Parallel.ForEach(message.Keys, key =>
            {
                output.Add(key, (string)_cache.Get(key));
            });

            Sender.Tell(new ActionComplete(output, null, message.Id));
        }



    }
}
