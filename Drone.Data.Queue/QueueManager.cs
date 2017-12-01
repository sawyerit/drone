using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Threading;
using Drone.Shared;
using System.Collections.Generic;

namespace Drone.Data.Queue
{
	public class QueueManager : IDisposable
	{
		#region private properties

		private MessageQueue _queue;
		private string[] _types;
		private static QueueManager _queueManager;
		private static Object lockObj = new Object();

		#endregion

		public static QueueManager Instance
		{
			get
			{
				if (Object.Equals(null, _queueManager))
				{
					lock (lockObj)
						if (Object.Equals(null, _queueManager))
							_queueManager = new QueueManager();
				}

				return _queueManager;
			}
		}

		public bool HasMessages
		{
			get
			{
				bool has = false;
				try
				{
					has = !Object.Equals(null, _queue.Peek(new TimeSpan(1)));
				}
				catch (MessageQueueException me)
				{ }
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "QueueManager.HasMessages");
				}

				return has;

			}
		}

		public int MessageCount
		{
			get
			{
				return GetMessageCount();
			}
		}

		private QueueManager()
		{
			string queuePath = @".\Private$\Drone";

			if (!MessageQueue.Exists(queuePath))
				_queue = MessageQueue.Create(queuePath, true);
			else
				_queue = new MessageQueue(queuePath);

			_queue.Label = "Drone Queue";
			//_queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });
		}

		public void AddToQueue(string body, string name)
		{
			try
			{
				using (MessageQueueTransaction mqt = new MessageQueueTransaction())
				{
					try
					{
						mqt.Begin();
						using (Message myMessage = new Message())
						{
							//myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });
							myMessage.UseDeadLetterQueue = true;
							myMessage.BodyType = 8;

							Byte[] buffer = Encoding.UTF8.GetBytes(body);
							myMessage.BodyStream.Write(buffer, 0, buffer.Length);
							myMessage.BodyStream.Flush();
							myMessage.Label = name;

							_queue.Send(myMessage, mqt);
						}
						mqt.Commit();
					}
					catch (Exception e)
					{
						mqt.Abort();
						ExceptionExtensions.LogError(e, "QueueManager.AddToQueue", "Domain: " + name);
					}
				}
			}
			catch (Exception) { }
		}

		public Message GetFromQueue()
		{
			Message msg = null;

			using (MessageQueueTransaction mqt = new MessageQueueTransaction())
			{
				try
				{
					mqt.Begin();
					msg = _queue.Receive(mqt);
					mqt.Commit();
				}
				catch (Exception)
				{
					mqt.Abort();
				}
			}

			return msg;
		}

        /// <summary>
        /// Used from webservice for debugging whats in the queue
        /// </summary>
        /// <param name="count"></param>
        public List<string> PeekQueue(int countToGet)
        {
            List<string> ms = new List<string>();
            Cursor cursor = _queue.CreateCursor();
            int count = 0;

            Message m = PeekWithoutTimeout(cursor, PeekAction.Current);
            if (m != null)
            {
                ms.Add(Encoding.UTF8.GetString(m.BodyStream.ToByteArray()));
                while ((m = PeekWithoutTimeout(cursor, PeekAction.Next)) != null && count < countToGet - 1)
                {
                    ms.Add(Encoding.UTF8.GetString(m.BodyStream.ToByteArray()));
                    count++;
                }
            }
            return ms;
        }

		#region private methods

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

		public int GetMessageCount()
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


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~QueueManager()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_queue != null)
				{
					_queue.Dispose();
				}
			}
		}
    }
}
