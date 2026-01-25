using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace DiffMatchPatch
{
	public class Patch
	{
		public List<Diff> diffs = new List<Diff>();

		public int start1;

		public int start2;

		public int length1;

		public int length2;

		public override string ToString()
		{
			string value = ((length1 == 0) ? (start1 + ",0") : ((length1 != 1) ? (start1 + 1 + "," + length1) : Convert.ToString(start1 + 1)));
			string value2 = ((length2 == 0) ? (start2 + ",0") : ((length2 != 1) ? (start2 + 1 + "," + length2) : Convert.ToString(start2 + 1)));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("@@ -").Append(value).Append(" +")
				.Append(value2)
				.Append(" @@\n");
			foreach (Diff diff in diffs)
			{
				switch (diff.operation)
				{
				case Operation.INSERT:
					stringBuilder.Append('+');
					break;
				case Operation.DELETE:
					stringBuilder.Append('-');
					break;
				case Operation.EQUAL:
					stringBuilder.Append(' ');
					break;
				}
				stringBuilder.Append(HttpUtility.UrlEncode(diff.text).Replace('+', ' ')).Append("\n");
			}
			return diff_match_patch.unescapeForEncodeUriCompatability(stringBuilder.ToString());
		}
	}
}
