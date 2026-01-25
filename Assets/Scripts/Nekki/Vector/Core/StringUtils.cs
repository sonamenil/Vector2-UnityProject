using System;
using System.Text;

namespace Nekki.Vector.Core
{
	public static class StringUtils
	{
		public static string FromBase64(string p_baseString)
		{
			byte[] bytes = Convert.FromBase64String(p_baseString);
			return Encoding.UTF8.GetString(bytes);
		}

		public static T ParseEnum<T>(string p_str, T p_def)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), p_str, true);
			}
			catch
			{
				return p_def;
			}
		}
	}
}
