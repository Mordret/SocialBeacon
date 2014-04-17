using System;

namespace AbstractClasses
{
	[Serializable]
	public class ChatMessage
	{
        string to, from, uID, message;
		bool isMine;

        public string To { get { return to; } }
        public string From { get { return from; } }
		public string UID { get { return uID; } }
		public string Message { get { return message; } }
		public bool IsMine { get { return isMine; } }

		public ChatMessage (string to, string from, string uID, string message, bool isMine)
		{
            this.to = to;
            this.from = from;
			this.uID = uID;
			this.message = message;
			this.isMine = isMine;
		}
	}
}

