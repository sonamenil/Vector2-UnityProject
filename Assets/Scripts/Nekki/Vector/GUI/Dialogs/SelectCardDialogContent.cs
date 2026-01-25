using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class SelectCardDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _SelectedCardName;

		[SerializeField]
		private LabelAlias _SelectedCardText;

		[SerializeField]
		private LabelAlias _ChangeCardName;

		[SerializeField]
		private LabelAlias _ChangeCardText;

		[SerializeField]
		private CardsPanel _CardsPanel;

		[SerializeField]
		private Transform _SelectedCardPlaceholder;

		[SerializeField]
		private Transform _ChangeCardPlaceholder;

		[SerializeField]
		private LabelAlias _TapText;

		[SerializeField]
		private GameObject _CardPrefab;

		private Action<bool> _Answer;

		private CardsGroupAttribute _DefaultCard;

		private GadgetItem _GadgetItem;

		private List<CardsGroupAttribute> _ItemCards;

		private BaseCardUI _GridSelectedCard;

		private BaseCardUI _SelectedCard;

		private BaseCardUI _ChangeCard;

		private DialogButtonData _OkReplaceBtn;

		private DialogButtonData _CompareBackBtn;

		private bool _IsCardsView;

		public void Init(GadgetItem p_item, CardsGroupAttribute p_card, Action<bool> p_answer)
		{
			_Answer = p_answer;
			_DefaultCard = p_card;
			_GadgetItem = p_item;
			List<DialogButtonData> list = new List<DialogButtonData>();
			_CompareBackBtn = new DialogButtonData(OnCompareTap, "^GUI.Buttons.Compare^", ButtonUI.Type.Blue);
			_OkReplaceBtn = new DialogButtonData(OnOkReplaceTap, "^GUI.Buttons.Replace^", ButtonUI.Type.Green);
			list.Add(_OkReplaceBtn);
			list[0]._SoundAlias = "equip_button";
			list.Add(new DialogButtonData(OnCancelTap, "^GUI.Buttons.Cancel^", ButtonUI.Type.Red));
			list.Add(_CompareBackBtn);
			list[2]._SoundAlias = "select_button";
			Init(list);
			if (_SelectedCard == null)
			{
				_SelectedCard = UnityEngine.Object.Instantiate(_CardPrefab).GetComponent<BaseCardUI>();
				_SelectedCard.transform.SetParent(_SelectedCardPlaceholder, false);
				_ChangeCard = UnityEngine.Object.Instantiate(_CardPrefab).GetComponent<BaseCardUI>();
				_ChangeCard.transform.SetParent(_ChangeCardPlaceholder, false);
			}
			_ChangeCard.CardSize = 250;
			_ChangeCard.NeedShowSlot = true;
			_ChangeCard.Card = _DefaultCard;
			_ChangeCard.SlotOffset = 20;
			_ChangeCard.NeedShowProgressBar = true;
			_ChangeCard.NeedShowCurrentLevelProgress = false;
			_ChangeCard.NeedShowForMissionIcon = true;
			_SelectedCard.NeedShowProgressBar = true;
			_SelectedCard.NeedShowCurrentLevelProgress = false;
			_SelectedCard.NeedShowForMissionIcon = true;
			_ChangeCard.ChangeColorToActive(0f);
			_ChangeCardName.SetAlias(_DefaultCard.CardVisualName);
			_ChangeCardText.SetAlias(_DefaultCard.CardText);
			_ItemCards = _GadgetItem.CardsWithEmpty;
			_CardsPanel.Init(_ItemCards, OnCardTap);
			_CardsPanel.gameObject.SetActive(_ItemCards.Count != 1);
			ShowOnlyCards(true);
			UpdateVisibleSelectedCard();
			_CardsPanel.SelectFirstOrFocused();
		}

		public void OnCardTap(BaseCardUI p_card)
		{
			if (p_card.Card != null && !(_GridSelectedCard == p_card))
			{
				if (_GridSelectedCard != null)
				{
					_GridSelectedCard.ChangeColorToInactive(0f);
				}
				_GridSelectedCard = p_card;
				_GridSelectedCard.ChangeColorToActive(0f);
				_SelectedCardName.SetAlias(p_card.Card.CardVisualName);
				_SelectedCardText.SetAlias(p_card.Card.CardText);
				_SelectedCard.CardSize = 250;
				_SelectedCard.Card = p_card.Card;
				if (_IsCardsView)
				{
					UpdateVisibleSelectedCard();
				}
			}
		}

		private void UpdateVisibleSelectedCard()
		{
			bool active = _GridSelectedCard != null;
			_SelectedCard.gameObject.SetActive(active);
			_SelectedCardName.gameObject.SetActive(active);
			_CompareBackBtn.Target.gameObject.SetActive(active);
		}

		private void ShowOnlyCards(bool p_state)
		{
			_IsCardsView = p_state;
			_CompareBackBtn.Target.ButtonText.SetAlias((!p_state) ? "^GUI.Buttons.Back^" : "^GUI.Buttons.Compare^");
			_SelectedCard.gameObject.SetActive(p_state);
			_ChangeCard.gameObject.SetActive(p_state);
			_ChangeCardText.gameObject.SetActive(!p_state);
			_SelectedCardText.gameObject.SetActive(!p_state);
		}

		public void OnCompareTap(BaseDialog p_dialog)
		{
			if (_GridSelectedCard != null)
			{
				ShowOnlyCards(!_SelectedCard.gameObject.activeSelf);
			}
		}

		public void OnReplaceTap()
		{
			if (!(_GridSelectedCard == null))
			{
				CardsGroupAttribute card = _GridSelectedCard.Card;
				CardsGroupAttribute card2 = _ChangeCard.Card;
				_GridSelectedCard.Card = card2;
				_ChangeCard.NeedShowSlot = true;
				_ChangeCard.Card = card;
				_ChangeCardName.SetAlias(card.CardVisualName);
				_ChangeCardText.SetAlias(card.CardText);
				if (_ItemCards.Contains(card) && _ItemCards.Contains(card2))
				{
					_GadgetItem.SwitchCard(card, card2);
				}
				BaseCardUI gridSelectedCard = _GridSelectedCard;
				_GridSelectedCard = null;
				OnCardTap(gridSelectedCard);
			}
		}

		private void ButtonsOff()
		{
			foreach (DialogButtonData button in _Buttons)
			{
				button.Target.gameObject.SetActive(false);
			}
			_CompareBackBtn.Target.gameObject.SetActive(false);
		}

		public void OnOkReplaceTap(BaseDialog p_Dialog)
		{
			ShowOnlyCards(true);
			ButtonsOff();
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_ChangeCard.GetFadeTween(true, 0.4f));
			sequence.Join(_SelectedCard.GetFadeTween(true, 0.4f));
			sequence.Join(_ChangeCard.GetShiftTween(-100f, 0.4f));
			sequence.Join(_SelectedCard.GetShiftTween(100f, 0.4f));
			sequence.AppendCallback(delegate
			{
				OnReplaceTap();
				ButtonsOff();
			});
			sequence.Append(_ChangeCard.GetFadeTween(false, 0.4f, true));
			sequence.Join(_SelectedCard.GetFadeTween(false, 0.4f, true));
			sequence.Join(_ChangeCard.GetShiftTween(-100f, 0f));
			sequence.Join(_SelectedCard.GetShiftTween(100f, 0f));
			sequence.Join(_ChangeCard.GetShiftTween(0f, 0.4f));
			sequence.Join(_SelectedCard.GetShiftTween(0f, 0.4f));
			sequence.AppendInterval(0.4f);
			sequence.AppendCallback(delegate
			{
				if (_ChangeCard.Card == _DefaultCard)
				{
					p_Dialog.Dismiss();
					_Answer(false);
				}
				else
				{
					p_Dialog.Dismiss();
					StringBuffer.AddString("CardName", _SelectedCard.Card.CardName);
					StringBuffer.AddString("DialogResult", _ChangeCard.Card.CardName);
					_Answer(true);
				}
			});
			sequence.Play();
		}

		public void OnCancelTap(BaseDialog p_Dialog)
		{
			_GridSelectedCard = null;
			p_Dialog.Dismiss();
			_Answer(false);
		}
	}
}
