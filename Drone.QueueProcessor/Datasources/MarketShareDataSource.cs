using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.MarketShare;
using Drone.QueueProcessor.Components;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class MarketShareDataSource : BaseDatasource<QueueProcessorComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			MarketShareDataComponent smbComponent = component as MarketShareDataComponent;
			if (!Object.Equals(smbComponent, null))
			{
				SaveMarketShare(smbComponent.MarketShareType);
			}
		}

		#region Database

		private static void SaveMarketShare(MarketShareDataType msType)
		{
			try
			{
				DateTime sampleDate = DateTime.Now;
				if (!DateTime.TryParse(msType.SampleDate, out sampleDate))
					sampleDate = DateTime.Now;

				List<KeyValuePair<string, object>> paramList = new List<KeyValuePair<string, object>>();
				paramList.Add(new KeyValuePair<string, object>("@DomainId", msType.DomainId));
				paramList.Add(new KeyValuePair<string, object>("@TypeId", msType.TypeId));
				paramList.Add(new KeyValuePair<string, object>("@BitMask", msType.BitMaskId));
				paramList.Add(new KeyValuePair<string, object>("@SampleDate", sampleDate));
				if (msType.Value != "None")
					paramList.Add(new KeyValuePair<string, object>("@Value", msType.Value));

				DataFactory.ExecuteNonQuery("MarketShareInsUp", paramList);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("deadlocked"))
				{
					SaveMarketShare(msType);
					ExceptionExtensions.LogInformation("MarketShareDataSource.SaveMarketShare", "Deadlock encountered, trying again");
				}
				else if (e.Message.Contains("Timeout expired"))
				{
					Thread.Sleep(5000);
					SaveMarketShare(msType);
				}
				else
				{
					ExceptionExtensions.LogError(e, "MarketShareDataSource.SaveMarketShare", "Name: " + msType.Value);

					//if tempdb full or other critical db error, re-throw
					if (Utility.IsCriticalDBError(e)) throw;
				}
			}
		}

		#endregion

	}
}
