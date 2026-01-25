using System;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Utilites;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Archive
{
	public class SlotButton : MonoBehaviour
	{
		public enum ButtonMode
		{
			Slot = 0,
			NextPage = 1,
			PrevPage = 2
		}

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private ResolutionImage _SlotIcon;

		[SerializeField]
		private Text _NotSlotText;

		[SerializeField]
		private Button _Button;

		[SerializeField]
		private GameObject _NewAnnounce;

		[SerializeField]
		private GameObject _LevelUpAnnounce;

		private static Color _ActiveIconColor = ColorUtils.FromHex("5EA4BBFF");

		private static Color _InactiveIconColor = ColorUtils.FromHex("273E54FF");

		private Action<SlotButton> _OnTapAction;

		private SlotItem.Slot _SlotType;

		private bool _IsEnabled;

		private ButtonMode _SlotButtonMode;

		public SlotItem.Slot SlotType
		{
			get
			{
				return _SlotType;
			}
			set
			{
				_SlotType = value;
				bool flag = _SlotType != SlotItem.Slot.NotSlot;
				if (flag)
				{
					_SlotIcon.SpriteName = _SlotType.GetIcon();
				}
				_SlotIcon.gameObject.SetActive(flag);
				_NotSlotText.gameObject.SetActive(!flag);
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
				ResolutionImage slotIcon = _SlotIcon;
				Color color = ((!_IsEnabled) ? _InactiveIconColor : _ActiveIconColor);
				_NotSlotText.color = color;
				slotIcon.color = color;
				_Button.interactable = _IsEnabled;
			}
		}

		public ButtonMode SlotButtonMode
		{
			get
			{
				return _SlotButtonMode;
			}
		}

		public void Init(SlotItem.Slot p_slotType, Action<SlotButton> p_onTap)
		{
			_OnTapAction = p_onTap;
			_SlotButtonMode = ButtonMode.Slot;
			SlotType = p_slotType;
			Refresh();
		}

		public void InitNext(Action<SlotButton> p_onTap)
		{
			_OnTapAction = p_onTap;
			_SlotButtonMode = ButtonMode.NextPage;
			SlotType = SlotItem.Slot.NotSlot;
			Refresh();
		}

		public void InitPrev(Action<SlotButton> p_onTap)
		{
			_OnTapAction = p_onTap;
			_SlotButtonMode = ButtonMode.PrevPage;
			SlotType = SlotItem.Slot.NotSlot;
			Refresh();
		}

		public void Refresh()
		{
			_NewAnnounce.SetActive(false);
			_LevelUpAnnounce.SetActive(false);
			if (_SlotButtonMode == ButtonMode.Slot)
			{
				SlotItem slot = DataLocalHelper.GetSlot(_SlotType);
				IsEnabled = slot != null && slot.Cards.Count > 0;
				if (slot.HasLevelUpCards)
				{
					_LevelUpAnnounce.SetActive(true);
				}
				else if (slot.HasNewCards)
				{
					_NewAnnounce.SetActive(true);
				}
			}
			else
			{
				IsEnabled = true;
			}
		}

		public void MoveLeft(float p_duration)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(DOTween.To(() => _Content.offsetMin.x, delegate(float x)
			{
				_Content.offsetMin = new Vector2(x, _Content.offsetMin.y);
			}, 0f, p_duration));
			sequence.Play();
		}

		public void MoveRight(float p_duration)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(DOTween.To(() => _Content.offsetMin.x, delegate(float x)
			{
				_Content.offsetMin = new Vector2(x, _Content.offsetMin.y);
			}, 155f, p_duration));
			sequence.Play();
		}

		public void OnTap()
		{
			if (_OnTapAction != null)
			{
				_OnTapAction(this);
			}
		}
	}
}
