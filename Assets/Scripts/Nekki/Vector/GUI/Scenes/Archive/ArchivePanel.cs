using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Archive
{
	public class ArchivePanel : UIModule
	{
		[SerializeField]
		private CardsGrid _CardsGrid;

		[SerializeField]
		private SlotButtonsController _SlotButons;

		[SerializeField]
		private CardInfoPanel _CardDescription;

		private SlotItem.Slot _CurrentSlot = SlotItem.Slot.Legs;

		public SlotItem.Slot CurrentSlot
		{
			get
			{
				return _CurrentSlot;
			}
			set
			{
				_CurrentSlot = value;
				if (_IsInited)
				{
					_CardsGrid.ChangeSlot(_CurrentSlot);
					_SlotButons.ChangeSlot(_CurrentSlot);
				}
			}
		}

		protected override void Init()
		{
			base.Init();
			_CardDescription.Init();
			_CardsGrid.Init(_CurrentSlot, SelectCard);
			_SlotButons.Init(this);
			PaymentDialog.OnClose += OnPaymentClosed;
			TimersManager.OnTimerExpired += OnBoostCardTimerOver;
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnClose -= OnPaymentClosed;
			TimersManager.OnTimerExpired -= OnBoostCardTimerOver;
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			_SlotButons.RefreshSlots();
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			_CardsGrid.SetCardsIsNew(false);
		}

		public void Refresh()
		{
			CurrentSlot = _CurrentSlot;
			_SlotButons.RefreshSlots();
		}

		private void SelectCard(BaseCardUI p_cardUI, bool p_manualSelect)
		{
			_CardDescription.SelectCard(p_cardUI.Card);
			CardsGroupAttribute card = p_cardUI.Card;
			if (p_manualSelect && card != null)
			{
				if (card.IsLevelUp)
				{
					AudioManager.PlaySound("levelup_button");
					card.UserMakeCardLevelUp();
					p_cardUI.Refresh();
					p_cardUI.PlayLevelUpFinishAnimation();
					_CardDescription.PlayLevelUpAnimation(OnLevelUpAnimationEnd);
				}
				else
				{
					AudioManager.PlaySound("select_button");
				}
			}
			_SlotButons.RefreshCurrentSlot();
		}

		public void TryRunPromo2()
		{
			int num = int.Parse(BalanceManager.Current.GetBalance("Cards", "BoostPrice", "1"));
			if (CouponsManager.GetCouponQuantity(CouponsManager.CouponType.CardsBoost) == 0 && (int)DataLocal.Current.Money3 < num && GameManager.TryRunPromo2())
			{
				DialogNotificationManager.ShowSaleDialog(1, true, 7);
			}
		}

		private void OnLevelUpAnimationEnd()
		{
			_CardsGrid.ScrollToNextLevelUp();
		}

		private void OnPaymentClosed()
		{
			_CardDescription.UpdateButton();
		}

		public void OnBoostButtonClick()
		{
			if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.CardsBoost) || (int)DataLocal.Current.Money3 >= _CardDescription.SelectedCard.UserBoostPrice)
			{
				DialogNotificationManager.ShowBoostConfirmationDialog(OnBoostCofirmed);
			}
			else if ((int)CounterController.Current.CounterPaymentPromo2 == 1 || GameManager.TryRunPromo2())
			{
				DialogNotificationManager.ShowSaleDialog(1, true, 7);
			}
			else
			{
				DialogNotificationManager.ShowNotEnoughtCurrencyDialog(CurrencyType.Money3);
			}
		}

		private void OnBoostCofirmed(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			int userBoostPrice = _CardDescription.SelectedCard.UserBoostPrice;
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
				CardsManager.Current.BoostCard(_CardDescription.SelectedCard);
				DataLocal.Current.Save(true);
				ArgsDict argsDict = new ArgsDict();
				argsDict.Add("coupon", flag);
				argsDict.Add("card", _CardDescription.SelectedCard);
				ArgsDict args = argsDict;
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Card_boost, args);
				RefreshBoost();
			}
		}

		public void SetCardsIsNew(bool p_value)
		{
			_CardsGrid.SetCardsIsNew(p_value);
		}

		public void OnBoostCardTimerOver(string p_timerName)
		{
			if (p_timerName.IndexOf(CardsManager.BoostCardTimerPrefix) != -1)
			{
				RefreshBoost();
			}
		}

		public void RefreshBoost()
		{
			_CardDescription.UpdateBoost();
			_CardDescription.UpdateButton();
			_CardsGrid.RefreshBoost();
		}
	}
}
