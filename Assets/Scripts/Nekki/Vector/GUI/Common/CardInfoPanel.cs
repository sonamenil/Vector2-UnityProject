using System;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Utilites;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nekki.Vector.GUI.Common
{
	public class CardInfoPanel : MonoBehaviour
	{
		[SerializeField]
		private BaseCardUISettings _CardUISettings = new BaseCardUISettings();

		[SerializeField]
		private BaseCardUI _CardPrefab;

		[SerializeField]
		private RectTransform _CardHolder;

		[SerializeField]
		private LabelAlias _CardName;

		[SerializeField]
		private LabelAlias _Description;

		[SerializeField]
		private Transform _CardContentTransform;

		[SerializeField]
		private ButtonUI _Button;

		private BaseCardUI _CardUI;

		private Vector2 _CardHolderDefaultPosition;

		private CardsGroupAttribute _SelectedCard;

		private TerminalItemGroupAttribute _SelectedItem;

		public ButtonUI Button
		{
			get
			{
				return _Button;
			}
		}

		public CardsGroupAttribute SelectedCard
		{
			get
			{
				return _SelectedCard;
			}
		}

		public TerminalItemGroupAttribute SelectedItem
		{
			get
			{
				return _SelectedItem;
			}
		}

		private void Awake()
		{
			_CardHolderDefaultPosition = _CardHolder.anchoredPosition;
		}

		public void Init()
		{
			CreateCardUI();
		}

		private void CreateCardUI()
		{
			BaseCardUI baseCardUI = UnityEngine.Object.Instantiate(_CardPrefab);
			baseCardUI.transform.SetParent(_CardHolder, false);
			_CardUI = baseCardUI.GetComponent<BaseCardUI>();
			_CardUI.UISettings = _CardUISettings;
		}

		public void PlayLevelUpAnimation(System.Action p_onAnimationEnd = null)
		{
			if (_SelectedCard == null)
			{
				return;
			}
			EventSystem eventSystem = EventSystem.current;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				if (eventSystem != null)
				{
					eventSystem.enabled = false;
				}
			});
			sequence.AppendInterval(0.4f);
			sequence.AppendCallback(delegate
			{
				UpdateCard(_SelectedCard, false, true);
				if (_Button != null)
				{
					UpdateButton(_SelectedCard);
				}
			});
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				if (p_onAnimationEnd != null)
				{
					p_onAnimationEnd();
				}
			});
			sequence.OnKill(delegate
			{
				if (eventSystem != null)
				{
					eventSystem.enabled = true;
				}
			});
			sequence.Play();
		}

		public void ClearSelected()
		{
			_SelectedCard = null;
			_SelectedItem = null;
			_CardUI.gameObject.SetActive(false);
		}

		public void SelectCard(CardsGroupAttribute card)
		{
			ClearSelected();
			_SelectedCard = card;
			bool flag = card == null || (card.CardType != CardType.Notes && card.CardType != CardType.StoryItems);
			_CardUI.NeedShowProgressAnimation = false;
			_CardUI.NeedShowProgressBar = flag;
			_CardUI.NeedShowProgressNumbers = flag;
			_CardUI.NeedShowCurrentLevelProgress = flag;
			UpdateCard(card);
			if (_Button != null)
			{
				UpdateButton(card);
			}
		}

		public void SelectCard(TerminalItemGroupAttribute terminalItem)
		{
			ClearSelected();
			_SelectedItem = terminalItem;
			_CardUI.AnimationCardCount = _SelectedItem.Count;
			_CardUI.NeedShowProgressAnimation = true;
			_CardUI.NeedShowProgressBar = true;
			_CardUI.NeedShowProgressNumbers = true;
			_CardUI.NeedShowCurrentLevelProgress = true;
			UpdateCard(terminalItem.Card, terminalItem.IsExpired);
			if (_Button != null)
			{
				UpdateButton(terminalItem, terminalItem.IsExpired);
			}
		}

		public void UpdateBoost()
		{
			if (_SelectedCard != null)
			{
				_CardUI.RefreshBoost();
				_Description.SetAlias(_SelectedCard.CardText);
			}
		}

		private void UpdateCard(CardsGroupAttribute p_card, bool p_expired = false, bool p_animDescription = false)
		{
			_CardUI.gameObject.SetActive(p_card != null);
			_CardUI.Card = p_card;
			_CardName.SetAlias((p_card == null) ? string.Empty : p_card.CardVisualName);
			_CardHolder.anchoredPosition = ((!_CardUI.NeedShowProgressBar) ? new Vector2(0f, _CardHolderDefaultPosition.y) : _CardHolderDefaultPosition);
			if (p_animDescription)
			{
				_Description.GetComponent<TextAnimator>().Arm();
			}
			_Description.SetAlias((p_card == null) ? string.Empty : p_card.CardText);
			_Description.resizeTextForBestFit = p_card != null && p_card.Slot == SlotItem.Slot.Notes;
			if (p_expired)
			{
				TwinkleOff();
			}
		}

		public void UpdateButton()
		{
			if (_Button != null)
			{
				if (_SelectedCard != null)
				{
					UpdateButton(_SelectedCard);
				}
				else if (_SelectedItem != null)
				{
					UpdateButton(_SelectedItem);
				}
			}
		}

		private void UpdateButton(CardsGroupAttribute p_card)
		{
			if (p_card.UserCardTotalLevel < p_card.CardMaxLevel && p_card.CardType != CardType.Notes && p_card.CardType != CardType.StoryItems)
			{
				int userBoostPrice = p_card.UserBoostPrice;
				bool flag = CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.CardsBoost);
				if (flag)
				{
					_Button.PaidIcon.SpriteName = CouponsManager.GetCouponButtonIcon(CouponsManager.CouponType.CardsBoost);
					_Button.PaidIcon.color = CouponsManager.GetCouponButtonIconColor(CouponsManager.CouponType.CardsBoost);
					_Button.PaidCount.SetAlias(string.Empty);
					_Button.ButtonText.SetAlias("^GUI.Buttons.Archive.Boost^");
				}
				else
				{
					_Button.PaidIcon.SpriteName = CurrencyInfo.GetCurrencySprite(CurrencyType.Money3);
					_Button.PaidIcon.color = CurrencyInfo.GetCurrencyColor(CurrencyType.Money3);
					_Button.PaidCount.SetAlias(userBoostPrice.ToString());
					_Button.ButtonText.SetAlias("^GUI.Buttons.Archive.Boost^");
				}
				SetButtonActive(flag || (int)DataLocal.Current.Money3 >= userBoostPrice);
				_Button.gameObject.SetActive(true);
			}
			else
			{
				_Button.gameObject.SetActive(false);
			}
		}

		private void UpdateButton(TerminalItemGroupAttribute p_terminalItem, bool p_expired = false)
		{
			if (p_terminalItem.RequirementIsCompleted)
			{
				CurrencyType currencyType = p_terminalItem.CurrencyType;
				int price = p_terminalItem.Price;
				string empty = string.Empty;
				bool buttonActive = p_terminalItem.Price == 0 || DataLocalHelper.CanSpendCurrency(currencyType, price);
				if (price > 0)
				{
					_Button.TurnToPaid();
					empty = ((p_terminalItem.Card.UserCardProgress <= 0) ? "^GUI.Buttons.Buy^" : "^GUI.Buttons.Improve^");
				}
				else
				{
					_Button.TurnToFree();
					empty = "^GUI.Buttons.GetFree^";
				}
				SetButtonActive(buttonActive);
				_Button.PaidIcon.SpriteName = CurrencyInfo.GetCurrencySprite(currencyType);
				_Button.PaidIcon.color = CurrencyInfo.GetCurrencyColor(currencyType);
				_Button.PaidCount.SetAlias(price.ToString());
				_Button.ButtonText.SetAlias(empty);
				_Button.gameObject.SetActive(true);
				_Button.Button.interactable = true;
				_Button.ResetAlpha();
				if (p_expired)
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

		public void SetButtonActive(bool active)
		{
			if (active)
			{
				_Button.SetType(ButtonUI.Type.Green);
			}
			else
			{
				_Button.SetType(ButtonUI.Type.Grey);
			}
		}

		public void SetDetailsPanelActive(bool active)
		{
			if (active)
			{
				_Description.DOColor(ColorUtils.FromHex("7b9dadff"), 0.6f);
				_CardName.DOColor(ColorUtils.FromHex("bababaff"), 0.6f);
				_CardUI.CardAlphaChange(false, 0.6f);
			}
			else
			{
				_Description.DOColor(ColorUtils.FromHex("7b9dad00"), 0.4f);
				_CardName.DOColor(ColorUtils.FromHex("bababa00"), 0.4f);
				_CardUI.CardAlphaChange(true, 0.4f);
			}
		}

		public void TwinkleOff()
		{
			_CardUI.NeedShowProgressAnimation = false;
		}

		public Tweener MoveContentY(float p_y, float p_duration, Ease p_ease)
		{
			return _CardContentTransform.DOLocalMoveY(p_y, p_duration).SetEase(p_ease);
		}

		public void ChangeBackButtonAlpha(float p_alpha, float p_duration)
		{
			_Button.Backdround.DOFade(p_alpha, p_duration);
			_Button.ButtonText.DOFade(p_alpha, p_duration);
		}

		public void HideInstantly(float p_y = 0f)
		{
			Vector3 localPosition = _CardContentTransform.localPosition;
			_CardContentTransform.localPosition = new Vector3(localPosition.x, p_y, localPosition.z);
			base.gameObject.SetActive(false);
		}
	}
}
