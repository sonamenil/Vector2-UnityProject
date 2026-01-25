using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	[ExecuteInEditMode]
	public class TextSizeHelper : MonoBehaviour
	{
		[Serializable]
		public class SizeData
		{
			public int MaxValue;

			public bool Enabled;

			public SizeData(int p_value, bool p_enabled = false)
			{
				MaxValue = p_value;
				Enabled = p_enabled;
			}
		}

		public Text Text;

		public LayoutElement LayoutElement;

		public SizeData Width;

		public SizeData Height;

		public int BaseFontSize = 50;

		public bool ResetFontSize = true;

		private void Awake()
		{
		}

		private void Update()
		{
			Refresh();
		}

		public void Refresh()
		{
			if (Text != null && LayoutElement != null)
			{
				if (ResetFontSize)
				{
					Text.fontSize = BaseFontSize;
					Canvas.ForceUpdateCanvases();
				}
				if (Width != null && Width.Enabled)
				{
					LayoutElement.preferredWidth = Mathf.Min(Text.preferredWidth, Width.MaxValue);
				}
				if (Height != null && Height.Enabled)
				{
					LayoutElement.preferredHeight = Mathf.Min(Text.preferredHeight, Height.MaxValue);
				}
			}
		}
	}
}
