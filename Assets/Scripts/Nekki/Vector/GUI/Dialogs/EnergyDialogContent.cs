using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class EnergyDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _Text;

		[SerializeField]
		private Text _Timer;

		[SerializeField]
		private LabelAlias _EnergyLevelInfo;

		[SerializeField]
		private float _RefreshTimerTime = 1f;

		private System.Action _OnCloseAfterRechargeAction;

		private float _DeltaTime;

		private bool _IsNeedUpdate = true;

		private int _CachedEnergyLevel = -1;

		public void Init(System.Action p_onCloseAfterRechargeAction)
		{
			_OnCloseAfterRechargeAction = p_onCloseAfterRechargeAction;
			List<DialogButtonData> list = new List<DialogButtonData>();
			string p_image = CurrencyInfo.GetCurrencySprite(CurrencyType.Money3);
			Color p_imageColor = CurrencyInfo.GetCurrencyColor(CurrencyType.Money3);
			string p_count = GetRechargePriceFromBalance();
			if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.EnergyRecharge))
			{
				p_image = CouponsManager.GetCouponButtonIcon(CouponsManager.CouponType.EnergyRecharge);
				p_imageColor = CouponsManager.GetCouponButtonIconColor(CouponsManager.CouponType.EnergyRecharge);
				p_count = string.Empty;
			}
			list.Add(new DialogButtonData(OnRechargeTap, "^Labels.Energy.RechargeFull^", p_count, p_image, p_imageColor, ButtonUI.Type.Green));
			list.Add(new DialogButtonData(OnCloseTap, "^GUI.Buttons.Ok^", ButtonUI.Type.Blue));
			Init(list);
			if (EnergyManager.IsEnergyFull)
			{
				SwitchToFullCharge();
				return;
			}
			SwitchToCharge();
			UpdateTimer();
			_CachedEnergyLevel = EnergyManager.CurrentLevel;
		}

		public void OnCloseTap(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
		}

		private string GetRechargePriceFromBalance()
		{
			return BalanceManager.Current.GetBalance("Energy", "FullRechargePrice");
		}

		private void ShowNotEnoughMoneyDialog()
		{
			DialogNotificationManager.ShowNotEnoughtCurrencyDialog(CurrencyType.Money3);
		}

		private bool TryBuy()
		{
			int num = Convert.ToInt32(GetRechargePriceFromBalance());
			if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.EnergyRecharge))
			{
				CouponsManager.SpendCoupon(CouponsManager.CouponType.EnergyRecharge);
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Energy_recharge, new ArgsDict { { "coupon", true } });
				return true;
			}
			if ((int)DataLocal.Current.Money3 >= num)
			{
				DataLocal current = DataLocal.Current;
				current.Money3 = (int)current.Money3 - num;
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Energy_recharge, new ArgsDict { { "coupon", false } });
				return true;
			}
			return false;
		}

		public void OnRechargeTap(BaseDialog p_dialog)
		{
			if (TryBuy())
			{
				EnergyManager.ForceChargeToFullLevel();
				if (_OnCloseAfterRechargeAction != null)
				{
					_OnCloseAfterRechargeAction();
				}
			}
			else
			{
				ShowNotEnoughMoneyDialog();
			}
			p_dialog.Dismiss();
		}

		private void Update()
		{
			if (!_IsNeedUpdate)
			{
				return;
			}
			_DeltaTime += Time.deltaTime;
			if (!(_DeltaTime >= _RefreshTimerTime))
			{
				return;
			}
			_DeltaTime = 0f;
			if (EnergyManager.IsEnergyFull)
			{
				SwitchToFullCharge();
				return;
			}
			UpdateTimer();
			if (_CachedEnergyLevel != EnergyManager.CurrentLevel)
			{
				_CachedEnergyLevel = EnergyManager.CurrentLevel;
				UpdateEnergyLevelInfo();
			}
		}

		private void UpdateTimer()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(EnergyManager.TimeToOneCharge);
			_Timer.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
		}

		private void UpdateEnergyLevelInfo()
		{
			_EnergyLevelInfo.SetAlias(string.Format(LocalizationManager.GetPhrase("Labels.Energy.Available"), EnergyManager.CurrentLevel, EnergyManager.MaxLevel));
		}

		private void SwitchToCharge()
		{
			_IsNeedUpdate = true;
			_Text.SetAlias("^Labels.Energy.NextCellRecharge^");
			_Text.fontSize = 55;
			_Timer.gameObject.SetActive(true);
			_EnergyLevelInfo.gameObject.SetActive(true);
			_Buttons[0].TurnOn();
			_Buttons[1].Target.ButtonText.SetAlias("^Labels.Energy.Wait^");
			base.Parent.RefreshButtonPosition();
			UpdateEnergyLevelInfo();
		}

		private void SwitchToFullCharge()
		{
			_IsNeedUpdate = false;
			_Text.SetAlias("^Labels.Energy.FullyCharged^");
			_Text.fontSize = 65;
			_Timer.gameObject.SetActive(false);
			_EnergyLevelInfo.gameObject.SetActive(false);
			_Buttons[0].TurnOff();
			_Buttons[1].Target.ButtonText.SetAlias("^GUI.Buttons.Ok^");
			base.Parent.RefreshButtonPosition();
			UpdateEnergyLevelInfo();
		}
	}
}
