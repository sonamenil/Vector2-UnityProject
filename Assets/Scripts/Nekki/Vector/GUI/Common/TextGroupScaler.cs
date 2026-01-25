using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	[ExecuteInEditMode]
	public class TextGroupScaler : MonoBehaviour
	{
		[Serializable]
		public class TextData
		{
			public Text Text;

			public LayoutElement Layout;
		}

		public List<TextData> Texts = new List<TextData>();

		public int CheckFontStep = 2;

		private int _OptimalFontSize;

		private void Awake()
		{
		}

		private void LateUpdate()
		{
			if (Texts.Count != 0)
			{
				Refresh();
			}
		}

		public void Refresh()
		{
			CalcOptimalFontSize();
			SetFontSizeToOptimal();
		}

		private void CalcOptimalFontSize()
		{
			_OptimalFontSize = int.MaxValue;
			foreach (TextData text in Texts)
			{
				if (text.Text != null && text.Layout != null)
				{
					_OptimalFontSize = Mathf.Min(_OptimalFontSize, GetOptimalFontSize(text));
				}
			}
		}

		private void SetFontSizeToOptimal()
		{
			foreach (TextData text in Texts)
			{
				if (text.Text != null)
				{
					text.Text.fontSize = _OptimalFontSize;
				}
			}
		}

		private int GetOptimalFontSize(TextData p_data)
		{
			while (p_data.Text.preferredWidth > p_data.Layout.preferredWidth)
			{
				p_data.Text.fontSize -= CheckFontStep;
				Canvas.ForceUpdateCanvases();
			}
			return p_data.Text.fontSize;
		}
	}
}
