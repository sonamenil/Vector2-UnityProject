using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Utilites;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class StatusEffect : MonoBehaviour
	{
		public enum FillSpriteType
		{
			None = 0,
			Normal = 1,
			Reversed = 2
		}

		private const float _IconOnBotOpacity = 0.3f;

		private const float _IconSumOpacity = 0.7f;

		private static float _IconOnTopOpacity = 0.57142854f;

		[SerializeField]
		private LayoutElement _LayoutElement;

		[SerializeField]
		private CanvasGroup _CanvasGroup;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private ResolutionImage _Icon_1;

		[SerializeField]
		private ResolutionImage _Icon_2;

		[SerializeField]
		private Text _Multiplicator;

		private int _Frames;

		private int _DelayFrames;

		private int _FillFramesDelta;

		private FillSpriteType _FillSpriteType;

		private bool _IsPermantent;

		private int _Quantity;

		private string _CounterName;

		private int _CounterValue;

		private int _ResetCounterFrame = -1;

		private List<KeyValuePair<int, int>> _FramesQuantity = new List<KeyValuePair<int, int>>();

		private Sequence _Sequence;

		public LayoutElement ElementLayout
		{
			get
			{
				return _LayoutElement;
			}
		}

		public CanvasGroup GroupCanvas
		{
			get
			{
				return _CanvasGroup;
			}
		}

		public int Frames
		{
			get
			{
				return _Frames;
			}
		}

		public int DelayFrames
		{
			get
			{
				return _DelayFrames;
			}
		}

		public bool IsFillSprite
		{
			get
			{
				return _FillSpriteType != FillSpriteType.None;
			}
		}

		public bool IsPermantent
		{
			get
			{
				return _IsPermantent;
			}
		}

		public int Quantity
		{
			get
			{
				return _Quantity;
			}
		}

		public bool IsOver
		{
			get
			{
				return _DelayFrames != -1 && _Frames >= _DelayFrames;
			}
		}

		public string CounterName
		{
			get
			{
				return _CounterName;
			}
			set
			{
				_CounterName = value;
			}
		}

		public int CounterValue
		{
			get
			{
				return _CounterValue;
			}
			set
			{
				_CounterValue = value;
			}
		}

		public bool ResetCounterValueOnNextFrame
		{
			set
			{
				if (value)
				{
					_ResetCounterFrame = _Frames + 1;
				}
			}
		}

		public bool ResetCounterValueOnLastFrame
		{
			set
			{
				if (value)
				{
					_ResetCounterFrame = _DelayFrames - 1;
				}
			}
		}

		public void Init(string p_image, string p_color, int p_DelayFrames, FillSpriteType p_fillSpriteType = FillSpriteType.None, bool p_isPermantent = false)
		{
			_DelayFrames = p_DelayFrames;
			_FillSpriteType = p_fillSpriteType;
			_IsPermantent = p_isPermantent;
			_Icon_2.SpriteName = p_image;
			_Icon_2.color = GetColor(p_color);
			if (IsFillSprite)
			{
				_Icon_1.SpriteName = p_image;
				_Icon_1.color = GetColor(p_color, true);
			}
			else
			{
				_Icon_1.gameObject.SetActive(false);
			}
			_CanvasGroup.alpha = 1f;
		}

		public void SetQuantity(int p_value, string p_stackType = "Subs")
		{
			switch (p_stackType)
			{
			case "Ignore":
				break;
			case "Subs":
				_Quantity = p_value;
				SetMultiplicatorText(_Quantity);
				_FramesQuantity.Clear();
				_FramesQuantity.Add(new KeyValuePair<int, int>(_DelayFrames, _Quantity));
				break;
			case "Add":
				if (_CounterValue == 0)
				{
					_Quantity = p_value;
				}
				else
				{
					_Quantity += p_value;
				}
				SetMultiplicatorText(_Quantity);
				_FramesQuantity.Clear();
				_FramesQuantity.Add(new KeyValuePair<int, int>(_DelayFrames, _Quantity));
				break;
			case "AddSeparate":
				_Quantity += p_value;
				SetMultiplicatorText(_Quantity);
				_FramesQuantity.Add(new KeyValuePair<int, int>(_DelayFrames, p_value));
				break;
			}
		}

		public void SetMultiplicatorText(int p_value)
		{
			_Quantity = p_value;
			if (p_value <= 1)
			{
				_Multiplicator.gameObject.SetActive(false);
				return;
			}
			_Multiplicator.gameObject.SetActive(true);
			_Multiplicator.text = "x" + p_value;
		}

		public void Render()
		{
			if (_DelayFrames == -1)
			{
				return;
			}
			_Frames++;
			if (_Frames == _ResetCounterFrame)
			{
				if (_FillSpriteType == FillSpriteType.Reversed)
				{
					Pulse();
				}
				_ResetCounterFrame = -1;
				_CounterValue = 0;
			}
			if (IsFillSprite)
			{
				if (_FillSpriteType == FillSpriteType.Reversed)
				{
					_Icon_2.fillAmount = (float)(_Frames - _FillFramesDelta) / (float)(_DelayFrames - _FillFramesDelta);
				}
				else
				{
					_Icon_2.fillAmount = 1f - (float)(_Frames - _FillFramesDelta) / (float)(_DelayFrames - _FillFramesDelta);
				}
			}
			if (_FramesQuantity.Count > 1 && _Frames >= _FramesQuantity[0].Key)
			{
				RecalcQuantity();
			}
		}

		private void RecalcQuantity()
		{
			for (int num = _FramesQuantity.Count - 1; num >= 0; num--)
			{
				if (_Frames >= _FramesQuantity[num].Key)
				{
					_FramesQuantity.RemoveAt(num);
				}
			}
			int num2 = 0;
			for (int i = 0; i < _FramesQuantity.Count; i++)
			{
				num2 += _FramesQuantity[i].Value;
			}
			SetMultiplicatorText(num2);
		}

		public void ResetFrames()
		{
			_Frames = 0;
		}

		public void ResetFillAmount()
		{
			_Icon_2.fillAmount = 1f;
			_FillFramesDelta = _Frames;
		}

		private Color GetColor(string p_color, bool p_transparent = false)
		{
			float num = 0f;
			num = (p_transparent ? 0.3f : ((!IsFillSprite) ? 0.7f : _IconOnTopOpacity));
			switch (p_color)
			{
			case "blue":
				return ColorUtils.FromHex("51B3DD", num);
			case "yellow":
				return ColorUtils.FromHex("FF7E1C", num);
			case "red":
				return ColorUtils.FromHex("D61418", num);
			default:
				return new Color(0f, 0f, 0f, num);
			}
		}

		public void ResetPulse()
		{
			if (_Sequence != null)
			{
				_Sequence.Complete(true);
				_Sequence = null;
			}
		}

		public void Pulse()
		{
			ResetPulse();
			_Sequence = DOTween.Sequence();
			_Sequence.Append(_Icon_1.rectTransform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 2));
			_Sequence.Join(_Icon_2.rectTransform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 2));
			_Sequence.Play();
		}
	}
}
