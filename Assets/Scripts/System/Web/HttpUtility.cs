using System.Text;
using System.Text.RegularExpressions;

namespace System.Web
{
	public static class HttpUtility
	{
		private const int _StrLimit = 32766;

		private static char[] base36CharArray = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

		private static string base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

		public static string UrlEncode(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			if (text.Length < 32766)
			{
				return Uri.EscapeDataString(text);
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int i = 32766;
			for (int length = text.Length; i < length; i += 32766)
			{
				stringBuilder.Append(Uri.EscapeDataString(text.Substring(num, 32766)));
				num = i;
			}
			if (num < text.Length)
			{
				stringBuilder.Append(Uri.EscapeDataString(text.Substring(num)));
			}
			return stringBuilder.ToString();
		}

		public static string UrlEncodePathSafe(string text)
		{
			string text2 = UrlEncode(text);
			return text2.Replace(".", "%2E").Replace("#", "%23");
		}

		public static string UrlDecode(string text)
		{
			text = text.Replace("+", " ");
			return Uri.UnescapeDataString(text);
		}

		public static string GetUrlEncodedKey(string urlEncoded, string key)
		{
			urlEncoded = "&" + urlEncoded + "&";
			int num = urlEncoded.IndexOf("&" + key + "=", StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				return string.Empty;
			}
			int num2 = num + 2 + key.Length;
			int num3 = urlEncoded.IndexOf("&", num2);
			if (num3 < 0)
			{
				return string.Empty;
			}
			return UrlDecode(urlEncoded.Substring(num2, num3 - num2));
		}

		public static string SetUrlEncodedKey(string urlEncoded, string key, string value)
		{
			if (!urlEncoded.EndsWith("?") && !urlEncoded.EndsWith("&"))
			{
				urlEncoded += "&";
			}
			Match match = Regex.Match(urlEncoded, "[?|&]" + key + "=.*?&");
			urlEncoded = ((match != null && !string.IsNullOrEmpty(match.Value)) ? urlEncoded.Replace(match.Value, match.Value.Substring(0, 1) + key + "=" + UrlEncode(value) + "&") : (urlEncoded + key + "=" + UrlEncode(value) + "&"));
			return urlEncoded.TrimEnd('&');
		}

		public static string Base36Encode(long value)
		{
			string text = string.Empty;
			bool flag = value < 0;
			if (flag)
			{
				value *= -1;
			}
			do
			{
				text = base36CharArray[value % base36CharArray.Length] + text;
				value /= 36;
			}
			while (value != 0L);
			return (!flag) ? text : (text + "-");
		}

		public static long Base36Decode(string input)
		{
			bool flag = false;
			if (input.EndsWith("-"))
			{
				flag = true;
				input = input.Substring(0, input.Length - 1);
			}
			char[] array = input.ToCharArray();
			Array.Reverse(array);
			long num = 0L;
			for (long num2 = 0L; num2 < array.Length; num2++)
			{
				long num3 = base36Chars.IndexOf(array[num2]);
				num += Convert.ToInt64((double)num3 * Math.Pow(36.0, num2));
			}
			return (!flag) ? num : (num * -1);
		}
	}
}
