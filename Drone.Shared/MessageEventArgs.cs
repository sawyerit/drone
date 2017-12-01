using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;

namespace Drone.Shared
{
	public class MessageEventArgs : EventArgs
	{
		private Message _message;

		public Message Message
		{
			get { return _message; }
		}

		public MessageEventArgs(Message msg)
		{
			_message = msg;
		}
	}

	public class MessageDelegates
	{
		public delegate void MessageReceivedEventHandler(object sender, MessageEventArgs args);
		public delegate void MessageProcessedEventHandler(object sender);
		public delegate void ShuttingDownEventHandler(object sender);
	}

}
