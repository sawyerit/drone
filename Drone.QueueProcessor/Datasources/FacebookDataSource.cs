using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.Facebook;
using Drone.QueueProcessor.Components;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class FacebookDataSource : BaseDatasource<QueueProcessorComponent>
	{
		public override void Process(IDroneDataComponent dataComponent)
		{
			FacebookDataComponent fbComponent = dataComponent as FacebookDataComponent;
			if (!Object.Equals(fbComponent, null))
			{
				if (!Object.Equals(fbComponent.FBPage, null))
					SaveLikes(fbComponent.FBPage);

				if (!Object.Equals(fbComponent.CountryDemographic, null))
					SaveCountryDemographics(fbComponent.CountryDemographic);

				if (!Object.Equals(fbComponent.LocaleDemographic, null))
					SaveLocaleDemographics(fbComponent.LocaleDemographic);

				if (!Object.Equals(fbComponent.GenderDemographic, null))
					SaveGenderDemographics(fbComponent.GenderDemographic);
			}
		}

		private void SaveLikes(Page facebookPageObject)
		{
			if (!Object.Equals(facebookPageObject, null))
			{
				try
				{
					if (!String.IsNullOrEmpty(facebookPageObject.Id))
					{
						DataFactory.ExecuteNonQuery("FacebookPageLikesInsert",
																				new KeyValuePair<string, object>("@facebookID", facebookPageObject.Id.ConvertStringToLong(0)),
																				new KeyValuePair<string, object>("@Likes", facebookPageObject.Likes),
																				new KeyValuePair<string, object>("@CreateDate", DateTime.Today.AddDays(-1)));
					}
					else
					{
						ExceptionExtensions.LogWarning(new ArgumentNullException("facebook_competitor")
																						, "FacebookDataSource.SaveLikes"
																						, string.Format("fb id: {0} - likes: {1}", facebookPageObject.Id, facebookPageObject.Likes));
					}
				}
				catch (Exception e)
				{
					if (e.Message.Contains("deadlocked"))
					{
						SaveLikes(facebookPageObject);
						ExceptionExtensions.LogInformation("FacebookDataSource.SaveLikes()", "Deadlock encountered, trying again");
					}
					else
					{
						ExceptionExtensions.LogError(e, "FacebookDataSource.SaveLikes", string.Format("fbUser: {0}", facebookPageObject.Id));
						
						//if tempdb full or other critical db error, re-throw
						if (Utility.IsCriticalDBError(e)) throw;
					}
				}
			}
		}

		private void SaveCountryDemographics(Demographic<Country> demographic)
		{
			try
			{
				if (IsValidDemographic<Country>(demographic) && !Object.Equals(demographic.Data[0].Days[0].Country, null))
				{

					foreach (var item in demographic.Data[0].Days)
					{
						foreach (PropertyInfo pi in typeof(Country).GetProperties())
						{
							DataFactory.ExecuteNonQuery("FacebookDemographicInsert",
																					new KeyValuePair<string, object>("@TypeID", 1),
																					new KeyValuePair<string, object>("@Code", pi.Name),
																					new KeyValuePair<string, object>("@Amount", (int)pi.GetValue(item.Country, null)),
																					new KeyValuePair<string, object>("@CreateDate", DateTime.ParseExact(item.End_Time, "yyyy-MM-ddTHH:mm:sszzzz", CultureInfo.InvariantCulture).ToUniversalTime().Date));
						}
					}
				}
				else
				{
					ExceptionExtensions.LogWarning(new ArgumentNullException("facebook_competitor"), "FacebookDataSource.SaveCountryDemographics", "Country demographic data missing");
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("deadlocked"))
				{
					SaveCountryDemographics(demographic);
					ExceptionExtensions.LogInformation("FacebookDataSource.SaveCountryDemographics()", "Deadlock encountered, trying again");
				}
				else
				{
					ExceptionExtensions.LogError(e, "FacebookDataSource.SaveCountryDemographics");

					//if tempdb full or other critical db error, re-throw
					if (Utility.IsCriticalDBError(e)) throw;
				}
			}
		}

		private void SaveLocaleDemographics(Demographic<Locale> demographic)
		{
			try
			{
				if (IsValidDemographic<Locale>(demographic) && !Object.Equals(demographic.Data[0].Days[0].Locale, null))
				{

					foreach (var item in demographic.Data[0].Days)
					{
						foreach (PropertyInfo pi in typeof(Locale).GetProperties())
						{
							DataFactory.ExecuteNonQuery("FacebookDemographicInsert",
																					new KeyValuePair<string, object>("@TypeID", 2),
																					new KeyValuePair<string, object>("@Code", pi.Name),
																					new KeyValuePair<string, object>("@Amount", (int)pi.GetValue(item.Locale, null)),
																					new KeyValuePair<string, object>("@CreateDate", DateTime.ParseExact(item.End_Time, "yyyy-MM-ddTHH:mm:sszzzz", CultureInfo.InvariantCulture).ToUniversalTime().Date));
						}
					}
				}
				else
				{
					ExceptionExtensions.LogWarning(new ArgumentNullException("facebook_competitor"), "FacebookDataSource.SaveLocaleDemographics", "Locale demographic data missing");
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("deadlocked"))
				{
					SaveLocaleDemographics(demographic);
					ExceptionExtensions.LogInformation("FacebookDataSource.SaveLocaleDemographics()", "Deadlock encountered, trying again");
				}
				else
				{
					ExceptionExtensions.LogError(e, "FacebookDataSource.SaveLocaleDemographics");

					//if tempdb full or other critical db error, re-throw
					if (Utility.IsCriticalDBError(e)) throw;
				}
			}
		}

		private void SaveGenderDemographics(Demographic<Gender> demographic)
		{
			try
			{
				if (IsValidDemographic<Gender>(demographic) && !Object.Equals(demographic.Data[0].Days[0].Gender, null))
				{
					foreach (var item in demographic.Data[0].Days)
					{
						foreach (PropertyInfo pi in typeof(Gender).GetProperties())
						{
							DataFactory.ExecuteNonQuery("FacebookDemographicInsert",
																					new KeyValuePair<string, object>("@TypeID", 3),
																					new KeyValuePair<string, object>("@Code", pi.Name),
																					new KeyValuePair<string, object>("@Amount", (int)pi.GetValue(item.Gender, null)),
																					new KeyValuePair<string, object>("@CreateDate", DateTime.ParseExact(item.End_Time, "yyyy-MM-ddTHH:mm:sszzzz", CultureInfo.InvariantCulture).ToUniversalTime().Date));
						}
					}
				}
				else
				{
					ExceptionExtensions.LogWarning(new ArgumentNullException("facebook_competitor"), "FacebookDataSource.SaveGenderDemographics", "Gender demographic data missing");
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("deadlocked"))
				{
					SaveGenderDemographics(demographic);
					ExceptionExtensions.LogInformation("FacebookDataSource.SaveGenderDemographics()", "Deadlock encountered, trying again");
				}
				else
				{
					ExceptionExtensions.LogError(e, "FacebookDataSource.SaveGenderDemographics");

					//if tempdb full or other critical db error, re-throw
					if (Utility.IsCriticalDBError(e)) throw;
				}
			}
		}


		private static bool IsValidDemographic<T>(Demographic<T> demographic)
		{
			return (!Object.Equals(demographic.Data, null) && demographic.Data.Count > 0 && !Object.Equals(demographic.Data[0].Days, null) && demographic.Data[0].Days.Count > 0);
		}
	}
}
