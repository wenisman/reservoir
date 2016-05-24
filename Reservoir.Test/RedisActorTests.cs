﻿using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;

using Akka.Actor;
using Akka.TestKit.NUnit;
using StackExchange.Redis;

using Reservoir.Actors;
using Reservoir.Factories;
using Reservoir.Messages;
using System.Threading.Tasks;

namespace Reservoir.Test
{

    [TestFixture]
    public class RedisActorTests : TestKit
    {

        [Test]
        public void Should_process_valid_messages()
        {
            var props = Props.Create<RedisActor>(CreateRedisMock());
            var actor = Sys.ActorOf(props, "redisActor");
            actor.Tell(new SetStrings(new Dictionary<string, string> { {"TestKey", "TestValue"} }));
            var result = ExpectMsg<ActionComplete>(TimeSpan.FromSeconds(30));

            Assert.IsNull(result.Exception);
            Assert.True((bool)result.Result);
        }



        private IRedisFactory CreateRedisMock()
        {
            var factoryMock = new Mock<IRedisFactory>();

            var databaseMock = new Mock<IDatabase>();

            databaseMock.Setup(mock => mock.StringSetAsync(It.IsAny<KeyValuePair<RedisKey, RedisValue>[]>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
            factoryMock.SetupGet(mock => mock.Database).Returns(databaseMock.Object);

            return factoryMock.Object;
        }
    }
}
