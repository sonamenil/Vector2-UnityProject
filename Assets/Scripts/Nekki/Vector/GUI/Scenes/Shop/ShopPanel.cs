using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Shop
{
	public class ShopPanel : UIModule
	{
		private static string _EquippedCardName;

		[SerializeField]
		private GadgetUIPanel _GadgetsPanel;

		[SerializeField]
		private GadgetInfoPanel _GadgetInfoPanel;

		[SerializeField]
		private ShopRightPanel _RightPanel;

		[SerializeField]
		private GadgetCardInfo _GadgetCardInfoPanel;

		[SerializeField]
		private Image _Blokirator_3000;

		private CardsGroupAttribute _SelectedCard;

		private BaseCardUI _SelectedCardUI;

		private bool _IsCardInfoShown;

		private bool _IsDirty;

		public static string EquippedCardName
		{
			get
			{
				return _EquippedCardName;
			}
			set
			{
				_EquippedCardName = value;
			}
		}

		public bool IsDirty
		{
			get
			{
				return _IsDirty;
			}
			set
			{
				_IsDirty = value;
			}
		}

		protected override void Init()
		{
			base.Init();
			_GadgetsPanel.Init(DataLocalHelper.GetUserGadgets(), OnGadgetTap);
			_RightPanel.Init(this);
			PaymentDialog.OnClose += OnPaymentClosed;
			if ((int)CounterController.Current.CounterMissionsBlock == 0)
			{
				MissionsManager.RestoreMissions();
				DialogNotificationManager.ShowMissionsDialog(true, 0);
			}
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnClose -= OnPaymentClosed;
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			if (_IsDirty)
			{
				_RightPanel.Refresh(true);
			}
		}

		public void UserSelectCard(BaseCardUI card, bool p_resetRerollTryCount = true)
		{
			if (_SelectedCardUI != null)
			{
				_SelectedCardUI.ChangeColorToInactive(0f);
			}
			_SelectedCardUI = card;
			_SelectedCardUI.ChangeColorToActive(0f);
			_SelectedCard = card.Card;
			_GadgetsPanel.SelectBySlotName(_SelectedCard.SlotName);
			_RightPanel.RerollButtonUpdate();
			if (p_resetRerollTryCount)
			{
				CounterController.Current.CounterRerollTryCount = 1;
			}
		}

		public void OnGadgetTap(GadgetUI p_gadget, bool p_instant = false)
		{
			if (p_instant || _IsCardInfoShown)
			{
				_GadgetInfoPanel.Init(p_gadget.Gadget, UserTapGadgetCard);
				OnBackTapGadgetCardInfo();
				return;
			}
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_GadgetInfoPanel.ChangeAlpha(0f, 0.3f, Ease.InQuad));
			sequence.AppendCallback(delegate
			{
				_GadgetInfoPanel.Init(p_gadget.Gadget, UserTapGadgetCard);
			});
			sequence.Append(_GadgetInfoPanel.ChangeAlpha(1f, 0.3f, Ease.OutQuad));
			sequence.Play();
		}

		public void UserTapGadgetCard(BaseCardUI p_card)
		{
			if (p_card.Card == null)
			{
				AudioManager.PlaySound("grey_button");
				return;
			}
			AudioManager.PlaySound("select_button");
			p_card.Card.IsFocusedOn = true;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_GadgetInfoPanel.ChangeAlpha(0f, 0.5f, Ease.InQuad));
			sequence.AppendCallback(delegate
			{
				_GadgetCardInfoPanel.Init(p_card.Card);
				_GadgetCardInfoPanel.gameObject.SetActive(true);
				_GadgetInfoPanel.gameObject.SetActive(false);
			});
			sequence.Append(_GadgetCardInfoPanel.MoveContentY(120f, 0.3f, Ease.OutQuad));
			sequence.AppendCallback(delegate
			{
				_GadgetCardInfoPanel.ChangeButtonsAlpha(1f, 0.3f);
				_IsCardInfoShown = true;
			});
			sequence.Play();
		}

		public void OnBackTapGadgetCardInfo()
		{
			if (_GadgetCardInfoPanel.Card != null)
			{
				_GadgetCardInfoPanel.Card.IsFocusedOn = false;
			}
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				_GadgetCardInfoPanel.ChangeButtonsAlpha(0f, 0.3f);
			});
			sequence.Append(_GadgetCardInfoPanel.MoveContentY(1040f, 0.5f, Ease.InQuad));
			sequence.AppendCallback(delegate
			{
				_GadgetCardInfoPanel.gameObject.SetActive(false);
				_GadgetInfoPanel.gameObject.SetActive(true);
			});
			sequence.Append(_GadgetInfoPanel.ChangeAlpha(1f, 0.3f, Ease.OutQuad));
			sequence.AppendCallback(delegate
			{
				_IsCardInfoShown = false;
			});
			sequence.Play();
		}

		public void OnEquipBtnTap()
		{
			if (_SelectedCard != null)
			{
				_EquippedCardName = _SelectedCard.CardName;
				RunPresetData.OnPresetEnd = (Action<List<PresetResult>>)Delegate.Combine(RunPresetData.OnPresetEnd, new Action<List<PresetResult>>(OnPresetEnd));
				PresetResult presetResult = CardPresetsManager.UsePreset(_SelectedCard);
				RunPresetData.OnPresetEnd = (Action<List<PresetResult>>)Delegate.Remove(RunPresetData.OnPresetEnd, new Action<List<PresetResult>>(OnPresetEnd));
				if (presetResult.DialogData != null)
				{
					AudioManager.PlaySound("grey_button");
					presetResult.DialogData.OnPresetEnd = OnPresetEnd;
					return;
				}
				if (presetResult.RunPreset)
				{
					AudioManager.PlaySound("grey_button");
					return;
				}
				AudioManager.PlaySound("equip_button");
				ArgsDict argsDict = new ArgsDict();
				argsDict.Add("new_card", _SelectedCard);
				ArgsDict args = argsDict;
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Equip_card, args);
				StartRemovingSequence();
			}
		}

		public void OnBoostButtonClick()
		{
			if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.CardsBoost) || (int)DataLocal.Current.Money3 >= _GadgetCardInfoPanel.Card.UserBoostPrice)
			{
				AudioManager.PlaySound("blue_button");
				DialogNotificationManager.ShowBoostConfirmationDialog(OnBoostCofirmed);
			}
			else if ((int)CounterController.Current.CounterPaymentPromo2 == 1 || GameManager.TryRunPromo2())
			{
				DialogNotificationManager.ShowSaleDialog(1, true, 7);
			}
			else
			{
				AudioManager.PlaySound("red_button");
				DialogNotificationManager.ShowNotEnoughtCurrencyDialog(CurrencyType.Money3);
			}
		}

		private void OnBoostCofirmed(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			int userBoostPrice = _GadgetCardInfoPanel.Card.UserBoostPrice;
			bool flag = CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.CardsBoost);
			if (flag || (int)DataLocal.Current.Money3 >= userBoostPrice)
			{
				if (flag)
				{
					CouponsManager.SpendCoupon(CouponsManager.CouponType.CardsBoost);
				}
				else
				{
					DataLocal current = DataLocal.Current;
					current.Money3 = (int)current.Money3 - userBoostPrice;
				}
				CardsManager.Current.BoostCard(_GadgetCardInfoPanel.Card);
				DataLocal.Current.Save(true);
				ArgsDict argsDict = new ArgsDict();
				argsDict.Add("coupon", flag);
				argsDict.Add("card", _GadgetCardInfoPanel.Card);
				ArgsDict args = argsDict;
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Card_boost, args);
				RefreshBoost();
			}
		}

		private void RefreshBoost()
		{
			_GadgetCardInfoPanel.RefreshCard();
			_GadgetCardInfoPanel.RefreshBoostButton();
		}

		private void SendStatistics()
		{
			string @string = StringBuffer.GetString("CardName");
			string string2 = StringBuffer.GetString("DialogResult");
			if (!string.IsNullOrEmpty(@string) && !string.IsNullOrEmpty(string2))
			{
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Equip_card, new ArgsDict
				{
					{
						"new_card",
						CardsGroupAttribute.Create(@string)
					},
					{
						"replaced_card",
						CardsGroupAttribute.Create(string2)
					}
				});
			}
			StringBuffer.AddString("CardName", string.Empty);
			StringBuffer.AddString("DialogResult", string.Empty);
		}

		private void StartRemovingSequence()
		{
			_Blokirator_3000.gameObject.SetActive(true);
			_SelectedCard.UnmountFromItem();
			GameRestorer.SaveBackup();
			float duration = 0.2f;
			float interval = 0.1f;
			float interval2 = 0.1f;
			string selectedSlotName = _GadgetsPanel.GetSelectedSlotName();
			_GadgetsPanel.Init(DataLocalHelper.GetUserGadgets(), OnGadgetTap, false);
			_GadgetsPanel.SelectBySlotName(selectedSlotName);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_SelectedCardUI.CanvasGroup.DOFade(0f, duration));
			sequence.AppendInterval(interval);
			sequence.AppendCallback(delegate
			{
				_SelectedCardUI.LayoutElement.ignoreLayout = true;
			});
			sequence.AppendInterval(interval2);
			sequence.OnKill(RemovingSquenceComplete);
			sequence.Play();
		}

		private void RemovingSquenceComplete()
		{
			_Blokirator_3000.gameObject.SetActive(false);
			_SelectedCard = null;
			_RightPanel.Refresh(true);
		}

		private void OnPresetEnd(List<PresetResult> p_result)
		{
			SendStatistics();
			if (p_result != null)
			{
				StartRemovingSequence();
			}
			else
			{
				_EquippedCardName = string.Empty;
			}
		}

		public string GetRerollPriceFromBalance()
		{
			return BalanceManager.Current.GetBalance("Reroll", "Price");
		}

		private void Refresh()
		{
			_GadgetsPanel.Init(DataLocalHelper.GetUserGadgets(), OnGadgetTap);
			_RightPanel.Refresh(true);
		}

		public void OnRerollBtnTap()
		{
			if (_SelectedCard == null)
			{
				return;
			}
			bool flag = _SelectedCard.CardFreeRerolls > 0;
			int num = Convert.ToInt32(GetRerollPriceFromBalance());
			bool haveCoupon = CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.Reroll);
			if (flag || haveCoupon || (int)DataLocal.Current.Money3 >= num)
			{
				AudioManager.PlaySound("green_button");
				if (!flag)
				{
					if (haveCoupon)
					{
						CouponsManager.SpendCoupon(CouponsManager.CouponType.Reroll);
					}
					else
					{
						DataLocal current = DataLocal.Current;
						current.Money3 = (int)current.Money3 - num;
					}
				}
				_RightPanel.RerollButtonUpdate();
				CardsGroupAttribute card = EndFloorManager.RerollItem(_SelectedCard);
				if (card != null)
				{
					Sequence sequence = DOTween.Sequence();
					sequence.AppendCallback(delegate
					{
						_Blokirator_3000.gameObject.SetActive(true);
						_SelectedCardUI.CardAlphaChange(true, 0.5f);
					});
					sequence.AppendInterval(0.5f);
					sequence.AppendCallback(delegate
					{
						ArgsDict args = new ArgsDict
						{
							{ "new_card", card },
							{ "rerolled_card", _SelectedCard },
							{ "coupon", haveCoupon }
						};
						StatisticsCollector.SetEvent(StatisticsEvent.EventType.Reroll_card, args);
						_SelectedCardUI.NeedShowSlot = true;
						_SelectedCardUI.Card = card;
						_RightPanel.SetInformation();
						UserSelectCard(_SelectedCardUI, false);
						DataLocal.Current.Save(true);
						GameRestorer.SaveBackup();
						++CounterController.Current.CounterRerollTryCount;
					});
					sequence.AppendCallback(delegate
					{
						_SelectedCardUI.CardAlphaChange(false, 0.5f);
					});
					sequence.OnComplete(delegate
					{
						_Blokirator_3000.gameObject.SetActive(false);
					});
					sequence.Play();
				}
			}
			else
			{
				AudioManager.PlaySound("red_button");
				DialogNotificationManager.ShowNotEnoughtCurrencyDialog(CurrencyType.Money3);
			}
		}

		public void RefreshGadgetInfoAndCards()
		{
			if (_GadgetCardInfoPanel.Card != null)
			{
				_GadgetCardInfoPanel.RefreshCard();
				_GadgetCardInfoPanel.RefreshBoostButton();
			}
			_RightPanel.RefreshCurrentCards();
			_RightPanel.RerollButtonUpdate();
		}

		private void OnPaymentClosed()
		{
			if (_GadgetCardInfoPanel.gameObject.activeSelf && _GadgetCardInfoPanel.Card != null)
			{
				_GadgetCardInfoPanel.RefreshBoostButton();
			}
		}
	}
}
