using System.Collections.Generic;
using Nekki.Vector.Core.Generator;

namespace Nekki.Vector.Core.GameManagement
{
	public static class StringBuffer
	{
		private static Dictionary<string, string> _Buffer = new Dictionary<string, string>();

		private static Dictionary<string, List<string>> _MultiBuffer = new Dictionary<string, List<string>>();

		public static void AddString(string p_name, string p_value, bool p_isMultu = false)
		{
			if (p_isMultu)
			{
				if (!_MultiBuffer.ContainsKey(p_name))
				{
					_MultiBuffer.Add(p_name, new List<string>());
				}
				if (p_value == string.Empty)
				{
					_MultiBuffer[p_name].Clear();
				}
				else
				{
					_MultiBuffer[p_name].Add(p_value);
				}
			}
			else if (_Buffer.ContainsKey(p_name))
			{
				_Buffer[p_name] = p_value;
			}
			else
			{
				_Buffer.Add(p_name, p_value);
			}
		}

		public static string GetString(string p_name)
		{
			if (_MultiBuffer.ContainsKey(p_name))
			{
				return MainRandom.GetRandomFromList(_MultiBuffer[p_name]);
			}
			if (_Buffer.ContainsKey(p_name))
			{
				return _Buffer[p_name];
			}
			return string.Empty;
		}

		public static List<string> GetStrings(string p_name)
		{
			List<string> list = new List<string>();
			if (_MultiBuffer.ContainsKey(p_name))
			{
				list.AddRange(_MultiBuffer[p_name]);
			}
			if (_Buffer.ContainsKey(p_name))
			{
				list.Add(_Buffer[p_name]);
			}
			return list;
		}

		public static void Clear()
		{
			_Buffer.Clear();
			_MultiBuffer.Clear();
		}

		public static string Log()
		{
			if (_Buffer.Count == 0)
			{
				return "Empty";
			}
			string text = string.Empty;
			foreach (KeyValuePair<string, string> item in _Buffer)
			{
				string text2 = text;
				text = text2 + item.Key + ": " + item.Value + "\n";
			}
			return text;
		}
	}
}
