using System;

namespace Drone.Shared
{
	public static class Conversions
	{

		/// <summary>
		/// Converts an object pulled from the Database into the proper datatype looking for DBNull values
		/// </summary>
		/// <example>
		/// Example: Utility.ConvertTo&lt;Decimal&gt;(resultTable.Rows[i]["DataColumn"], 0)
		/// returns the DataColumn value as a Decimal type, or the Decimal value 0 if DBNull is found
		/// </example>
		/// <typeparam name="T">The type of the value you are expecting to receive</typeparam>
		/// <param name="value">The object returned from the Database to be converted</param>
		/// <param name="defaultValue">The value that will be returned if a DBNull is encountered</param>
		/// <returns>The value object converted to the proper datatype if DBNull is not encountered and the defaultValue of DBNull is found</returns>
		public static T ConvertTo<T>(object value, T defaultValue)
		{
			bool foundNull;
			return ConvertTo(value, defaultValue, out foundNull);
		}

		/// <summary>
		/// Converts an object pulled from the Database into the proper datatype looking for DBNull values
		/// </summary>
		/// <example>
		/// Example: Utility.ConvertTo&lt;Decimal&gt;(resultTable.Rows[i]["DataColumn"], 0, out defaultUsed)
		/// Returns the DataColumn value as a Decimal type, or the Decimal value 0 if DBNull is found.
		/// The defaultUsed variable will be set to true if the DBNull was encountered, otherwise will be false
		/// </example>
		/// <typeparam name="T">The type of the value you are expecting to receive</typeparam>
		/// <param name="value">The object returned from the Database to be converted</param>
		/// <param name="defaultValue">The value that will be returned if a DBNull is encountered</param>
		/// <param name="foundNull">The OUT param that flags whether DBNull was encountered</param>
		/// <returns>The value object converted to the proper datatype if DBNull is not encountered and the defaultValue of DBNull is found</returns>
		public static T ConvertTo<T>(object value, T defaultValue, out bool foundNull)
		{
			T returnValue = defaultValue;
			foundNull = true;
			try
			{
				if (!Object.Equals(value, null) && value != DBNull.Value)
				{
					var conversionType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
					returnValue = (T)Convert.ChangeType(value, conversionType);
					foundNull = false;
				}
			}
			catch { /* Do Nothing */ }
			return returnValue;
		}
	}
}
