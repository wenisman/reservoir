using System;
using System.Collections.Generic;

using Autofac;
using NUnit.Framework;

using Akka.Actor;
using Akka.TestKit.NUnit;

using Reservoir.Actors;
using Reservoir.Messages;

namespace Reservoir.Test
{

    [TestFixture]
    public class RedisActorTests : TestKit
    {

        [Test]
        public void Should_process_valid_messages()
        {
            var actor = Sys.ActorOf(Props.Create<RedisActor>(null, SupervisorStrategy.DefaultStrategy));

            actor.Tell(new SetStrings(new Dictionary<string, string> { {"TestKey", "TestValue"} }));

            var result = ExpectMsg<ActionComplete>();
            Assert.IsNull(result.Exception);
            Assert.True((bool)result.Result);
        }
    }
}
