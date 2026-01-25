using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Scenes.Terminal;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class ItemInfoPanel : MonoBehaviour
	{
		[SerializeField]
		private int _ItemRewardSize = 270;

		[SerializeField]
		private GameObject _ItemRewardPrefab;

		[SerializeField]
		private Transform _ItemRewardHolder;

		private ItemRewardUI _ItemRewardUI;

		[SerializeField]
		private LabelAlias _Name;

		[SerializeField]
		private LabelAlias _Description;

		[SerializeField]
		private ButtonUI _Button;

		private TerminalItemGroupAttribute _CurrentItem;

		public ButtonUI Button
		{
			get
			{
				return _Button;
			}
		}

		public TerminalItemGroupAttribute CurrentItem
		{
			get
			{
				return _CurrentItem;
			}
		}

		public void Init()
		{
			CreateItemReward();
			ClearSelected();
		}

		private void CreateItemReward()
		{
			GameObject gameObject = Object.Instantiate(_ItemRewardPrefab);
			gameObject.transform.SetParent(_ItemRewardHolder, false);
			_ItemRewardUI = gameObject.GetComponent<ItemRewardUI>();
			_ItemRewardUI.Size = _ItemRewardSize;
		}

		public void ClearSelected()
		{
			_CurrentItem = null;
			Refresh();
		}

		public void Select(TerminalItemGroupAttribute p_item)
		{
			_CurrentItem = p_item;
			Refresh();
		}

		public void Refresh()
		{
			UpdateButton();
			if (_CurrentItem == null)
			{
				_ItemRewardUI.gameObject.SetActive(false);
				_Name.gameObject.SetActive(false);
				_Description.gameObject.SetActive(false);
				return;
			}
			_ItemRewardUI.Init(_CurrentItem.ItemImage);
			_Name.SetAlias(_CurrentItem.ItemVisualName);
			_Description.SetAlias(_CurrentItem.ItemDescription);
			_ItemRewardUI.gameObject.SetActive(true);
			_Name.gameObject.SetActive(true);
			_Description.gameObject.SetActive(true);
		}

		public void UpdateButton()
		{
			if (_CurrentItem != null && _CurrentItem.RequirementIsCompleted)
			{
				CurrencyType currencyType = _CurrentItem.CurrencyType;
				int price = _CurrentItem.Price;
				bool buttonActive = _CurrentItem.Price == 0 || DataLocalHelper.CanSpendCurrency(currencyType, price);
				if (_CurrentItem.Price > 0)
				{
					_Button.TurnToPaid();
					_Button.ButtonText.SetAlias("^GUI.Buttons.Buy^");
				}
				else
				{
					_Button.TurnToFree();
					_Button.ButtonText.SetAlias("^GUI.Buttons.GetFree^");
				}
				SetButtonActive(buttonActive);
				_Button.PaidIcon.SpriteName = CurrencyInfo.GetCurrencySprite(currencyType);
				_Button.PaidIcon.color = CurrencyInfo.GetCurrencyColor(currencyType);
				_Button.PaidCount.SetAlias(price.ToString());
				_Button.Button.interactable = true;
				_Button.gameObject.SetActive(true);
				_Button.ResetAlpha();
				if (_CurrentItem.IsExpired)
				{
					_Button.Button.interactable = false;
					_Button.gameObject.SetActive(false);
				}
			}
			else
			{
				_Button.gameObject.SetActive(false);
			}
		}

		private void SetButtonActive(bool p_active)
		{
			_Button.SetType(p_active ? ButtonUI.Type.Green : ButtonUI.Type.Grey);
		}

		public void SetDetailsActive(bool p_active)
		{
			if (p_active)
			{
				_Name.DOColor(ColorUtils.FromHex("bababaff"), 0.6f);
				_Description.DOColor(ColorUtils.FromHex("7b9dadff"), 0.6f);
				_ItemRewardUI.TweenAlpha(false, 0.6f);
			}
			else
			{
				_Name.DOColor(ColorUtils.FromHex("bababa00"), 0.4f);
				_Description.DOColor(ColorUtils.FromHex("7b9dad00"), 0.4f);
				_ItemRewardUI.TweenAlpha(true, 0.4f);
			}
		}

		public void ChangeBackButtonAlpha(float p_alpha, float p_duration)
		{
			_Button.Backdround.DOFade(p_alpha, p_duration);
			_Button.ButtonText.DOFade(p_alpha, p_duration);
		}
	}
}
