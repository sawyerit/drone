using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Drone.Entities.Facebook;

namespace Drone.API.Facebook
{
	public class DemographicJavaScriptConverter : JavaScriptConverter
	{
		private static readonly Type[] _supportedTypes = new[]
    {
        typeof( Gender ),
				typeof( Daily<Gender> ),
				typeof( DemographicData<Gender> ),
				typeof( Demographic<Gender> ),
				typeof( Daily<Country> ),
				typeof( DemographicData<Country> ),
				typeof( Demographic<Country> ),
				typeof( Daily<Locale> ),
				typeof( DemographicData<Locale> ),
				typeof( Demographic<Locale> )
    };

		public override IEnumerable<Type> SupportedTypes
		{
			get { return _supportedTypes; }
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			if (type == typeof(Demographic<Gender>))
			{
				var obj = new Demographic<Gender>();
				if (dictionary.ContainsKey("data"))
					obj.Data = serializer.ConvertToType<List<DemographicData<Gender>>>(dictionary["data"]);

				return obj;
			}
			else if (type == typeof(Demographic<Country>))
			{
				var obj = new Demographic<Country>();
				if (dictionary.ContainsKey("data"))
					obj.Data = serializer.ConvertToType<List<DemographicData<Country>>>(dictionary["data"]);

				return obj;
			}
			else if (type == typeof(Demographic<Locale>))
			{
				var obj = new Demographic<Locale>();
				if (dictionary.ContainsKey("data"))
					obj.Data = serializer.ConvertToType<List<DemographicData<Locale>>>(dictionary["data"]);

				return obj;
			}
			else if (type == typeof(DemographicData<Gender>))
			{
				var obj = new DemographicData<Gender>();
				if (dictionary.ContainsKey("id"))
					obj.id = serializer.ConvertToType<string>(dictionary["id"]);

				if (dictionary.ContainsKey("name"))
					obj.Name = serializer.ConvertToType<string>(dictionary["name"]);

				if (dictionary.ContainsKey("values"))
					obj.Days = serializer.ConvertToType<List<Daily<Gender>>>(dictionary["values"]);

				return obj;
			}
			else if (type == typeof(DemographicData<Country>))
			{
				var obj = new DemographicData<Country>();
				if (dictionary.ContainsKey("id"))
					obj.id = serializer.ConvertToType<string>(dictionary["id"]);

				if (dictionary.ContainsKey("name"))
					obj.Name = serializer.ConvertToType<string>(dictionary["name"]);

				if (dictionary.ContainsKey("values"))
					obj.Days = serializer.ConvertToType<List<Daily<Country>>>(dictionary["values"]);

				return obj;
			}
			else if (type == typeof(DemographicData<Locale>))
			{
				var obj = new DemographicData<Locale>();
				if (dictionary.ContainsKey("id"))
					obj.id = serializer.ConvertToType<string>(dictionary["id"]);

				if (dictionary.ContainsKey("name"))
					obj.Name = serializer.ConvertToType<string>(dictionary["name"]);

				if (dictionary.ContainsKey("values"))
					obj.Days = serializer.ConvertToType<List<Daily<Locale>>>(dictionary["values"]);

				return obj;
			}
			else if (type == typeof(Daily<Gender>))
			{
				var obj = new Daily<Gender>();
				if (dictionary.ContainsKey("end_time"))
					obj.End_Time = serializer.ConvertToType<string>(dictionary["end_time"]);

				if (dictionary.ContainsKey("value"))
					obj.Gender = serializer.ConvertToType<Gender>(dictionary["value"]);

				return obj;
			}
			else if (type == typeof(Daily<Country>))
			{
				var obj = new Daily<Country>();
				if (dictionary.ContainsKey("end_time"))
					obj.End_Time = serializer.ConvertToType<string>(dictionary["end_time"]);

				if (dictionary.ContainsKey("value"))
					obj.Country = serializer.ConvertToType<Country>(dictionary["value"]);

				return obj;
			}
			else if (type == typeof(Daily<Locale>))
			{
				var obj = new Daily<Locale>();
				if (dictionary.ContainsKey("end_time"))
					obj.End_Time = serializer.ConvertToType<string>(dictionary["end_time"]);

				if (dictionary.ContainsKey("value"))
					obj.Locale = serializer.ConvertToType<Locale>(dictionary["value"]);

				return obj;
			}
			else if (type == typeof(Gender))
			{
				var obj = new Gender();

				obj.M_25to34 = SetValue(serializer, dictionary, "M.25-34");
				obj.M_18to24 = SetValue(serializer, dictionary, "M.18-24");
				obj.M_45to54 = SetValue(serializer, dictionary, "M.45-54");
				obj.F_25to34 = SetValue(serializer, dictionary, "F.25-34");
				obj.F_35to44 = SetValue(serializer, dictionary, "F.35-44");
				obj.F_45to54 = SetValue(serializer, dictionary, "F.45-54");
				obj.F_18to24 = SetValue(serializer, dictionary, "F.18-24");
				obj.M_55to64 = SetValue(serializer, dictionary, "M.55-64");
				obj.M_13to17 = SetValue(serializer, dictionary, "M.13-17");
				obj.F_55to64 = SetValue(serializer, dictionary, "F.55-64");
				obj.M_65to = SetValue(serializer, dictionary, "M.65+");
				obj.F_13to17 = SetValue(serializer, dictionary, "F.13-17");
				obj.F_65to = SetValue(serializer, dictionary, "F.65+");
				obj.U_25to34 = SetValue(serializer, dictionary, "U.25-34");
				obj.U_35to44 = SetValue(serializer, dictionary, "U.35-44");
				obj.U_45to54 = SetValue(serializer, dictionary, "U.45-54");
				obj.U_18to24 = SetValue(serializer, dictionary, "U.18-24");
				obj.U_55to64 = SetValue(serializer, dictionary, "U.55-64");
				obj.U_65to = SetValue(serializer, dictionary, "U.65to+");

				return obj;
			}

			return null;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			var dataObj = obj as Gender;
			if (dataObj != null)
			{
				return new Dictionary<string, object>
            {
                {"M.25-34", dataObj.M_25to34 },
								{"M.18-24", dataObj.M_18to24 }
            };
			}
			return new Dictionary<string, object>();
		}

		private static int SetValue(JavaScriptSerializer serializer, IDictionary<string, object> dictionary, string field)
		{
			if (dictionary.ContainsKey(field))
				return serializer.ConvertToType<int>(dictionary[field]);

			return 0;
		}
	}
}
