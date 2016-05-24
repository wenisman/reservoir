using System;
using System.Collections.Generic;

namespace Reservoir.Messages
{

    #region Set Commands

    public class SetStrings : Message
    {
        public IDictionary<string, string> Data { get; private set; }

        public SetStrings(IDictionary<string, string> data, string id = "") : base()
        {
            Data = data;
        }
    }

    public class SetHash : Message
    {
        public IDictionary<string, string> Data { get; private set; }

        public string Key { get; private set; }

        public SetHash(string key, IDictionary<string, string> data, string id = "") : base()
        {
            Data = data;
        }
    }

    #endregion


    #region Get Commands

    public class GetStrings : Message
    {
        public string[] Keys { get; private set; }


        public GetStrings(string[] keys, string id = "") : base(id)
        {
            Keys = keys;
        }
    }

    #endregion 
}
