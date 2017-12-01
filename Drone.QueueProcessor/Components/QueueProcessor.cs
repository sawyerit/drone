using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	[Export(typeof(IDroneComponent))]
	public class QueueProcessor : BaseComponent<QueueProcessorComponent>
	{
		#region public properties

		public event MessageDelegates.MessageReceivedEventHandler MessageReceived;

		public List<BaseQueueComponent<QueueProcessorComponent>> Components { get; set; }
		private int _processingCounter = 0;
		private const int MAX_PROCESSING = 50000;

		QueueManager _qm;

		#endregion

		#region constructors

		[ImportingConstructor]
		public QueueProcessor()
			: base()
		{
			_qm = QueueManager.Instance;

			Components = WireupComponents();
		}

		#endregion

		/// <summary>
		/// Main method that gathers data
		/// </summary>
		/// <param name="context">iDroneContext</param>
		public override void GetData(object context)
		{
			try
			{
				BaseContext cont = context as BaseContext;

				lock (cont) cont.NextRun = XMLUtility.GetNextRunInterval(Xml);

				if (!Object.Equals(cont, null) && GetContextStatus(cont) != "processing")
				{
					Context = cont;
					SetContextStatus("processing", cont);

					if (XMLUtility.IsEnabled(Xml))
					{
						//do work
						WriteToUsageLogFile("QueueProcessor.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started QueueProcessor calls");

						while (!cont.ShuttingDown)
						{

							try
							{
								int counters = 0;
								lock (this)
									counters = _processingCounter;

								if (_qm.HasMessages && counters == 0)
								{
									ProcessQueue();
								}
							}
							catch (Exception ex)
							{
								ExceptionExtensions.LogWarning(ex, "QueueProcessor.GetData");
							}

							Thread.Sleep(5000);
						}

						WriteToUsageLogFile("QueueProcessor.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed QueueProcessor calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				SetContextStatus("waiting", Context);
				ExceptionExtensions.LogError(e, "Drone.QueueProcessor.Components.QueueProcessor.GetData()");
			}
		}

		private void ProcessQueue()
		{
			if ((_processingCounter < MAX_PROCESSING) && !Context.ShuttingDown) //todo: maybe remove the max processing and let it rip, since its putting them into a list
			{
				int numToProcess = (MAX_PROCESSING - _processingCounter);
				for (int i = 0; i < numToProcess; i++)
				{
					if (!_qm.HasMessages)
						break;

					Message msg = _qm.GetFromQueue();
					FireRecieveEvent(msg);
				}
			}
		}

		private void FireRecieveEvent(Message msg)
		{
			if (MessageReceived != null)
			{
				var receivers = MessageReceived.GetInvocationList();
				foreach (MessageDelegates.MessageReceivedEventHandler receiver in receivers)
				{
					receiver.BeginInvoke(this, new MessageEventArgs(msg), null, null);
				}
			}
		}

		private List<BaseQueueComponent<QueueProcessorComponent>> WireupComponents()
		{
			List<BaseQueueComponent<QueueProcessorComponent>> list = new List<BaseQueueComponent<QueueProcessorComponent>>() { new QueueCrunchbase()
																																																												, new QueueFacebook()
																																																												, new QueueMarketShare()
																																																												, new QueueTwitter()
																																																												, new QueueYouTube()
																																																												, new QueuePortfolio()
			};

			foreach (var item in list)
			{
				item.MessageProcessed += QueueProcessor_MessageProcessed;
				item.MessageProcessing += QueueProcessor_MessageProcessing;
				item.ShuttingDown += QueueProcessor_ShuttingDown;
				MessageReceived += item.MSMQManager_MessageReceived;
			}

			return list;
		}

		/// <summary>
		/// Decrement counter when processor is finished
		/// </summary>
		/// <param name="sender"></param>
		void QueueProcessor_MessageProcessed(object sender)
		{
			lock (this)
				_processingCounter--;

			if (!Context.ShuttingDown)
				ProcessQueue();
		}

		void QueueProcessor_MessageProcessing(object sender)
		{
			lock (this)
				_processingCounter++;
		}

		void QueueProcessor_ShuttingDown(object sender)
		{
			Context.ShuttingDown = true;
		}
	}
}
