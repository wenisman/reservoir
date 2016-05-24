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
        private IRedisFactory _redis;


        public RedisActor(IRedisFactory redis)
        {
            _redis = redis;
            ReceiveMessage();
        }

        private void ReceiveMessage()
        {
            Receive<SetStrings>(message => SaveStrings(message));
            Receive<GetStrings>(message => GetStrings(message));
        }


        private void SaveStrings(SetStrings message)
        {
            var data = message
                    .Data
                    .Select(value => new KeyValuePair<RedisKey, RedisValue>(value.Key, value.Value))
                    .ToArray();

            _redis
                .Database
                .StringSetAsync(data)
                .ContinueWith(task => {
					if ((task.IsFaulted || task.IsCanceled))
					{
						return new ActionComplete(false, task.Exception, message.Id);
					}
					else if (!task.Result)
					{
						return new ActionComplete(false, new System.Exception("Unable to save to redis"), message.Id);
					}
					else
					{
						return new ActionComplete(true, null, message.Id);
					}
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Sender);
        }


        private void SaveHashes(SetHash message)
        {
            var data = message.Data.Select(item => new HashEntry(item.Key, item.Value)).ToArray();


            _redis
                .Database
                .HashSetAsync(message.Key, data)
                .ContinueWith(task =>
                {
                    if ((task.IsFaulted || task.IsCanceled))
                    {
                        return new ActionComplete(false, task.Exception, message.Id);
                    }
                    else
                    {
                        return new ActionComplete(true, null, message.Id);
                    }
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Sender);
        }


        private void GetStrings(GetStrings message)
        {
            var data = message.Keys.Select(key => (RedisKey)key).ToArray();

            _redis
                .Database
                .StringGetAsync(data)
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
						return new ActionComplete(null, task.Exception, message.Id);
					}
					else
                    {
                        return new ActionComplete(CreateDictionary(message.Keys, task.Result), null, message.Id);
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
