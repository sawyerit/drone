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
	public abstract class BaseComponent<TComponentType> : IDroneComponent
	{
		public event EventHandler ProcessingCompleted;

		#region Properties

		public Type ComponentType { get; set; }

		public static string ComponentTypeName { get; set; }

		public string ProcessorName
		{
			get
			{
				return this.GetType().Name.ToLower();
			}
		}

		public IDroneDataSource DroneDataSource { get; set; }

		public IDroneDataComponent DroneDataComponent { get; set; }
		
		public BaseContext Context { get; set; }

		public abstract void GetData(object context);

		protected bool VerboseLoggingEnabled
		{
			get
			{
				return GetBoolFromXMLConfig(ProcessorName + "/verboselogging");
			}
		}

		#endregion

		#region public methods

		protected BaseComponent() 
		{
			ComponentType = typeof(TComponentType);
			ComponentTypeName = ComponentType.Name;
			Context = new BaseContext { Status = "waiting", TimeOfStatus = DateTime.Now, DurationPreviousStatus = TimeSpan.FromSeconds(0) };
		}

		protected BaseComponent(IDroneDataSource datasource)
		{
			this.DroneDataSource = datasource;

			ComponentType = typeof(TComponentType);
			ComponentTypeName = ComponentType.Name;
			Context = new BaseContext { Status = "waiting", TimeOfStatus = DateTime.Now, DurationPreviousStatus = TimeSpan.FromSeconds(0) };
		}

		public void SetContextStatus(string status, BaseContext context)
		{
			if (!Object.Equals(context, null))
				lock (context)
					context.SetStatus(status);
		}

		public string GetContextStatus(BaseContext context)
		{
			if (!Object.Equals(context, null))
			{
				lock (context)
				{
					return context.Status;
				}
			}
			return string.Empty;
		}

		public void SetNextRunIntervalByNode(string node, BaseContext context)
		{
			if (!Object.Equals(context, null))
				lock (context)
					context.NextRun = XMLUtility.GetNextRunIntervalByNode(Xml, node);
		}

		protected virtual void RaiseProcessingCompletedEvent(EventArgs e)
		{
			if (this.ProcessingCompleted != null)
				this.ProcessingCompleted(this, e);
		}

		protected void WriteToUsageLogFile(string method, string action, bool isVerbose = false)
		{
			string filename = string.Format("DroneProcessor_Usage_{0:M_d_yyyy}.log", DateTime.Today);

			if (!isVerbose)
				Utility.WriteToLogFile(filename, string.Format("{0} | {1} | {2}", action, method, DateTime.Now));
			else
				if (VerboseLoggingEnabled)
					Utility.WriteToLogFile(filename, string.Format("{0} | {1} | {2}", action, method, DateTime.Now));
		}
		#endregion

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
				ExceptionExtensions.LogError(ex, "BaseComponent.LoadComponentXMLCache()", sXMLPath);
				throw ex;
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

				if (!String.IsNullOrEmpty(xmlString))
				{
					try
					{
						xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(xmlString);
					}
					catch (Exception ex)
					{
						ExceptionExtensions.LogError(ex, "Xml (load)", ComponentTypeName);
					}
				}

				return xmlDoc;
			}
		}

		#endregion
	}
}
