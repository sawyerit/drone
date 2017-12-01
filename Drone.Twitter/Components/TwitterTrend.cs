using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Drone.API.Twitter;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Shared;
using Drone.Twitter.Datasources;

namespace Drone.Twitter.Components
{
	[Export(typeof(IDroneComponent))]
	public class TwitterTrend : BaseComponent<TwitterComponent>
	{
		#region public properties

		//public List<List<TrendRoot>> TrendRootList { get; set; }
		internal TwitterDataComponent _dataComponent;

		#endregion

		#region constructors
		
		[ImportingConstructor]
		public TwitterTrend()
			: base()
		{
			DroneDataSource = new TwitterTrendDataSource();
			DroneDataComponent = new TwitterDataComponent();
		}

		public TwitterTrend(IDroneDataSource datasource)
			: base(datasource)
		{
			DroneDataSource = datasource;
			DroneDataComponent = new TwitterDataComponent();
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

				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);
					SetNextRunIntervalByNode(ProcessorName, cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						//do work						
						WriteToUsageLogFile("TwitterTrend.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetTwitterTrendsAllPlaces");

						TwitterDataComponent _dataComponent = DroneDataComponent as TwitterDataComponent;
						_dataComponent.TrendRootList = GetTwitterTrendsAllPlaces();
						//DroneDataSource.Process(_dataComponent);
						//add via webapi call if this is needed again

						WriteToUsageLogFile("TwitterTrend.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetTwitterTrendsAllPlaces");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.Twitter.Components.TwitterTrend.GetData()");
			}
		}

		#region internal methods

		/// <summary>
		/// Gets top 10 trends for all available WoeIDs
		/// </summary>
		internal static List<List<TrendRoot>> GetTwitterTrendsAllPlaces()
		{
			return new PlaceManager().GetTrendsForAllPlaces();
		}

		#endregion
	}
}
