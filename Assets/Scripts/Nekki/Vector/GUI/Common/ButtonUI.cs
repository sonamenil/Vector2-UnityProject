using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Utilites;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	[ExecuteInEditMode]
	public class ButtonUI : MonoBehaviour
	{
		public enum Type
		{
			Red = 0,
			Green = 1,
			Blue = 2,
			Grey = 3
		}

		[SerializeField]
		private Image _Backdround;

		[SerializeField]
		private LabelAlias _ButtonText;

		[SerializeField]
		private GameObject _PaidPlate;

		[SerializeField]
		private ResolutionImage _PaidIcon;

		[SerializeField]
		private LabelAlias _PaidCount;

		[SerializeField]
		private GameObject _FreeSpace;

		[SerializeField]
		private Button _Button;

		[SerializeField]
		private List<LayoutElement> _PaidPlateSpaces;

		private bool _FlexiblePaidSize;

		private string _LastPaidValue;

		private float _PaidSpacesWidth;

		public Image Backdround
		{
			get
			{
				return _Backdround;
			}
		}

		public LabelAlias ButtonText
		{
			get
			{
				return _ButtonText;
			}
		}

		public Image PaidPlate
		{
			get
			{
				return _PaidPlate.GetComponent<Image>();
			}
		}

		public ResolutionImage PaidIcon
		{
			get
			{
				return _PaidIcon;
			}
		}

		public LabelAlias PaidCount
		{
			get
			{
				return _PaidCount;
			}
		}

		public bool IsPaid
		{
			get
			{
				return _PaidPlate.activeSelf;
			}
		}

		public Button Button
		{
			get
			{
				return _Button;
			}
		}

		private bool IsFlexibleContentDirty
		{
			get
			{
				return _LastPaidValue != _PaidCount.text;
			}
		}

		public void SetType(Type p_type, bool changeOpacity = true, string soundAlias = null)
		{
			float num = 1f;
			if (!changeOpacity)
			{
				num = _Backdround.color.a;
			}
			switch (p_type)
			{
			case Type.Blue:
				_Backdround.color = new Color(0.234f, 0.375f, 0.441f, num);
				SetSound(soundAlias, "blue_button");
				break;
			case Type.Green:
				_Backdround.color = new Color(0.234f, 0.398f, 0.3125f, num);
				SetSound(soundAlias, "blue_button");
				break;
			case Type.Red:
				_Backdround.color = new Color(0.376f, 0.192f, 0.212f, num);
				SetSound(soundAlias, "red_button");
				break;
			case Type.Grey:
				_Backdround.color = ColorUtils.FromHex("515151", num);
				SetSound(soundAlias, "grey_button");
				break;
			}
		}

		private void SetSound(string p_soundAlias, string p_defaultSoundAlias)
		{
			ButtonSoundClick componentInChildren = GetComponentInChildren<ButtonSoundClick>();
			if (p_soundAlias == null)
			{
				p_soundAlias = p_defaultSoundAlias;
			}
			if (componentInChildren != null)
			{
				componentInChildren.SoundAlias = p_soundAlias;
			}
		}

		public void TurnToPaid()
		{
			_FreeSpace.SetActive(true);
			_PaidPlate.SetActive(true);
			if (_FlexiblePaidSize)
			{
				LayoutElement component = _ButtonText.GetComponent<LayoutElement>();
				component.flexibleWidth = -1f;
				ResetFlexibleContentDirty();
				ResizeContent();
			}
		}

		public void TurnToFree()
		{
			_FreeSpace.SetActive(false);
			_PaidPlate.SetActive(false);
			if (_FlexiblePaidSize)
			{
				LayoutElement component = _ButtonText.GetComponent<LayoutElement>();
				component.flexibleWidth = 1f;
				component.preferredWidth = -1f;
			}
		}

		private void Awake()
		{
			CalculatePaidSpacesWidth();
		}

		private void Update()
		{
			if (_FlexiblePaidSize && IsPaid && IsFlexibleContentDirty)
			{
				ResetFlexibleContentDirty();
				ResizeContent();
			}
		}

		private void ResetFlexibleContentDirty()
		{
			_LastPaidValue = _PaidCount.text;
		}

		private void ResizeContent()
		{
			bool flag = string.IsNullOrEmpty(_PaidCount.text);
			float actualPaidSpacesWidth = GetActualPaidSpacesWidth(flag);
			_PaidPlateSpaces[1].gameObject.SetActive(!flag);
			float num = _PaidIcon.GetComponent<LayoutElement>().minWidth + _PaidCount.preferredWidth + actualPaidSpacesWidth;
			_PaidPlate.GetComponent<LayoutElement>().preferredWidth = num;
			float b = GetComponent<RectTransform>().sizeDelta.x - (num + _FreeSpace.GetComponent<LayoutElement>().preferredWidth);
			_ButtonText.GetComponent<LayoutElement>().preferredWidth = Mathf.Max(0f, b);
		}

		private void CalculatePaidSpacesWidth()
		{
			if (_PaidPlateSpaces == null || _PaidPlateSpaces.Count == 0)
			{
				return;
			}
			_PaidSpacesWidth = 0f;
			foreach (LayoutElement paidPlateSpace in _PaidPlateSpaces)
			{
				_PaidSpacesWidth += paidPlateSpace.minWidth;
			}
			_FlexiblePaidSize = true;
		}

		private float GetActualPaidSpacesWidth(bool p_isTextEmpty)
		{
			float num = _PaidSpacesWidth;
			if (p_isTextEmpty)
			{
				num -= _PaidPlateSpaces[1].minWidth;
			}
			return num;
		}

		public void Fade(float p_time)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_Backdround.DOFade(0f, p_time));
			sequence.Join(_PaidCount.DOFade(0f, p_time));
			sequence.Join(_ButtonText.DOFade(0f, p_time));
			sequence.Join(_PaidIcon.DOFade(0f, p_time));
			sequence.Join(_PaidPlate.GetComponent<Image>().DOFade(0f, p_time));
			sequence.Play();
		}

		public void ResetAlpha()
		{
			_Backdround.color = new Color(_Backdround.color.r, _Backdround.color.g, _Backdround.color.b, 1f);
			_PaidCount.color = new Color(_PaidCount.color.r, _PaidCount.color.g, _PaidCount.color.b, 1f);
			_ButtonText.color = new Color(_ButtonText.color.r, _ButtonText.color.g, _ButtonText.color.b, 1f);
			_PaidIcon.color = new Color(_PaidIcon.color.r, _PaidIcon.color.g, _PaidIcon.color.b, 1f);
			_PaidPlate.GetComponent<Image>().color = new Color(_PaidPlate.GetComponent<Image>().color.r, _PaidPlate.GetComponent<Image>().color.g, _PaidPlate.GetComponent<Image>().color.b, 1f);
		}
	}
}
