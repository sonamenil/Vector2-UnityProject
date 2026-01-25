using System;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using UIFigures;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class GadgetUI : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _Container;

		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private UICircle _Background;

		[SerializeField]
		private GagetChargesUI _ChargesUI;

		[SerializeField]
		private Button _Button;

		private LayoutElement _LayoutElement;

		private Action<GadgetUI> _OnTap;

		private Color _GadgetFrameColor = Color.black;

		private GadgetItem _Gadget;

		private float _Width;

		private float _Height;

		private string _DefaultSpriteName;

		public Color GadgetFrameColor
		{
			get
			{
				return _GadgetFrameColor;
			}
			set
			{
				_GadgetFrameColor = value;
				_ChargesUI.FrameColor = value;
				Refresh();
			}
		}

		public GadgetItem Gadget
		{
			get
			{
				return _Gadget;
			}
		}

		public void Awake()
		{
			RectTransform component = GetComponent<RectTransform>();
			_Width = component.sizeDelta.x;
			_Height = component.sizeDelta.y;
			_LayoutElement = GetComponent<LayoutElement>();
		}

		public void Init(GadgetItem p_gadget, float p_width = -1f, float p_height = -1f, Action<GadgetUI> p_onTap = null, string p_defSpriteName = null)
		{
			_OnTap = p_onTap;
			_Gadget = p_gadget;
			_DefaultSpriteName = p_defSpriteName;
			_Width = ((p_width == -1f) ? _Width : p_width);
			_Height = ((p_height == -1f) ? _Height : p_height);
			Refresh();
		}

		public void SetTapEnable(bool p_value)
		{
			_Button.enabled = p_value;
		}

		public void Refresh()
		{
			_LayoutElement.preferredWidth = _Width;
			_LayoutElement.preferredHeight = _Height;
			SetGadgetImage();
			_ChargesUI.Init(_Gadget);
		}

		private void SetGadgetImage()
		{
			_Icon.SpriteName = ((_Gadget == null) ? _DefaultSpriteName : _Gadget.ItemImage);
		}

		public void RefreshCharges()
		{
			int currentCharges = _Gadget.CurrentCharges;
			int bonusCharges = _Gadget.BonusCharges;
			if (currentCharges != _ChargesUI.Segments || bonusCharges != _ChargesUI.BonusSegments)
			{
				_ChargesUI.ChangeAllSegments(currentCharges, bonusCharges);
			}
		}

		public void AddSegments(int p_segments)
		{
			_ChargesUI.ChangeAllSegments(_ChargesUI.Segments + p_segments, _ChargesUI.BonusSegments);
		}

		public void MoveContentTo(float p_x)
		{
			_Container.localPosition = new Vector3(p_x, 0f, 0f);
		}

		public void OnGadgetTap()
		{
			if (_OnTap != null)
			{
				AudioManager.PlaySound("select_button");
				_OnTap(this);
			}
		}

		public void OnSelect()
		{
			_ChargesUI.FrameColor = new Color(0.216f, 0.38f, 0.459f, 1f);
			_Background.color = new Color(0.012f, 0.059f, 0.094f, 0.75f);
			Refresh();
		}

		public void OnDeselect()
		{
			_ChargesUI.FrameColor = new Color(0.216f, 0.38f, 0.459f, 0.5f);
			_Background.color = new Color(0.012f, 0.059f, 0.094f, 0.25f);
			Refresh();
		}

		public void BlinkGadgetFrame(int p_loopcount, float p_duration)
		{
			float duration = p_duration / (float)(p_loopcount * 2);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_Container.DOScale(1.2f, duration));
			sequence.Append(_Container.DOScale(1f, duration));
			sequence.SetLoops(p_loopcount);
			sequence.Play();
		}
	}
}
