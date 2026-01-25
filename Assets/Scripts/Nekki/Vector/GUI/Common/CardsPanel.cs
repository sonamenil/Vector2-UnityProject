using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class CardsPanel : MonoBehaviour
	{
		[SerializeField]
		private BaseCardUISettings _CardUISettings = new BaseCardUISettings();

		[SerializeField]
		private GameObject _BaseCardUIPrefab;

		private List<BaseCardUI> _Cards = new List<BaseCardUI>();

		private Action<BaseCardUI> _OnCardTap;

		private int _LastSelectedCardIndex;

		public void Init(List<CardsGroupAttribute> p_cards, Action<BaseCardUI> p_onCardTap = null, bool p_showCardLevel = false)
		{
			_OnCardTap = p_onCardTap;
			DestroyChild();
			for (int i = 0; i < p_cards.Count; i++)
			{
				BaseCardUI component = UnityEngine.Object.Instantiate(_BaseCardUIPrefab).GetComponent<BaseCardUI>();
				component.transform.SetParent(base.transform, false);
				component.Init(_CardUISettings, OnCardTap, p_cards[i]);
				if (p_showCardLevel)
				{
					component.NeedShowProgressBar = true;
					component.NeedShowCurrentLevelProgress = false;
					component.SlotOffset = 15;
				}
				_Cards.Add(component);
			}
		}

		public void SelectFirst()
		{
			if (_Cards.Count != 0)
			{
				OnCardTap(_Cards[0]);
				_LastSelectedCardIndex = 0;
			}
		}

		public void SelectFirstOrFocused()
		{
			if (_Cards.Count == 0)
			{
				return;
			}
			foreach (BaseCardUI card in _Cards)
			{
				if (card.Card.IsFocusedOn)
				{
					OnCardTap(card);
					_LastSelectedCardIndex = 0;
					return;
				}
			}
			OnCardTap(_Cards[0]);
			_LastSelectedCardIndex = 0;
		}

		public void SelectLastSelected()
		{
			if (_Cards.Count != 0)
			{
				if (_LastSelectedCardIndex > 0)
				{
					_LastSelectedCardIndex--;
				}
				if (_Cards.Count > _LastSelectedCardIndex)
				{
					OnCardTap(_Cards[_LastSelectedCardIndex]);
				}
			}
		}

		private void DestroyChild()
		{
			for (int i = 0; i < _Cards.Count; i++)
			{
				UnityEngine.Object.Destroy(_Cards[i].gameObject);
			}
			_Cards.Clear();
		}

		public void AppendCards(List<BaseCardUI> toAppend)
		{
			_Cards.AddRange(toAppend);
		}

		public void AppendCards(List<CardsGroupAttribute> toAppend)
		{
			for (int i = 0; i < toAppend.Count; i++)
			{
				BaseCardUI component = UnityEngine.Object.Instantiate(_BaseCardUIPrefab).GetComponent<BaseCardUI>();
				component.CanvasGroupAlpha = 0f;
				component.transform.SetParent(base.transform, false);
				component.Init(_CardUISettings, OnCardTap, toAppend[i]);
				_Cards.Add(component);
				component.CardAppearEndfloor();
			}
		}

		public void AppendCards(BaseCardUI toAppend)
		{
			_Cards.Add(toAppend);
		}

		public int IndexOfCard(BaseCardUI p_card)
		{
			return _Cards.IndexOf(p_card);
		}

		public BaseCardUI CardByIndex(int p_index)
		{
			if (p_index >= _Cards.Count || p_index < 0)
			{
				return null;
			}
			return _Cards[p_index];
		}

		public void RefreshCards()
		{
			foreach (BaseCardUI card in _Cards)
			{
				card.Refresh();
			}
		}

		public void OnCardTap(BaseCardUI p_cardUI)
		{
			if (_OnCardTap != null)
			{
				_OnCardTap(p_cardUI);
				_LastSelectedCardIndex = IndexOfCard(p_cardUI);
			}
		}
	}
}
