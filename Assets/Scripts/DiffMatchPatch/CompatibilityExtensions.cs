using System.Collections.Generic;

namespace DiffMatchPatch
{
	internal static class CompatibilityExtensions
	{
		public static List<T> Splice<T>(this List<T> input, int start, int count, params T[] objects)
		{
			List<T> range = input.GetRange(start, count);
			input.RemoveRange(start, count);
			input.InsertRange(start, objects);
			return range;
		}

		public static string JavaSubstring(this string s, int begin, int end)
		{
			return s.Substring(begin, end - begin);
		}
	}
}
