using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public static class DebugDataView
	{
		private static ScrollRect _ScrollView;

		private static Text _DataLabel;

		private static bool _IsInited;

		public static void Init(ScrollRect p_panel, Text p_label)
		{
			_IsInited = true;
			_ScrollView = p_panel;
			_DataLabel = p_label;
		}

		public static void Reset()
		{
			_IsInited = false;
			_ScrollView = null;
			_DataLabel = null;
		}

		public static void SetText(string p_string)
		{
			if (!_IsInited)
			{
				Debug.LogError("Please init DebugDataView");
				return;
			}
			if (p_string == _DataLabel.text)
			{
				p_string = string.Empty;
			}
			_ScrollView.content.localPosition = Vector2.zero;
			_DataLabel.text = p_string;
		}

		public static void SetEnable(bool p_value)
		{
			if (!_IsInited)
			{
				Debug.LogError("Please init DebugDataView");
			}
			else
			{
				_ScrollView.gameObject.SetActive(p_value);
			}
		}
	}
}
