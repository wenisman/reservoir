﻿using System;

namespace Reservoir.Messages
{
    public class Message
    {

        public DateTime TimeCreated { get; private set;  }

        public string Id { get; private set; }

        public string Version { get; private set; }

        public Message(string id = "",string version = "1")
        {
            if (string.IsNullOrEmpty(id))
            {
                id = new Guid().ToString();
            }

            Id = id;

            TimeCreated = DateTime.UtcNow;
            Version = version;
        }
    }
}