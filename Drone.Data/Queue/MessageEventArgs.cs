using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace SocialMedia.Data.Queue
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
	}
}
