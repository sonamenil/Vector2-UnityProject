using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Utilites;
using UIFigures;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.ArchiveCategory
{
	public class CategoryButton : MonoBehaviour
	{
		[SerializeField]
		private Graphic _CategoryIcon;

		[SerializeField]
		private LabelAlias _CategoryName;

		[SerializeField]
		private Button _CategoryButton;

		[SerializeField]
		private UILine _Border;

		[SerializeField]
		private UIPoligon _Triangle;

		[SerializeField]
		private UIPoligon _BackgroundLeft;

		[SerializeField]
		private UIPoligon _BackgroundRight;

		[SerializeField]
		private GameObject _NewAnnounce;

		[SerializeField]
		private GameObject _LevelUpAnnounce;

		[SerializeField]
		private List<Vector2> _Points = new List<Vector2>();

		[SerializeField]
		private List<int> _LeftBgPointIndexes = new List<int>();

		[SerializeField]
		private List<int> _RightBgPointIndexes = new List<int>();

		private static Color _ActiveIconColor = ColorUtils.FromHex("5EA4BB");

		private static Color _InactiveIconColor = ColorUtils.FromHex("273E54");

		private static Color _ActiveTextColor = Color.white;

		private static Color _InactiveTextColor = ColorUtils.FromHex("325269");

		private SlotItem.Slot _SlotType;

		private bool _IsEnabled;

		public SlotItem.Slot SlotType
		{
			get
			{
				return _SlotType;
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
				_CategoryButton.interactable = _IsEnabled;
				if (_IsEnabled)
				{
					_CategoryIcon.color = _ActiveIconColor;
					_CategoryName.color = _ActiveTextColor;
				}
				else
				{
					_CategoryIcon.color = _InactiveIconColor;
					_CategoryName.color = _InactiveTextColor;
				}
			}
		}

		public void Init(SlotItem.Slot p_slotType, Action<CategoryButton> p_onClickCallback)
		{
			_SlotType = p_slotType;
			_CategoryButton.onClick.RemoveAllListeners();
			_CategoryButton.onClick.AddListener(delegate
			{
				p_onClickCallback(this);
			});
			Refresh();
		}

		private void Refresh()
		{
			_NewAnnounce.SetActive(false);
			_LevelUpAnnounce.SetActive(false);
			if (_SlotType == SlotItem.Slot.NotSlot)
			{
				IsEnabled = true;
				return;
			}
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
	}
}
