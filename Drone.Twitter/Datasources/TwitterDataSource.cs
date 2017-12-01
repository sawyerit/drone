using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Shared;

namespace Drone.Twitter.Datasources
{
	public class TwitterDataSource : BaseDatasource<TwitterComponent>
	{
		public HttpWebRequest _requestGet;
		JavaScriptSerializer _jserializer = new JavaScriptSerializer();

		public override void Process(IDroneDataComponent component)
		{
			TwitterDataComponent twitterDataComponent = component as TwitterDataComponent;
			if (!Object.Equals(twitterDataComponent.KeywordStatus, null))
				SendAllMentions(twitterDataComponent.KeywordStatus);
		}


		#region Helpers
		
		protected void CreateGetRequest()
		{
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri") + "/keywords");

			_requestGet = (HttpWebRequest)WebRequest.Create(apiuri);
			_requestGet.Method = "GET";
			_requestGet.ContentType = "application/json";
			_requestGet.UseDefaultCredentials = true;
			_requestGet.PreAuthenticate = true;
		}

		private void SendAllMentions(KeywordStatus keywordStatus)
		{
			//CreatePostRequest
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri") + "/mentions");

			//send
			try
			{
				HttpStatusCode code = SendRequest(apiuri, keywordStatus, true);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "TwitterDataSource.SendAllMentions");
			}
		}

		#endregion


		#region Lookup Data

		internal List<Keyword> GetTweetQueries()
		{
			CreateGetRequest();
			string result = string.Empty;

			// Get response  
			using (HttpWebResponse response = _requestGet.GetResponse() as HttpWebResponse)
			{
				StreamReader reader = new StreamReader(response.GetResponseStream());
				result = reader.ReadToEnd();
			}

			List<Keyword> keywordList = new JavaScriptSerializer().Deserialize<List<Keyword>>(result);

			return keywordList;
		}

		#endregion
	}
}




