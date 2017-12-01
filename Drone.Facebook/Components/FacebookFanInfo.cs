using System;
using System.ComponentModel.Composition;
using System.Threading;
using Drone.API.Facebook;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Facebook;
using Drone.Entities.WebAPI;
using Drone.Facebook.Datasources;
using Drone.Shared;

namespace Drone.Facebook.Components
{
	[Export(typeof(IDroneComponent))]
	public class FacebookFanInfo : BaseComponent<FacebookComponent>
	{
		#region Constructors

		[ImportingConstructor]
		public FacebookFanInfo()
			: base()
		{
			DroneDataSource = new FacebookDataSource();
		}

		public FacebookFanInfo(IDroneDataSource datasource)
			: base(datasource)
		{
			DroneDataSource = datasource;
		}

		#endregion

		public override void GetData(object context)
		{
			try
			{
				BaseContext cont = context as BaseContext;
				Context = cont;

				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);
					SetNextRunIntervalByNode(ProcessorName, cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						GetFanDemographics();
						GetPageInfoForAllCompetitors();
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "FacebookFanInfo.GetData()");
			}
		}


		#region internal

		internal void GetPageInfoForAllCompetitors()
		{
			WriteToUsageLogFile("FacebookFanInfo.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetPageInfoForAllCompetitors");
			
			FacebookDataSource fds = DroneDataSource as FacebookDataSource;
			if (!Object.Equals(null, fds))
			{
				foreach (Competitor item in fds.GetCompetitorAccounts())
				{
					if (item.FacebookID != 0)
					{
						Page fpo = GetPageInfoByCompany(item.FacebookID);
						if (!Object.Equals(fpo, null))
						{
							FacebookDataComponent fdc = new FacebookDataComponent();
							fdc.FBPage = fpo;
							DroneDataSource.Process(fdc);
						}
						else
						{
							Shared.Utility.WriteToLogFile(String.Format("Facebook_NoPage_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("{0},{1}", fpo.Name, fpo.Id));
						}
					}
				}
			}

			WriteToUsageLogFile("FacebookFanInfo.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetPageInfoForAllCompetitors");
		}

		internal void GetFanDemographics()
		{
			WriteToUsageLogFile("FacebookFanInfo.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetFanDemographics");

			Graph g = new Graph();
			FacebookDataComponent fdc;
			string accountId = XMLUtility.GetTextFromAccountNode(Xml, "id");
			string accessToken = XMLUtility.GetTextFromAccountNode(Xml, "accesstoken");

			Demographic<Country> country = g.GetFanDemographics<Demographic<Country>>(accountId, accessToken, "page_fans_country");
			if (!Object.Equals(country, null))
			{
				fdc = new FacebookDataComponent();
				fdc.CountryDemographic = country;
				DroneDataSource.Process(fdc);
			}

			Demographic<Locale> locale = g.GetFanDemographics<Demographic<Locale>>(accountId, accessToken, "page_fans_locale");
			if (!Object.Equals(locale, null))
			{
				fdc = new FacebookDataComponent();
				fdc.LocaleDemographic = locale;
				DroneDataSource.Process(fdc);
			}

			Demographic<Gender> gender = g.GetFanDemographics<Demographic<Gender>>(accountId, accessToken, "page_fans_gender_age");
			if (!Object.Equals(gender, null))
			{
				fdc = new FacebookDataComponent();
				fdc.GenderDemographic = gender;
				DroneDataSource.Process(fdc);
			}				

			WriteToUsageLogFile("FacebookFanInfo.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetFanDemographics");
		}


		#region Helpers

		internal static Page GetPageInfoByCompany(long companyID)
		{
			return new Graph().GetPageInfo(companyID);
		}

		#endregion

		#endregion
	}
}
