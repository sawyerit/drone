using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Threading;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.MarketShare;
using Drone.QueueProcessor.Components;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class MarketShareBulkDataSource : BaseDatasource<QueueProcessorComponent>
	{
		private static Dictionary<int, MarketShareBulkData> _bulkTableDict = new Dictionary<int, MarketShareBulkData>();
		private bool _isVerboseEnabled = false;
		private int _bulkThreshold = 500;
		private int _bulkTimeLimit = 10000;
		private int _lastCount = 0;
		private System.Timers.Timer _bulkTimer = new System.Timers.Timer();
		private static Object _lockObj = new Object();

		public MarketShareBulkDataSource()
		{
			_bulkThreshold = XMLUtility.GetTextFromAccountNode(Xml, "queuemarketshare/bulklimit").ConvertStringToInt(500);
			_bulkTimeLimit = XMLUtility.GetTextFromAccountNode(Xml, "queuemarketshare/bulktimer").ConvertStringToInt(1000);
			_isVerboseEnabled = XMLUtility.GetTextFromAccountNode(Xml, "queuemarketshare/verboselogging").ToLower() == "true";

			_bulkTimer.Elapsed += _bulkTimer_Elapsed;
			_bulkTimer.Interval = _bulkTimeLimit;
			_bulkTimer.Start();
		}


		public override void Process(IDroneDataComponent component)
		{
			MarketShareDataComponent smbComponent = component as MarketShareDataComponent;
			if (!Object.Equals(smbComponent, null))
			{
				AddMarketShareToBulk(smbComponent.MarketShareType);
			}
		}

		#region Database

		internal void AddMarketShareToBulk(MarketShareDataType msType)
		{
			DateTime sampleDate = DateTime.Now;
			if (!DateTime.TryParse(msType.SampleDate, out sampleDate))
				sampleDate = DateTime.Now;

			lock (_lockObj)
			{
				lock (_bulkTableDict)
				{
					//check if it exists, if not, add table then add row
					if (!_bulkTableDict.ContainsKey(msType.TypeId))
					{
						DataTable bulkTable = new DataTable("DomainMarketShareType");
						bulkTable.Columns.Add("DomainID", typeof(int));
						bulkTable.Columns.Add("TypeID", typeof(int));
						bulkTable.Columns.Add("Value", typeof(string));
						bulkTable.Columns.Add("SampleDate", typeof(DateTime));
						bulkTable.Columns.Add("BitMask", typeof(int));

						_bulkTableDict.Add(msType.TypeId, new MarketShareBulkData { LastCount = 0, BulkTable = bulkTable });
					}

					//Make sure row doesn't exist, then add it
					if (!_bulkTableDict[msType.TypeId].BulkTable.AsEnumerable().Any(row => row.Field<int>("DomainID") == msType.DomainId && row.Field<int>("TypeID") == msType.TypeId))
					{
						_bulkTableDict[msType.TypeId].BulkTable.Rows.Add(msType.DomainId, msType.TypeId, (msType.Value != "None" ? msType.Value : null), sampleDate, msType.BitMaskId);
					}
				}

				CheckBulkData(msType.TypeId);
			}
		}

		#endregion

		#region Timer

		void _bulkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_isVerboseEnabled)
				Utility.WriteToLogFile(String.Format("MarketShareBulkDataSource_TimerElapsed{0:M_d_yyyy}", DateTime.Today) + ".log", "BulkTimer Elapsed, CheckBulkData Called - " + DateTime.Now);

			foreach (int bulkItemType in _bulkTableDict.Keys)
			{
				CheckBulkData(bulkItemType);
			}
		}

		private void CheckBulkData(int bulkItemType)
		{
			_bulkTimer.Stop();

			try
			{
				lock (_bulkTableDict)
				{
					//check only the table thats been added to
					if (_bulkTableDict.ContainsKey(bulkItemType))
					{
						MarketShareBulkData bulkItem = _bulkTableDict[bulkItemType];
						if (bulkItem.BulkTable.Rows.Count != 0 && (bulkItem.LastCount >= _bulkThreshold || bulkItem.LastCount == bulkItem.BulkTable.Rows.Count))
						{
							//save to db
							if (_isVerboseEnabled)
								Utility.WriteToLogFile(String.Format("MarketShareBulkDataSource_Insert{0:M_d_yyyy}", DateTime.Today) + ".log"
									, String.Format("Type: {0}, LastCount: {1}, BulkTableCount: {2}, BulkTimeLimit: {3}, BulkThreshold: {4} - Time:{5}", bulkItemType, bulkItem.LastCount, bulkItem.BulkTable.Rows.Count, _bulkTimeLimit, _bulkThreshold, DateTime.Now));

							DataFactory.ExecuteNonQueryTable("MarketShareTableIns", bulkItem.BulkTable);

							//clear datatable
							for (int i = 0; i < bulkItem.BulkTable.Rows.Count; i++)
							{
								bulkItem.BulkTable.Rows[i].Delete();
							}

							bulkItem.BulkTable.AcceptChanges();
							bulkItem.BulkTable.Clear();
						}

						//reset last count
						bulkItem.LastCount = bulkItem.BulkTable.Rows.Count;
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionExtensions.LogWarning(ex, "MarketShareBulkDataSource.SaveBulkData");

				//if tempdb full or other critical db error, re-throw
				if (Utility.IsCriticalDBError(ex)) throw;
			}
			finally
			{
				_bulkTimer.Start();
			}
		}

		#endregion

	}
}
