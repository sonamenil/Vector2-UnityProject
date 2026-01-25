using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Archive
{
	public class CardsCategory : MonoBehaviour
	{
		private const int _CardsPerRow = 4;

		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _MoreToUnlock;

		[SerializeField]
		private Image _LeftImage;

		[SerializeField]
		private Image _RightImage;

		[SerializeField]
		private GridLayoutGroup _CardGrid;

		private ComponentsPool<BaseCardUI> _CardsPool;

		private Action<BaseCardUI> _OnCardTap;

		private Action<BaseCardUI> _OnPlusCardTap;

		private List<BaseCardUI> _Cards = new List<BaseCardUI>();

		private List<BaseCardUI> _EmptyCards = new List<BaseCardUI>();

		public bool HasLockedCards
		{
			get
			{
				return _MoreToUnlock.enabled;
			}
		}

		public BaseCardUI GetCard(int index)
		{
			if (_Cards.Count > index)
			{
				return _Cards[index];
			}
			return null;
		}

		public BaseCardUI GetCardByCondition(Predicate<BaseCardUI> p_condition)
		{
			return _Cards.Find(p_condition);
		}

		public void Init(ComponentsPool<BaseCardUI> p_cardsPool, Action<BaseCardUI> p_onCardTap, Action<BaseCardUI> p_onPlusTap)
		{
			_CardsPool = p_cardsPool;
			_OnCardTap = p_onCardTap;
			_OnPlusCardTap = p_onPlusTap;
		}

		public void SetCards(List<CardsGroupAttribute> p_cards, string p_title, string p_moreToUnlock, Color p_color, Color p_moreToUnlockColor)
		{
			_Title.gameObject.SetActive(true);
			_Title.SetAlias(p_title);
			_MoreToUnlock.enabled = !string.IsNullOrEmpty(p_moreToUnlock);
			if (HasLockedCards)
			{
				_MoreToUnlock.SetAlias(p_moreToUnlock);
				_MoreToUnlock.color = p_moreToUnlockColor;
			}
			_LeftImage.gameObject.SetActive(true);
			_RightImage.gameObject.SetActive(true);
			_LeftImage.color = p_color;
			_RightImage.color = p_color;
			_CardGrid.cellSize = new Vector2(165f, 165f);
			CreateCards(p_cards);
		}

		public void SetNotes(string p_categoryName, string p_title, Color p_color)
		{
			_Title.gameObject.SetActive(true);
			_Title.SetAlias(p_title);
			_MoreToUnlock.enabled = false;
			_LeftImage.gameObject.SetActive(true);
			_RightImage.gameObject.SetActive(true);
			_LeftImage.color = p_color;
			_RightImage.color = p_color;
			_CardGrid.cellSize = new Vector2(165f, 165f);
			int result = 0;
			int.TryParse(BalanceManager.Current.GetBalance("Notes", p_categoryName, "Count"), out result);
			CreateNotesCards(p_categoryName, result);
		}

		public void SetStoryItems(List<CardsGroupAttribute> p_cards)
		{
			_Title.gameObject.SetActive(false);
			_LeftImage.gameObject.SetActive(false);
			_RightImage.gameObject.SetActive(false);
			_CardGrid.cellSize = new Vector2(165f, 205f);
			_MoreToUnlock.enabled = false;
			CreateStoryItemsCards(p_cards);
		}

		private void CreateCards(List<CardsGroupAttribute> p_cards)
		{
			for (int i = 0; i < p_cards.Count; i++)
			{
				BaseCardUISettings baseCardUISettings = new BaseCardUISettings();
				baseCardUISettings.NeedShowProgressBar = true;
				baseCardUISettings.NeedAnnounce = true;
				baseCardUISettings.NeedShowLevelUpAnimation = true;
				baseCardUISettings.NeedShowStoryCount = false;
				BaseCardUI baseCardUI = _CardsPool.Get();
				baseCardUI.Init(baseCardUISettings, OnCardTap, p_cards[i]);
				baseCardUI.transform.SetParent(_CardGrid.transform, false);
				_Cards.Add(baseCardUI);
			}
			if (HasLockedCards)
			{
				BaseCardUISettings baseCardUISettings = new BaseCardUISettings();
				baseCardUISettings.NeedShowPlusIcon = true;
				baseCardUISettings.NeedShowNoCardIcon = false;
				BaseCardUI baseCardUI = _CardsPool.Get();
				baseCardUI.Init(baseCardUISettings, OnPlusCardTap);
				baseCardUI.transform.SetParent(_CardGrid.transform, false);
				_EmptyCards.Add(baseCardUI);
			}
		}

		private void CreateNotesCards(string p_categoryName, int p_notesCount)
		{
			for (int i = 1; i <= p_notesCount; i++)
			{
				BaseCardUISettings baseCardUISettings = new BaseCardUISettings();
				baseCardUISettings.NeedShowProgressBar = false;
				baseCardUISettings.NeedAnnounce = true;
				baseCardUISettings.NeedShowLevelUpAnimation = false;
				baseCardUISettings.NeedShowStoryCount = false;
				CardsGroupAttribute cardsGroupAttribute = CardsGroupAttribute.Create(string.Format("{0}_{1}", p_categoryName, i));
				BaseCardUI baseCardUI = _CardsPool.Get();
				baseCardUI.Init(baseCardUISettings, OnCardTap, cardsGroupAttribute, cardsGroupAttribute != null && cardsGroupAttribute.UserCardIsExists);
				baseCardUI.transform.SetParent(_CardGrid.transform, false);
				_Cards.Add(baseCardUI);
			}
		}

		private void CreateStoryItemsCards(List<CardsGroupAttribute> p_cards)
		{
			for (int i = 0; i < p_cards.Count; i++)
			{
				BaseCardUISettings baseCardUISettings = new BaseCardUISettings();
				baseCardUISettings.NeedShowProgressBar = false;
				baseCardUISettings.NeedAnnounce = true;
				baseCardUISettings.NeedShowLevelUpAnimation = false;
				baseCardUISettings.NeedShowStoryCount = true;
				BaseCardUI baseCardUI = _CardsPool.Get();
				baseCardUI.Init(baseCardUISettings, OnCardTap, p_cards[i]);
				baseCardUI.transform.SetParent(_CardGrid.transform, false);
				_Cards.Add(baseCardUI);
			}
		}

		public void Reset()
		{
			if (_Cards.Count > 0)
			{
				foreach (BaseCardUI card in _Cards)
				{
					_CardsPool.Return(card);
				}
				_Cards.Clear();
			}
			if (_EmptyCards.Count <= 0)
			{
				return;
			}
			foreach (BaseCardUI emptyCard in _EmptyCards)
			{
				_CardsPool.Return(emptyCard);
			}
			_EmptyCards.Clear();
		}

		private void OnCardTap(BaseCardUI p_card)
		{
			if (_OnCardTap != null)
			{
				_OnCardTap(p_card);
			}
		}

		private void OnPlusCardTap(BaseCardUI p_card)
		{
			if (_OnPlusCardTap != null)
			{
				_OnPlusCardTap(p_card);
			}
		}

		public void SetCardsIsNew(bool p_value)
		{
			foreach (BaseCardUI card in _Cards)
			{
				card.IsNew = p_value;
			}
		}

		public void RefreshBoost()
		{
			foreach (BaseCardUI card in _Cards)
			{
				card.RefreshBoost();
			}
		}

		public int GetRowsCount()
		{
			return (_Cards.Count - 1) / 4 + 1;
		}

		public int GetRowWithLevelUp()
		{
			int i = 0;
			for (int count = _Cards.Count; i < count; i++)
			{
				if (_Cards[i].Card.IsLevelUp)
				{
					return i / 4;
				}
			}
			return -1;
		}
	}
}
