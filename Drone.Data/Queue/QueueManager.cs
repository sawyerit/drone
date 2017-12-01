using System;
using System.Messaging;
using SocialMedia.Shared;

namespace SocialMedia.Data.Queue
{
	public class QueueManager
	{
		#region constants

		private const int MAX_PROCESSES = 5;

		#endregion

		#region public properties

		public event MessageDelegates.MessageReceivedEventHandler MessageReceived;

		#endregion

		#region private properties

		private int _processingCounter;
		private MessageQueue _queue;
		private string[] _types;

		#endregion

		public QueueManager(string queuePath, string[] types)
		{
			_types = types;

			if (!MessageQueue.Exists(queuePath))
				_queue = MessageQueue.Create(queuePath, true);
			else
				_queue = new MessageQueue(queuePath);

			_queue.Label = "Social Media Queue";
			_queue.Formatter = new XmlMessageFormatter(_types);
		}

		public void AddToQueue(object body, string permaLink)
		{
			try
			{
				using (MessageQueueTransaction mqt = new MessageQueueTransaction())
				{
					try
					{
						mqt.Begin();
						using (Message myMessage = new Message(body))
						{
							myMessage.UseDeadLetterQueue = true;
							_queue.Send(myMessage, mqt);
						}
						mqt.Commit();
					}
					catch (Exception e)
					{
						mqt.Abort();
						ExceptionExtensions.LogError(e, "QueueManager.AddToQueue", "Domain: " + permaLink);
					}
				}
			}
			catch (Exception) { }

			GetFromQueue();
		}

		public void GetFromQueue()
		{
			lock (this)
			{
				if (_processingCounter < MAX_PROCESSES)
				{
					using (MessageQueueTransaction mqt = new MessageQueueTransaction())
					{
						try
						{
							//Message msg = _queue.Peek(new TimeSpan(1));
							if (GetMessageCount() > 0)
							{
								mqt.Begin();
								Message msg = _queue.Receive(mqt);
								if (!Object.Equals(msg, null))
								{
									msg.Formatter = new XmlMessageFormatter(_types);
									_processingCounter++;

									//object bdy = msg.Body;

									FireRecieveEvent(msg);
								}
								mqt.Commit();
							}
						}
						catch (Exception)
						{
							mqt.Abort();
						}
					}
				}
			}
		}

		public void LookForMore()
		{
			lock (this)
				_processingCounter--;

			GetFromQueue();
		}

		#region private methods

		private void FireRecieveEvent(Message msg)
		{
			if (MessageReceived != null)
			{
				MessageReceived(this, new MessageEventArgs(msg));
			}
		}

		private Message PeekWithoutTimeout(Cursor cursor, PeekAction action)
		{
			Message ret = null;
			try
			{
				ret = _queue.Peek(new TimeSpan(1), cursor, action);
			}
			catch (MessageQueueException mqe)
			{
				if (!mqe.Message.ToLower().Contains("timeout"))
				{
					throw;
				}
			}
			return ret;
		}

		private int GetMessageCount()
		{
			int count = 0;
			Cursor cursor = _queue.CreateCursor();

			Message m = PeekWithoutTimeout(cursor, PeekAction.Current);
			if (m != null)
			{
				count = 1;
				while ((m = PeekWithoutTimeout(cursor, PeekAction.Next)) != null)
				{
					count++;
				}
			}
			return count;
		}

		#endregion



	}
}
