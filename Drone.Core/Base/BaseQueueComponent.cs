using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.XPath;
using Drone.Core.Interfaces;
using Drone.Shared;

namespace Drone.Core
{
	public abstract class BaseQueueComponent<TComponentType>
	{
		public event MessageDelegates.MessageProcessedEventHandler MessageProcessed;
		public event MessageDelegates.MessageProcessedEventHandler MessageProcessing;
		public event MessageDelegates.ShuttingDownEventHandler ShuttingDown;

		public Type ComponentType { get; set; }

		public static string ComponentTypeName { get; set; }

		public string ProcessorName
		{
			get
			{
				return this.GetType().Name.ToLower();
			}
		}

		public BaseContext Context { get; set; }

		protected bool VerboseLoggingEnabled
		{
			get
			{
				return GetBoolFromXMLConfig(ProcessorName + "/verboselogging");
			}
		}

		public IDroneDataSource QueueComponentDataSource { get; set; }

		protected BaseQueueComponent()
		{
			ComponentType = typeof(TComponentType);
			Context = new BaseContext { Status = "running", TimeOfStatus = DateTime.Now, DurationPreviousStatus = TimeSpan.FromSeconds(0) };
			ComponentTypeName = ComponentType.Name;
		}

		#region events

		public void FireMessageProcessedEvent()
		{
			if (MessageProcessed != null)
			{
				MessageProcessed.Invoke(this);
			}
		}

		public void FireMessageProcessingEvent()
		{
			if (MessageProcessing != null)
			{
				MessageProcessing.Invoke(this);
			}
		}

		public void FireShuttingDownEvent()
		{
			if (ShuttingDown != null)
			{
				ShuttingDown.Invoke(this);
			}
		}

		public void MSMQManager_MessageReceived(object sender, MessageEventArgs args)
		{
			ProcessMessage(sender, args);
		}

		#endregion

		public virtual void ProcessMessage(object sender, MessageEventArgs args) { }

		protected void WriteToUsageLogFile(string method, string action, bool isVerbose = false)
		{
			string filename = string.Format("DroneQueueProcessor_Usage_{0:M_d_yyyy}.log", DateTime.Today);

			if (!isVerbose)
				Utility.WriteToLogFile(filename, string.Format("{0} | {1} | {2}", action, method, DateTime.Now));
			else
				if (VerboseLoggingEnabled)
					Utility.WriteToLogFile(filename, string.Format("{0} | {1} | {2}", action, method, DateTime.Now));
		}

		#region private methods

		private bool GetBoolFromXMLConfig(string node)
		{
			bool boolVal;
			bool.TryParse(XMLUtility.GetTextFromAccountNode(Xml, node), out boolVal);
			return boolVal;
		}

		#endregion

		#region Cache Logic

		private static CacheItemUpdateCallback OnUpdate = CacheUpdateCallback;
		private MessageDelegates.MessageReceivedEventHandler messageReceived;

		private static void CacheUpdateCallback(string key, CacheItemUpdateReason callbackreason, out object value, out CacheDependency dependency, out DateTime expiration, out TimeSpan slidingExpiration)
		{
			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", ComponentTypeName + ".xml");
			dependency = new CacheDependency(sXMLPath);
			expiration = Cache.NoAbsoluteExpiration;
			slidingExpiration = Cache.NoSlidingExpiration;
			value = LoadComponentXMLCache();
		}

		public static string LoadComponentXMLCache()
		{
			XmlDocument xmlDoc = new XmlDocument();

			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", ComponentTypeName + ".xml");
			string xmlString = String.Empty;

			try
			{
				xmlDoc.Load(sXMLPath);
				StringWriter sw = new StringWriter();
				XmlTextWriter xw = null;
				try
				{
					xw = new XmlTextWriter(sw);
				}
				catch
				{
					if (sw != null) sw.Dispose();
				}

				using (xw)
				{
					xmlDoc.WriteTo(xw);
					xmlString = sw.ToString();
				}

				// put into app cache
				using (CacheDependency dependency = new CacheDependency(sXMLPath))
				{
					HttpRuntime.Cache.Insert(ComponentTypeName + "XML", xmlString, dependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, OnUpdate);
				}
			}
			catch (Exception ex)
			{
				throw new FileNotFoundException("Unable to load Social Media account XML at " + sXMLPath, ex);
			}
			return xmlString;
		}

		public IXPathNavigable Xml
		{
			get
			{
				string xmlString = HttpRuntime.Cache.Get(ComponentTypeName + "XML") as string;
				XmlDocument xmlDoc = null;

				if (String.IsNullOrEmpty(xmlString))
				{
					xmlString = LoadComponentXMLCache();
				}

				try
				{
					xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(xmlString);
				}
				catch (Exception ex)
				{
					ExceptionExtensions.LogError(ex, "Xml (load)", ComponentTypeName);
				}
				return xmlDoc;
			}
		}

		#endregion
	}
}
