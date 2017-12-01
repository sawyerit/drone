using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.XPath;
using Drone.Core.Interfaces;
using Drone.Shared;

namespace Drone.Core
{
	public abstract class BaseDatasource<TComponentType> : IDroneDataSource
	{
		public HttpWebRequest _requestGet;
		JavaScriptSerializer _jserializer = new JavaScriptSerializer();

		public Type ComponentType { get; set; }
		private static string ComponentTypeName { get; set; }

		public abstract void Process(IDroneDataComponent component);

		protected BaseDatasource()
		{
			ComponentType = typeof(TComponentType);
			ComponentTypeName = ComponentType.Name;
		}

		public HttpStatusCode SendRequest(Uri apiUri, object value, bool retry)
		{
			HttpStatusCode code = HttpStatusCode.InternalServerError;
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiUri);			

			req.Method = "POST";
			req.ContentType = "application/json";
			req.KeepAlive = false;
            req.UseDefaultCredentials = true;
            req.PreAuthenticate = true;

			try
			{
				byte[] data = Encoding.UTF8.GetBytes(_jserializer.Serialize(value));

				using (Stream dataStream = req.GetRequestStream())
				{
					dataStream.Write(data, 0, data.Length);
				}

				using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
				{
					code = response.StatusCode;
					string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
				}

				if (code != HttpStatusCode.Created && retry)
				{
					ExceptionExtensions.LogInformation("Processor.SendRequest", "Trying again due to bad status code. HttpStatusCode: " + code);
					SendRequest(apiUri, value, false);
				}
			}
			catch (Exception e)
			{
				if (retry)
				{
					ExceptionExtensions.LogWarning(e, "Processor.SendRequest", "Trying again due to exception. HttpStatusCode: " + code);
					SendRequest(apiUri, value, false);
				}
				else
					throw e;
			}

			return code;
		}

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
