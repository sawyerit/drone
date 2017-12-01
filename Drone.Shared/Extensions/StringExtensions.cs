using System;
using System.IO;
namespace Drone.Shared
{
	public static class StringExtensions
	{
		public static int ConvertStringToInt(this string value, int defaultValue)
		{
			int outValue;
			if (int.TryParse(value, out outValue))
			{
				defaultValue = outValue;
			}

			return defaultValue;
		}

        public static long ConvertStringToLong(this string value, long defaultValue)
        {
            long outValue;
            if (long.TryParse(value, out outValue))
            {
                defaultValue = outValue;
            }

            return defaultValue;
        }

		public static byte[] ToByteArray(this Stream stream)
		{
			var syncStream = Stream.Synchronized(stream);
			using (MemoryStream memStream = new MemoryStream())
			{
				syncStream.Position = 0;
				syncStream.CopyTo(memStream);
				return memStream.ToArray();
			}
		}

		/// <summary>
		/// Returns the specified length of the string or the entire string if its smaller than the requested length
		/// </summary>
		/// <param name="length">length of string to return</param>
		/// <returns></returns>
		public static string Truncate(this string value, int length)
		{
			return value.Substring(0, value.Length <= length ? value.Length : length);
		}
	}
}
