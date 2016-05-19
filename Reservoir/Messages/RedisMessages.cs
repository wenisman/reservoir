using System;
using System.Collections.Generic;

namespace Reservoir.Messages
{
    public class SetStrings : Message
    {
        public Dictionary<string, string> Data { get; private set; }

        public SetStrings(Dictionary<string, string> data, string id = "") : base()
        {
            Data = data;
        }
    }

    public class SetHash : Message
    {
        public Dictionary<string, object> Data { get; private set; }

        public string Key { get; private set; }

        public SetHash(string key, Dictionary<string, object> data, string id = "") : base()
        {
            Data = data;
        }
    }


    public class GetStrings : Message
    {
        public string[] Keys { get; private set; }


        public GetStrings(string[] keys, string id = "") : base(id)
        {
            Keys = keys;
        }
    }

}
