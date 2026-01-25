using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Yaml;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Archive
{
	internal class CardsGrid : MonoBehaviour
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private Transform _ScrollTransform;

		[SerializeField]
		private ScrollRect _ScrollRect;

		[SerializeField]
		private Transform _CardsCategoryPoolRoot;

		[SerializeField]
		private GameObject _CardsCategoryPrefab;

		[SerializeField]
		private Transform _CardPoolRoot;

		[SerializeField]
		private GameObject _CardPrefab;

		private Action<BaseCardUI, bool> _OnTap;

		private ComponentsPool<CardsCategory> _CategoriesPool;

		private ComponentsPool<BaseCardUI> _CardsPool;

		private SlotItem.Slot _Slot = SlotItem.Slot.NotSlot;

		private SlotItem _SlotItem;

		private List<CardsCategory> _CardsCategories = new List<CardsCategory>();

		private BaseCardUI _SelectedCard;

		private float _ScrollPositionOnEnable = 1f;

		public void Init(SlotItem.Slot p_slot, Action<BaseCardUI, bool> p_onTap)
		{
			_OnTap = p_onTap;
			_CategoriesPool = new ComponentsPool<CardsCategory>(_CardsCategoryPoolRoot, _CardsCategoryPrefab);
			_CardsPool = new ComponentsPool<BaseCardUI>(_CardPoolRoot, _CardPrefab);
			ChangeSlot(p_slot);
		}

		public void ChangeSlot(SlotItem.Slot slot)
		{
			_Slot = slot;
			_SlotItem = DataLocalHelper.GetSlot(_Slot);
			_Title.SetAlias(_SlotItem.SlotArchiveTitle);
			ClearCategory();
			InitCategories();
			ScrollToNextLevelUp();
		}

		private void InitCategories()
		{
			Dictionary<string, List<CardsGroupAttribute>> dictionary = _SlotItem.CategorizedCards();
			if (_Slot == SlotItem.Slot.StoryItems)
			{
				InitCardsCategory(_Slot.GetName(), dictionary[_Slot.GetName()]);
				return;
			}
			Mapping mapping = ((_Slot != SlotItem.Slot.Notes) ? BalanceManager.Current.GetBalanceMapping("CardsCategories") : BalanceManager.Current.GetBalanceMapping("Notes"));
			foreach (Node item in mapping)
			{
				if (dictionary.ContainsKey(item.key))
				{
					InitCardsCategory(item.key, dictionary[item.key]);
				}
			}
		}

		private void InitCardsCategory(string p_name, List<CardsGroupAttribute> p_cards)
		{
			bool p_isTrick = _Slot == SlotItem.Slot.Stunts;
			bool flag = _Slot == SlotItem.Slot.Notes;
			bool flag2 = _Slot == SlotItem.Slot.StoryItems;
			CardsCategory cardsCategory = _CategoriesPool.Get();
			cardsCategory.Init(_CardsPool, SelectCard, PlusCardTap);
			if (flag)
			{
				cardsCategory.SetNotes(p_name, CategoryTitle(p_name, p_isTrick), CategoryColor(p_name));
			}
			else if (flag2)
			{
				cardsCategory.SetStoryItems(p_cards);
			}
			else
			{
				cardsCategory.SetCards(p_cards, CategoryTitle(p_name, p_isTrick), CategoryMoreToUnlock(p_name), CategoryColor(p_name), CategoryMoreToUnlockColor(p_name));
			}
			cardsCategory.transform.SetParent(_ScrollTransform, false);
			_CardsCategories.Add(cardsCategory);
			if (_SelectedCard == null)
			{
				SelectCard(cardsCategory.GetCardByCondition((BaseCardUI p_card) => p_card.IsEnabled), false);
			}
		}

		private void ClearCategory()
		{
			if (_CardsCategories.Count <= 0)
			{
				return;
			}
			foreach (CardsCategory cardsCategory in _CardsCategories)
			{
				_CategoriesPool.Return(cardsCategory);
				cardsCategory.Reset();
			}
			_CardsCategories.Clear();
			_SelectedCard = null;
		}

		public void SelectCard(BaseCardUI p_card)
		{
			SelectCard(p_card, true);
		}

		public void SelectCard(BaseCardUI p_card, bool p_manualSelect)
		{
			UnselectCurrentCard();
			_SelectedCard = p_card;
			_SelectedCard.IsNew = false;
			DataLocal.Current.Save(false);
			p_card.ChangeColorToActive(0.15f);
			if (_OnTap != null)
			{
				_OnTap(p_card, p_manualSelect);
			}
		}

		public void PlusCardTap(BaseCardUI p_card)
		{
			DialogNotificationManager.ShowPaymentDialog("Boosterpacks");
		}

		private void UnselectCurrentCard()
		{
			if (_SelectedCard != null)
			{
				_SelectedCard.ChangeColorToInactive(0.15f);
				_SelectedCard = null;
			}
		}

		public void ScrollToNextLevelUp()
		{
			bool flag = false;
			float num = 0f;
			float num2 = 0f;
			int i = 0;
			for (int count = _CardsCategories.Count; i < count; i++)
			{
				num += 0.33f;
				if (!flag)
				{
					int rowWithLevelUp = _CardsCategories[i].GetRowWithLevelUp();
					if (rowWithLevelUp != -1)
					{
						flag = true;
						num2 = num + (float)rowWithLevelUp + 1f;
					}
				}
				num += (float)_CardsCategories[i].GetRowsCount();
			}
			if (flag)
			{
				float num3 = 1f;
				if (num2 <= 4.7f)
				{
					num3 = 1f;
				}
				else if (num - num2 <= 4.7f)
				{
					num3 = 0f;
				}
				else
				{
					num3 = 1f - num2 / num;
					num3 += (4.7f * num3 - 2.35f) / num;
				}
				_ScrollPositionOnEnable = num3;
				if (base.isActiveAndEnabled)
				{
					StopAllCoroutines();
					StartCoroutine(SetScrollPosition());
				}
			}
		}

		public void OnEnable()
		{
			StopAllCoroutines();
			StartCoroutine(SetScrollPosition());
		}

		private IEnumerator SetScrollPosition()
		{
			yield return new WaitForEndOfFrame();
			_ScrollRect.verticalNormalizedPosition = _ScrollPositionOnEnable;
			_ScrollPositionOnEnable = 1f;
		}

		private Color CategoryColor(string p_name)
		{
			string balance = BalanceManager.Current.GetBalance("CardsCategories", p_name, "Color");
			return ColorUtils.FromHex(balance);
		}

		private string CategoryTitle(string p_name, bool p_isTrick)
		{
			return BalanceManager.Current.GetBalance("CardsCategories", p_name, (!p_isTrick) ? "Text" : "TextTricks");
		}

		private string CategoryMoreToUnlock(string p_name)
		{
			int lockedCardsCount = DataLocalHelper.GetLockedCardsCount(p_name, _Slot.GetName());
			if (lockedCardsCount == 0)
			{
				return string.Empty;
			}
			return string.Format("{0} ^GUI.Labels.Archive.MoreToUnlock^", lockedCardsCount);
		}

		private Color CategoryMoreToUnlockColor(string p_name)
		{
			string balance = BalanceManager.Current.GetBalance("CardsCategories", p_name, "MoreToUnlockColor");
			return ColorUtils.FromHex(balance);
		}

		public void SetCardsIsNew(bool p_value)
		{
			foreach (CardsCategory cardsCategory in _CardsCategories)
			{
				cardsCategory.SetCardsIsNew(p_value);
			}
			DataLocal.Current.Save(false);
		}

		public void RefreshBoost()
		{
			foreach (CardsCategory cardsCategory in _CardsCategories)
			{
				cardsCategory.RefreshBoost();
			}
		}
	}
}
