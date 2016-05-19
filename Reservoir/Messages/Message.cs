using System;

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


	public class ActionComplete : Message
	{
		public Exception Exception { get; private set; }

		public object Result { get; private set; }

		public ActionComplete(object result, Exception exception, string id) : base(id)
		{
			Result = result;
			Exception = exception;
		}

	}



    public class ExceptionResult : Message
    {
        public Exception Exception { get; private set; }

        public ExceptionResult(Exception exception, string id) : base(id)
        {
            Exception = exception;
        }
    }

    public class SuccessResult : Message
    {
        public object Result { get; private set; }

        public SuccessResult(object result, string id) : base(id)
        {
            Result = result;
        }
    }
}
