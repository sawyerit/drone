using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Drone.Shared;

namespace Drone.Data
{
	public static class StoredProcedures
	{
		private static CacheItemUpdateCallback OnUpdate = CacheUpdateCallback;

		private static Uri GetUriXmlPath(string path)
		{
			string appPath = String.Empty;
			Uri xmlPath;

			if (!Object.Equals(HttpRuntime.AppDomainAppId, null))
			{
				appPath = HttpRuntime.AppDomainAppPath + "\\Xml";
				xmlPath = new Uri(Path.Combine(appPath, path));
			}
			else
			{
				appPath = Utility.ComponentBaseFolder + "\\Xml";
				xmlPath = new Uri(Path.Combine(appPath, path));
			}

			return xmlPath;
		}

		private static void CacheUpdateCallback(string key, CacheItemUpdateReason callbackreason, out object value, out CacheDependency dependency, out DateTime expiration, out TimeSpan slidingExpiration)
		{
			Uri xmlPath = GetUriXmlPath(@"StoredProcedures.xml");

			dependency = new CacheDependency(xmlPath.LocalPath);
			expiration = Cache.NoAbsoluteExpiration;
			slidingExpiration = Cache.NoSlidingExpiration;
			value = LoadStoredProcedureCache();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
		public static Hashtable LoadStoredProcedureCache()
		{
			XmlDocument xmlDoc = new XmlDocument();
			Hashtable oStoredProcedureHash = new Hashtable();

			Uri xmlPath = GetUriXmlPath(@"StoredProcedures.xml");

			try
			{
				xmlDoc.Load(xmlPath.LocalPath);
				XmlNodeList oNodeList = xmlDoc.SelectNodes("StoredProcedures/StoredProcedure");

				foreach (XmlNode oNode in oNodeList)
				{
					ProcedureDefinition oProcedureDefinition = new ProcedureDefinition(oNode.Attributes["ProcName"].Value, oNode.Attributes["ProcCommand"].Value, oNode.Attributes["ProcDatabase"].Value, oNode.Attributes["ProcConnection"].Value);
					oStoredProcedureHash.Add(oProcedureDefinition.Name, oProcedureDefinition);
				}

				// put into app cache
				using (CacheDependency dependency = new CacheDependency(xmlPath.LocalPath))
				{
					HttpRuntime.Cache.Insert("StoredProcedures", oStoredProcedureHash, dependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, OnUpdate);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to load StoredProcedures XML at " + xmlPath, ex);
			}
			return oStoredProcedureHash;
		}

		public static ProcedureDefinition GetProcedureDefinition(string sProcName)
		{
			Hashtable oStoredProcedureHash = (Hashtable)HttpRuntime.Cache.Get("StoredProcedures") ?? LoadStoredProcedureCache();

			try
			{
				if (oStoredProcedureHash[sProcName] == null)
				{
					LoadStoredProcedureCache();
					oStoredProcedureHash = (Hashtable)HttpRuntime.Cache.Get("StoredProcedures");
				}
				return ((ProcedureDefinition)oStoredProcedureHash[sProcName]);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static string GetProcCommand(string sProcName)
		{
			ProcedureDefinition oProcedureDefinition = GetProcedureDefinition(sProcName);
			return (oProcedureDefinition.Command);
		}

		public static string GetProcConnection(string sProcName)
		{
			ProcedureDefinition oProcedureDefinition = GetProcedureDefinition(sProcName);
			return (oProcedureDefinition.Connection);
		}
	}

	#region ProcedureDefinition Helper Class
	public class ProcedureDefinition
	{
		public string Name { get; set; }
		public string Command { get; set; }
		public string Connection { get; set; }
		public string Database { get; set; }

		public ProcedureDefinition(string sProcName, string sProcCommand, string sDatabase, string sProcConnection)
		{
			Name = sProcName;
			Command = sProcCommand;
			Connection = sProcConnection;
			Database = sDatabase;
		}

		public string FullyQualifiedCommand
		{
			get
			{
				StringBuilder sFQCommand = new StringBuilder();
				if (!String.IsNullOrEmpty(Database))
					sFQCommand.Append(String.Format("[{0}].[dbo].", Database));
				if (!String.IsNullOrEmpty(Command))
					sFQCommand.Append(String.Format("[{0}]", Command));
				return sFQCommand.ToString();
			}
		}
	}
	#endregion
}