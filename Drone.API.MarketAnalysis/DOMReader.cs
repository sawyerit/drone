using System;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class DOMReader
	{
		private CookieCollection _requestCookies;
		private CookieCollection _responseCookies;
		private HtmlNodeCollection _atags;
		private HtmlNodeCollection _generatorTags;
		private HtmlNodeCollection _imageTags;
		private HtmlNodeCollection _javascriptTags;
		private HtmlNodeCollection _linkTags;
		private HtmlNodeCollection _metaTags;
		private HtmlNodeCollection _styleSheets;
		private HtmlNode _placeholder;
		private static int _maxPageByteSize;
		private static Encoding enc8 = Encoding.UTF8;

		#region public properties


		public HtmlNodeCollection ATags
		{
			get
			{
				if (Object.Equals(null, _atags))
				{
					_atags = Document.DocumentNode.SelectNodes("//a[@href]");

					if (Object.Equals(null, _atags))
						_atags = new HtmlNodeCollection(_placeholder);
				}
				return _atags;
			}
		}

		public HtmlDocument Document { get; set; }

		public HtmlNodeCollection GeneratorTags
		{
			get
			{
				if (Object.Equals(null, _generatorTags))
				{
					_generatorTags = Document.DocumentNode.SelectNodes("//meta[@name='generator']");

					if (Object.Equals(null, _generatorTags))
						_generatorTags = new HtmlNodeCollection(_placeholder);
				}
				return _generatorTags;
			}
		}

		public HtmlNodeCollection ImageTags
		{
			get
			{
				if (Object.Equals(null, _imageTags))
				{
					_imageTags = Document.DocumentNode.SelectNodes("//img[@src]");

					if (Object.Equals(null, _imageTags))
						_imageTags = new HtmlNodeCollection(_placeholder);
				}
				return _imageTags;
			}
		}

		public HtmlNodeCollection JavascriptTags
		{
			get
			{
				if (Object.Equals(null, _javascriptTags))
				{
					_javascriptTags = Document.DocumentNode.SelectNodes("//script[@src]");

					if (Object.Equals(null, _javascriptTags))
						_javascriptTags = new HtmlNodeCollection(_placeholder);
				}
				return _javascriptTags;
			}
		}

		public HtmlNodeCollection LinkTags
		{
			get
			{
				if (Object.Equals(null, _linkTags))
				{
					_linkTags = Document.DocumentNode.SelectNodes("//link[@href]");

					if (Object.Equals(null, _linkTags))
						_linkTags = new HtmlNodeCollection(_placeholder);
				}
				return _linkTags;
			}
		}

		public HtmlNodeCollection MetaTags
		{
			get
			{
				if (Object.Equals(null, _metaTags))
				{
					_metaTags = Document.DocumentNode.SelectNodes("//meta");

					if (Object.Equals(null, _metaTags))
						_metaTags = new HtmlNodeCollection(_placeholder);
				}
				return _metaTags;
			}
		}

		public HtmlNode ShortCutIcon
		{
			get
			{
				return Document.DocumentNode.SelectSingleNode("//link[@rel='shortcut icon']");
			}
		}

		public HtmlNodeCollection Stylesheets
		{
			get
			{
				if (Object.Equals(null, _styleSheets))
				{
					_styleSheets = Document.DocumentNode.SelectNodes("//link[@rel='stylesheet']");

					if (Object.Equals(null, _styleSheets))
						_styleSheets = new HtmlNodeCollection(_placeholder);
				}
				return _styleSheets;
			}
		}

		public String Domain { get; set; }

		public CookieCollection RequestCookies
		{
			get
			{
				return _requestCookies;
			}
			set {
				_requestCookies = value;
			}
		}

		public CookieCollection ResponseCookies
		{
			get
			{
				return _responseCookies;
			}
			set
			{
				_responseCookies = value;
			}
		}

		#endregion

		#region constructor

		/// <summary>
		/// Creates a new DOMReader and loads the html document
		/// </summary>
		/// <param name="domain">domain to load html document from (domain.com).  Do not include www</param>			
		public DOMReader(string domain, int pageByteSize)
		{
			_maxPageByteSize = pageByteSize;
			ReadDocument(domain);
			_placeholder = new HtmlNode(HtmlNodeType.Comment, Document, 0);
		}


		#endregion

		/// <summary>
		/// Find rule value in a collection of tag URLs
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="rule"></param>
		/// <returns></returns>
		public bool ExistsInCollection(HtmlNodeCollection tags, MarketShareRule rule)
		{
			foreach (var item in tags)
			{
				if (item.Attributes.Contains(rule.Property))
				{
					string propValue = item.Attributes[rule.Property].Value.ToLower();
					if (propValue.Length >= rule.Value.Length && propValue.Contains(rule.Value.ToLower()))
					{
						if (!rule.IgnoreAbsURI && IsAbsoluteUrl(propValue))
						{
							if (propValue.Contains(this.Domain))
								return true;
						}
						else
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		#region private methods


		private static bool IsAbsoluteUrl(string url)
		{
			Uri result;
			return Uri.TryCreate(url, UriKind.Absolute, out result);
		}

		/// <summary>
		/// Load the document
		/// </summary>
		/// <param name="domain"></param>
		private void ReadDocument(string domain)
		{
			Domain = domain;
			Document = new HtmlDocument();
			Document.OptionMaxNestedChildNodes = 5000;
			try
			{
				Document.LoadHtml(GetPage("http://www." + domain, out _requestCookies, out _responseCookies));

				if (!String.IsNullOrEmpty(Document.DocumentNode.InnerHtml))
					CheckForRefresh(Document, 0);
			}
			catch (Exception e)
			{
				Utility.WriteToLogFile(String.Format("MarketAnalysis_DOMReaderErrors{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain:{0} - Message:{1}", domain, e.Message));
			}
		}

		private void CheckForRefresh(HtmlDocument Document, int count)
		{
			if (count < 5)
			{
				//if its a meta refresh url, follow what it says
				HtmlNode refreshNode = null;
				foreach (HtmlNode item in MetaTags)
				{
					if (item.Attributes["http-equiv"] != null
						&& item.Attributes["http-equiv"].Value.ToLower() == "refresh"
						&& item.Attributes["content"].Value.ToLower().Contains("url"))
					{
						refreshNode = item;
						_metaTags = null;
						break;
					}
				}

				if (refreshNode != null)
				{
					string url = refreshNode.Attributes["content"].Value.Split('=')[1].Replace("'", string.Empty);
					Domain = IsAbsoluteUrl(url) ? url : Domain + url;

					Document.LoadHtml(GetPage(Domain, out _requestCookies, out _responseCookies));
					CheckForRefresh(Document, ++count);
				}
			}
		}

		/// <summary>
		/// Create request object and query url
		/// </summary>
		/// <param name="fullUrl">url to query</param>
		/// <returns></returns>
		private static string GetPage(string fullUrl, out CookieCollection requestCookies, out CookieCollection responseCookies)
		{
			string responseText = string.Empty;
			requestCookies = null;
			responseCookies = null;

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
			request.AllowAutoRedirect = true;
			request.Method = "GET";
			request.Timeout = 20000;
			request.KeepAlive = false;
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.94 Safari/537.4";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			request.Headers.Add("Cache-Control", "no-cache");
			request.Headers.Add("Pragma", "no-cache");
			request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
			request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				if (response.ContentType.Contains("text/html"))
				{
					responseText = BuildTextFromResponse(response);

					CookieContainer cookieJar = new CookieContainer();
					requestCookies = cookieJar.GetCookies(request.RequestUri);
					responseCookies = cookieJar.GetCookies(response.ResponseUri);
				}
			}

			return responseText;
		}

		private static string BuildTextFromResponse(HttpWebResponse response)
		{
			string responseText = string.Empty;

			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				using (Stream urlStream = response.GetResponseStream())
				{
					int read; int totalRead = 0;
					while (true)
					{
						read = urlStream.Read(buffer, 0, buffer.Length);
						totalRead += read;

						if (totalRead > _maxPageByteSize)
							break;

						if (read <= 0)
						{
							responseText = enc8.GetString(ms.ToArray());
							break;
						}
						else
							ms.Write(buffer, 0, read);
					}
				}
			}

			return responseText;
		}


		#endregion
	}
}

