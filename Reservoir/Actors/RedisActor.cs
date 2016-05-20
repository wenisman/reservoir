using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Akka.Actor;
using StackExchange.Redis;

using Reservoir.Factories;
using Reservoir.Messages;

namespace Reservoir.Actors
{
    public class RedisActor : ReceiveActor
    {
        private IDatabase _database;
        private IRedisFactory _redis;


        public RedisActor(IRedisFactory redis)
        {
            _redis = redis;
        }

        protected override void PreStart()
        {
            base.PreStart();

            _database = _redis.Multiplexer.GetDatabase();
        }


        private void ReceiveMessage()
        {
            Receive<SetStrings>(message => SaveStrings(message));
            Receive<GetStrings>(message => GetStrings(message));
            Receive<ActionComplete>(message => Sender.Tell(message));
        }


        private void SaveStrings(SetStrings message)
        {
            var data = message
                    .Data
                    .Select(value => new KeyValuePair<RedisKey, RedisValue>(value.Key, value.Value))
                    .ToArray();

            _database
                .StringSetAsync(data)
                .ContinueWith((task) => {
					if ((task.IsFaulted || task.IsCanceled))
					{
						new ActionComplete(null, task.Exception, message.Id);
					}
					else if (!task.Result)
					{
						new ActionComplete(null, new System.Exception("Unable to save to redis"), message.Id);
					}
					else
					{
						new ActionComplete(true, null, message.Id);
					}
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Self);
        }


        private void GetStrings(GetStrings message)
        {
            var data = message.Keys.Select(key => (RedisKey)key).ToArray();

            _database
                .StringGetAsync(data)
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
						new ActionComplete(null, task.Exception, message.Id);
					}
					else
                    {
                        new ActionComplete(CreateDictionary(message.Keys, task.Result), null, message.Id);
                    }
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Self);
        }


        private IDictionary<string, string> CreateDictionary(string[] keys, RedisValue[] values)
        {
            var output = new Dictionary<string, string>();
            for (int index = 0; index < values.Length; index++)
            {
                output.Add(keys[index], values[index]);
            }

            return output;
        } 

    }
}
