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

        protected override void PreStart()
        {
            base.PreStart();
            _database = RedisFactory.Instance.Multiplexer.GetDatabase();
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

            _database.StringSetAsync(data)
                .ContinueWith((task) => {
                    if ((task.IsFaulted || task.IsCanceled) || !task.Result)
                    {
                        new ExceptionResult(task.Exception, message.Id);
                    }
                    else
                    {
                        new SuccessResult(true, message.Id);
                    }
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Self);
        }


        private void GetStrings(GetStrings message)
        {
            var data = message.Keys.Select(key => (RedisKey)key).ToArray();

            _database.StringGetAsync(data)
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        new ExceptionResult(task.Exception, message.Id);
                    }
                    else
                    {
                        var output = new Dictionary<string, string>();
                        for (int index = 0; index < message.Keys.Length; index++)
                        {
                            output.Add(message.Keys[index], task.Result[index]);
                        }

                        new SuccessResult(output, message.Id);
                    }
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Self);
        }

    }
}