//Country:.              Worldwide.             1         Supername
//Argentina.             Buenos Aires.          468739    Town
//Argentina.             Argentina.             23424747  Country
//Australia.             Australia.             23424748  Country
//Australia.             Sydney.                1105779   Town
//Brazil.                Rio de Janeiro.        455825    Town
//Brazil.                Brazil.                23424768  Country
//Brazil.                São Paulo.             455827    Town
//Canada.                Canada.                23424775  Country
//Canada.                Toronto.               4118      Town
//Canada.                Vancouver.             9807      Town
//Canada.                Montreal.              3534      Town
//Chile.                 Chile.                 23424782  Country
//Chile.                 Santiago.              349859    Town
//Colombia.              Colombia.              23424787  Country
//Colombia.              Bogotá.                368148    Town								       								        
//Ecuador.               Ecuador.               23424801  Country
//France.                France.                23424819  Country
//Germany.               Germany.               23424829  Country
//India.                 Mumbai.                2295411   Town
//India.                 India.                 23424848  Country
//Indonesia.             Bandung.               1047180   Town
//Indonesia.             Jakarta.               1047378   Town
//Indonesia.             Indonesia.             23424846  Country
//Ireland.               Dublin.                560743    Town
//Ireland.               Ireland.               23424803  Country
//Italy.                 Italy.                 23424853  Country
//Japan.                 Sapporo.               1118108   Town
//Japan.                 Kyoto.                 15015372  Town
//Japan.                 Fukuoka.               1117099   Town
//Japan.                 Japan.                 23424856  Country
//Japan.                 Osaka.                 15015370  Town
//Japan.                 Sendai.                1118129   Town
//Japan.                 Nagoya.                1117817   Town
//Japan.                 Tokyo.                 1118370   Town
//Malaysia.              Malaysia.              23424901  Country
//Malaysia.              Kuala Lumpur.          1154781   Town
//Mexico.                Mexico City.           116545    Town
//Mexico.                Monterrey.             134047    Town
//Mexico.                Mexico.                23424900  Country
//Netherlands.           Netherlands.           23424909  Country
//Netherlands.           Amsterdam.             727232    Town
//New Zealand.           New Zealand.           23424916  Country
//Nigeria.               Lagos.                 1398823   Town
//Nigeria.               Nigeria.               23424908  Country
//Peru.                  Peru.                  23424919  Country
//Peru.                  Lima.                  418440    Town
//Philippines.           Philippines.           23424934  Country
//Singapore.             Singapore.             23424948  Country
//South Africa.          South Africa.          23424942  Country
//South Africa.          Johannesburg.          1582504   Town
//Spain.                 Madrid.                766273    Town
//Spain.                 Spain.                 23424950  Country
//Sweden.                Sweden.                23424954  Country
//Turkey.                Turkey.                23424969  Country								       
//United Kingdom.        Glasgow.               21125     Town
//United Kingdom.        United Kingdom.        23424975  Country
//United Kingdom.        Manchester.            28218     Town
//United Kingdom.        Birmingham.            12723     Town
//United Kingdom.        London.                44418     Town
//United States.         New Haven.             2458410   Town
//United States.         Raleigh.               2478307   Town
//United States.         Harrisburg.            2418046   Town
//United States.         Sacramento.            2486340   Town
//United States.         Memphis.               2449323   Town
//United States.         Norfolk.               2460389   Town
//United States.         Atlanta.               2357024   Town
//United States.         San Antonio.           2487796   Town
//United States.         Boston.                2367105   Town
//United States.         San Francisco.         2487956   Town
//United States.         Indianapolis.          2427032   Town
//United States.         Las Vegas.             2436704   Town
//United States.         Baton Rouge.           2359991   Town
//United States.         Chicago.               2379574   Town
//United States.         Miami.                 2450022   Town
//United States.         United States.         23424977  Country
//United States.         Washington.            2514815   Town
//United States.         Dallas-Ft. Worth.      2388929   Town
//United States.         Los Angeles.           2442047   Town
//United States.         Denver.                2391279   Town
//United States.         Pittsburgh.            2473224   Town
//United States.         Salt Lake City.        2487610   Town
//United States.         St. Louis.             2486982   Town
//United States.         Cleveland.             2381475   Town
//United States.         Detroit.               2391585   Town
//United States.         Seattle.               2490383   Town
//United States.         Orlando.               2466256   Town
//United States.         Greensboro.            2414469   Town
//United States.         Columbus.              2383660   Town
//United States.         Richmond.              2480894   Town
//United States.         Houston.               2424766   Town
//United States.         Jackson.               2428184   Town
//United States.         New Orleans.           2458833   Town
//United States.         Minneapolis.           2452078   Town
//United States.         Tallahassee.           2503713   Town
//United States.         Birmingham.            2364559   Town
//United States.         Baltimore.             2358820   Town
//United States.         Portland.              2475687   Town
//United States.         New York.              2459115   Town
//United States.         Austin.                2357536   Town
//United States.         Philadelphia.          2471217   Town
//United States.         Milwaukee.             2451822   Town
//United States.         San Diego.             2487889   Town
//United States.         Tampa.                 2503863   Town
//United States.         Cincinnati.            2380358   Town
//United States.         Phoenix.               2471390   Town
//United States.         Charlotte.             2378426   Town
//United States.         Nashville.             2457170   Town
//United States.         Providence.            2477058   Town
//Venezuela.             Venezuela.             23424982  Country
//Venezuela.             Caracas.               395269    Town
//Venezuela.             Valencia.              395272    Town
//Venezuela.             Maracaibo.             395270    Town
//Venezuela.             Barquisimeto.          468382    Town
//United Arab Emirates.  United Arab Emirates.  23424738  Country
//Dominican Republic.    Santo Domingo.         76456     Town
//Dominican Republic.    Dominican Republic.    23424800  Country