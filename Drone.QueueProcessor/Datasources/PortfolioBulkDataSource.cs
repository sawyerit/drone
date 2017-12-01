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
using Drone.QueueProcessor.Components;
using Drone.Shared;
using Drone.Entities.Portfolio;
using System.Reflection;

namespace Drone.QueueProcessor.Datasources
{
	public class PortfolioBulkDataSource : BaseDatasource<QueueProcessorComponent>
	{
		private static Dictionary<int, PortfolioBulkData> _bulkTableDict = new Dictionary<int, PortfolioBulkData>();
		private bool _isVerboseEnabled = false;
		private int _bulkThreshold = 500;
		private int _bulkTimeLimit = 10000;
		private int _lastCount = 0;
		private System.Timers.Timer _bulkTimer = new System.Timers.Timer();
		private static Object _lockObj = new Object();

		public PortfolioBulkDataSource()
		{
			_bulkThreshold = XMLUtility.GetTextFromAccountNode(Xml, "queueportfolio/bulklimit").ConvertStringToInt(500);
			_bulkTimeLimit = XMLUtility.GetTextFromAccountNode(Xml, "queueportfolio/bulktimer").ConvertStringToInt(1000);
			_isVerboseEnabled = XMLUtility.GetTextFromAccountNode(Xml, "queueportfolio/verboselogging").ToLower() == "true";

			_bulkTimer.Elapsed += _bulkTimer_Elapsed;
			_bulkTimer.Interval = _bulkTimeLimit;
			_bulkTimer.Start();
		}


		public override void Process(IDroneDataComponent component)
		{
			PortfolioDataComponent smbComponent = component as PortfolioDataComponent;
			if (!Object.Equals(smbComponent, null))
			{
				AddPortfolioToBulk(smbComponent.PortfolioType);
			}
		}

		#region Database

		internal void AddPortfolioToBulk(PortfolioDataType portType)
		{
			try
			{
				lock (_lockObj)
				{
					lock (_bulkTableDict)
					{
						//check if it exists, if not, add table then add row
						if (!_bulkTableDict.ContainsKey(portType.TypeId))
						{
							DataTable bulkTable = new DataTable("DomainPortfolioType");
							bulkTable.Columns.Add("rptGDDomainsID", typeof(int));
							bulkTable.Columns.Add("TypeID", typeof(int));
							bulkTable.Columns.Add("Value", typeof(string));

							_bulkTableDict.Add(portType.TypeId, new PortfolioBulkData { LastCount = 0, BulkTable = bulkTable });
						}

						//Make sure row doesn't exist, then add it
						if (!_bulkTableDict[portType.TypeId].BulkTable.AsEnumerable().Any(row => row.Field<int>("rptGDDomainsID") == portType.rptGDDomainsId && row.Field<int>("TypeID") == portType.TypeId))
						{
							_bulkTableDict[portType.TypeId].BulkTable.Rows.Add(portType.rptGDDomainsId, portType.TypeId, (portType.Value != "None" ? portType.Value.Truncate(1000) : null));
						}
					}

					CheckBulkData(portType.TypeId);
				}
			}
			catch (Exception ex)
			{
				ExceptionExtensions.LogWarning(ex, "PortfolioBulkDataSource.AddPortfolioToBulk");
			}
		}

		#endregion

		#region Timer

		void _bulkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_isVerboseEnabled)
				Utility.WriteToLogFile(String.Format("PortfolioBulkDataSource_TimerElapsed{0:M_d_yyyy}", DateTime.Today) + ".log", "BulkTimer Elapsed, CheckBulkData Called - " + DateTime.Now);

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
						PortfolioBulkData bulkItem = _bulkTableDict[bulkItemType];
						if (bulkItem.BulkTable.Rows.Count != 0 && (bulkItem.LastCount >= _bulkThreshold || bulkItem.LastCount == bulkItem.BulkTable.Rows.Count))
						{
							//save to db
							if (_isVerboseEnabled)
								Utility.WriteToLogFile(String.Format("PortfolioBulkDataSource_Insert{0:M_d_yyyy}", DateTime.Today) + ".log"
									, String.Format("Type: {0}, LastCount: {1}, BulkTableCount: {2}, BulkTimeLimit: {3}, BulkThreshold: {4} - Time:{5}", bulkItemType, bulkItem.LastCount, bulkItem.BulkTable.Rows.Count, _bulkTimeLimit, _bulkThreshold, DateTime.Now));

							DataFactory.ExecuteNonQueryTable("PortfolioTableIns", bulkItem.BulkTable);

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
				ExceptionExtensions.LogWarning(ex, "PortfolioBulkDataSource.CheckBulkData");

				//if tempdb full or other critical db error, re-throw
				if (Utility.IsCriticalDBError(ex)) throw;
			}
			finally
			{
				if (_bulkTimer != null)
					_bulkTimer.Start();
				else
					ExceptionExtensions.LogInformation("PortfolioBulkDataSource.CheckBulkData", "_bulkTimer is null when calling start");
			}
		}

		#endregion

	}
}
