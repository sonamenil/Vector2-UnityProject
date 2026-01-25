using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Shop
{
	public class ShopRightPanel : MonoBehaviour
	{
		[SerializeField]
		private CardsPanel _CardsPanel;

		[SerializeField]
		private PlateScroller _CardScroller;

		[SerializeField]
		private LabelAlias _CardName;

		[SerializeField]
		private LabelAlias _NoCardsLabel;

		[SerializeField]
		private LabelAlias _CardDiscription;

		[SerializeField]
		private ButtonUI _RerollButton;

		[SerializeField]
		private GameObject _EquipGameObject;

		[SerializeField]
		private GameObject _EmptyCardPrefab;

		private ShopPanel _Parent;

		private BaseCardUI _LastSelectedCard;

		private void Awake()
		{
			PlateScroller cardScroller = _CardScroller;
			cardScroller.OnMove = (Action<int, float>)Delegate.Combine(cardScroller.OnMove, new Action<int, float>(OnScrollerMove));
			PlateScroller cardScroller2 = _CardScroller;
			cardScroller2.OnStop = (Action<int>)Delegate.Combine(cardScroller2.OnStop, new Action<int>(OnStop));
		}

		public void Init(ShopPanel p_parent)
		{
			_Parent = p_parent;
			Refresh(false);
		}

		public void RerollButtonUpdate()
		{
			if (_LastSelectedCard != null && _LastSelectedCard.Card.CardFreeRerolls > 0)
			{
				_RerollButton.TurnToFree();
			}
			else if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.Reroll))
			{
				_RerollButton.TurnToPaid();
				_RerollButton.PaidIcon.SpriteName = CouponsManager.GetCouponButtonIcon(CouponsManager.CouponType.Reroll);
				_RerollButton.PaidIcon.color = CouponsManager.GetCouponButtonIconColor(CouponsManager.CouponType.Reroll);
				_RerollButton.PaidCount.Text = string.Empty;
			}
			else
			{
				_RerollButton.TurnToPaid();
				_RerollButton.PaidIcon.SpriteName = CurrencyInfo.GetCurrencySprite(CurrencyType.Money3);
				_RerollButton.PaidIcon.color = CurrencyInfo.GetCurrencyColor(CurrencyType.Money3);
				_RerollButton.PaidCount.Text = BalanceManager.Current.GetBalance("Reroll", "Price");
			}
		}

		private void SetCardsInfoEnabled(bool value)
		{
			_CardDiscription.gameObject.SetActive(value);
			_EquipGameObject.gameObject.SetActive(value);
			RerollButtonUpdate();
			_RerollButton.gameObject.SetActive(value);
			_CardName.gameObject.SetActive(value);
			_NoCardsLabel.gameObject.SetActive(!value);
			_CardScroller.gameObject.SetActive(value);
		}

		public void Refresh(bool p_keepSelection = true)
		{
			List<CardsGroupAttribute> list = EndFloorManager.BasketItemsAllCards();
			_CardScroller.gameObject.SetActive(list.Count > 0);
			if (_CardScroller.gameObject.activeSelf)
			{
				SetCardsInfoEnabled(true);
				_CardsPanel.Init(list, OnCardTap, true);
				_CardScroller.CreateEmpty(3, 3, _EmptyCardPrefab);
				if (p_keepSelection)
				{
					_CardsPanel.SelectLastSelected();
				}
				else
				{
					_CardsPanel.SelectFirst();
				}
				_RerollButton.gameObject.SetActive((int)CounterController.Current.CounterRerollShopBlock != 1 && (int)CounterController.Current.CounterAvailableCardsIsOver != 1);
			}
			else
			{
				SetCardsInfoEnabled(false);
			}
		}

		public void RefreshCurrentCards()
		{
			_CardsPanel.RefreshCards();
			SetInformation();
		}

		public void OnCardTap(BaseCardUI p_Card)
		{
			if (_LastSelectedCard != null)
			{
				_CardScroller.SetPlateToCenter(_CardsPanel.IndexOfCard(p_Card), true, _CardsPanel.IndexOfCard(_LastSelectedCard));
			}
			else
			{
				_CardScroller.SetPlateToCenter(_CardsPanel.IndexOfCard(p_Card), true, _CardsPanel.IndexOfCard(p_Card));
			}
		}

		public void SetInformation()
		{
			if (_LastSelectedCard != null)
			{
				_CardName.SetAlias(_LastSelectedCard.Card.CardVisualName);
				_CardDiscription.SetAlias(_LastSelectedCard.Card.CardText);
			}
		}

		private void OnScrollerMove(int p_pos, float p_delta)
		{
			p_delta = 1f - Mathf.Min(Mathf.Abs(p_delta), 1f);
			if (_LastSelectedCard != null && _CardsPanel.IndexOfCard(_LastSelectedCard) == p_pos)
			{
				_CardsPanel.CardByIndex(p_pos).MoveContentTo(-40f * p_delta, 0f, false);
			}
		}

		private void OnStop(int p_position)
		{
			if (_LastSelectedCard != null && _CardsPanel.IndexOfCard(_LastSelectedCard) != p_position)
			{
				_LastSelectedCard.MoveContentTo(0f);
			}
			_LastSelectedCard = _CardsPanel.CardByIndex(p_position);
			_LastSelectedCard.MoveContentTo(-40f, 0.3f, true, true, 40f);
			SetInformation();
			_Parent.UserSelectCard(_LastSelectedCard);
		}
	}
}
