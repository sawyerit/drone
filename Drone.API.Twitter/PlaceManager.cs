using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Entities.Twitter;
using Drone.Entities.Twitter.v11;

namespace Drone.API.Twitter
{
	public class PlaceManager : BaseManager
	{
		//Not used
		public List<Place> GetAvailablePlaces()
		{
			List<Place> twitterPlaces = new List<Place>();

			string requestText = string.Format("{0}/trends/available.json", ApiUrl); 
			twitterPlaces.AddRange(Request.Deserialize<List<Place>>(Request.ExecuteAnonymousWebRequest(requestText)));

			return twitterPlaces;
		}

		//Not used
		public List<List<TrendRoot>> GetTrendsForAllPlaces()
		{
			List<Place> twitterPlaces = GetAvailablePlaces();
			List<List<TrendRoot>> rootList = new List<List<TrendRoot>>();
			string requestText = string.Empty;

			foreach (var item in twitterPlaces)
			{
				requestText = string.Format("{1}/trends/{0}.json", item.woeid, ApiUrl);
				rootList.Add(Request.Deserialize<List<TrendRoot>>(Request.ExecuteAnonymousWebRequest(requestText)));
			}

			return rootList;
		}
				
		#region Yahoo WOEID calls (comments)

		//--- Rate limited, cached for 5 min (can't call anymore frequently than that)
		//Yahoo appid
		//appid=[MKErk5zV34Fxzx8Pp2Hp4GONIIVRe_YOibrhbnLXfFg6IJE4QY2VWcuYtdjEvVj6a3z96tszEY.N1oN_O2PYdB_mWE9s6zI-]

		//--countries
		//http://query.yahooapis.com/v1/public/yql?q=select+*+from+geo.countries

		//--States in US
		//http://query.yahooapis.com/v1/public/yql?q=select+*+from+geo.states+where+place='United+States'

		//--Counties in a state
		//http://query.yahooapis.com/v1/public/yql?q=select+*+from+geo.counties+where+place='Colorado'
		
		//--Zip to woeid?
		//http://query.yahooapis.com/v1/public/yql?q=select+*+from+geo.places+where+text='80111'+limit+1
		
		//--Available WOEIDs
		//https://api.twitter.com/1/trends/available.json

		#endregion
	}
}