using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class TopPanel : UIModule
	{
		private const string PaymentPromoTimerId = "PaymentPromoTimer";

		private const string PromoActiveEffectTimerId = "PromoActiveEffectTimer";

		[SerializeField]
		private Image _Mone1Icon;

		[SerializeField]
		private Text _Money1Text;

		[SerializeField]
		private Image _Mone2Icon;

		[SerializeField]
		private Text _Money2Text;

		[SerializeField]
		private TimerUI _PromoTimer;

		[SerializeField]
		private TimerUI _PromoActiveEffectTimer;

		[SerializeField]
		private EnergyUI _EnergyUI;

		[SerializeField]
		private TextSizeHelper _Money1TextHelper;

		[SerializeField]
		private TextSizeHelper _Money2TextHelper;

		[SerializeField]
		private TextGroupScaler _MoneyTextGroupScaler;

		[SerializeField]
		private Button _PaymentButton;

		[SerializeField]
		private Button _FreeButton;

		[SerializeField]
		private Button _SettingsButton;

		private int _Money1CachedValue;

		private int _Money2CachedValue;

		protected override void Init()
		{
			base.Init();
			EnergyManager.RefreshValuesByBalance();
			_EnergyUI.Init(EnergyManager.MaxLevel);
			if (DataLocal.Current.IsPaidVersion)
			{
				_EnergyUI.gameObject.SetActive(false);
				_PromoActiveEffectTimer.gameObject.SetActive(false);
			}
			_Money1CachedValue = DataLocal.Current.Money2;
			_Money2CachedValue = DataLocal.Current.Money3;
			_Money1Text.text = _Money1CachedValue.ToString();
			_Money2Text.text = _Money2CachedValue.ToString();
			UpdateTimer();
			_Money1TextHelper.enabled = false;
			_Money2TextHelper.enabled = false;
			_MoneyTextGroupScaler.enabled = false;
			UpdateMoneyTextSizes();
			PaymentDialog.OnOpen += OnPaymentOpen;
			PaymentDialog.OnClose += OnPaymentClose;
			OptionsDialog.OnOpen += OnOptionsOpen;
			OptionsDialog.OnClose += OnOptionsClose;
			OptionsDialog.OnReturnToGameButton += OnOptionsClose;
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnOpen -= OnPaymentOpen;
			PaymentDialog.OnClose -= OnPaymentClose;
			OptionsDialog.OnOpen -= OnOptionsOpen;
			OptionsDialog.OnClose -= OnOptionsClose;
			OptionsDialog.OnReturnToGameButton -= OnOptionsClose;
		}

		private void OnPaymentOpen()
		{
			MoveToDialogsCanvas();
			_SettingsButton.interactable = false;
			_EnergyUI.GetComponent<Button>().interactable = false;
		}

		private void OnPaymentClose()
		{
			MoveToSceneCanvas();
			_SettingsButton.interactable = true;
			_EnergyUI.GetComponent<Button>().interactable = true;
			_EnergyUI.gameObject.SetActive(!DataLocal.Current.IsPaidVersion);
			_PromoActiveEffectTimer.gameObject.SetActive(ShowPromoActiveEffectTimer());
		}

		private void OnOptionsOpen()
		{
			MoveToDialogsCanvas();
			_PaymentButton.interactable = false;
			_FreeButton.interactable = false;
			_EnergyUI.GetComponent<Button>().interactable = false;
		}

		private void OnOptionsClose()
		{
			MoveToSceneCanvas();
			_PaymentButton.interactable = true;
			_FreeButton.interactable = true;
			_EnergyUI.GetComponent<Button>().interactable = true;
			_EnergyUI.gameObject.SetActive(!DataLocal.Current.IsPaidVersion);
			_PromoActiveEffectTimer.gameObject.SetActive(ShowPromoActiveEffectTimer());
		}

		public void Update()
		{
			bool flag = false;
			if (_Money1CachedValue != (int)DataLocal.Current.Money2)
			{
				_Money1CachedValue = DataLocal.Current.Money2;
				_Money1Text.text = _Money1CachedValue.ToString();
				flag = true;
			}
			if (_Money2CachedValue != (int)DataLocal.Current.Money3)
			{
				_Money2CachedValue = DataLocal.Current.Money3;
				_Money2Text.text = _Money2CachedValue.ToString();
				flag = true;
			}
			if (flag)
			{
				UpdateMoneyTextSizes();
			}
		}

		private bool ShowPromoActiveEffectTimer()
		{
			return !DataLocal.Current.IsPaidVersion && (int)CounterController.Current.CounterPromoActiveEffect == 1;
		}

		private void UpdateMoneyTextSizes()
		{
			_Money1TextHelper.Refresh();
			_Money2TextHelper.Refresh();
			_MoneyTextGroupScaler.Refresh();
		}

		public void OnSettingsTap()
		{
			DebugUtils.Log("OnSettingsTap");
			if (OptionsDialog.Current == null)
			{
				DialogNotificationManager.ShowOptionsDialog(0);
			}
			else
			{
				OptionsDialog.Current.Close();
			}
		}

		public void OnPaymentTap()
		{
			DebugUtils.Log("OnPaymentTap");
			if (PaymentDialog.Current == null)
			{
				DialogNotificationManager.ShowPaymentDialog("Promo", 0);
			}
			else
			{
				PaymentDialog.Current.Close();
			}
		}

		public void OnFreeTap()
		{
			DebugUtils.Log("OnFreeTap");
			if (PaymentDialog.Current == null)
			{
				DialogNotificationManager.ShowPaymentDialog("Ads");
			}
			else
			{
				PaymentDialog.Current.Close();
			}
		}

		public void OnTimerExpired()
		{
			UpdateTimer();
			UpdateEnergy();
		}

		private void UpdateEnergy()
		{
			EnergyManager.RefreshValuesByBalance();
			EnergyManager.ForceChargeToFullLevel();
			_EnergyUI.ForcedUpdate(EnergyManager.MaxLevel);
		}

		public void UpdateTimer()
		{
			bool flag = (int)CounterController.Current.CounterPaymentPromo == 1 || (int)CounterController.Current.CounterPaymentPromo2 == 1;
			_PromoTimer.gameObject.SetActive(flag);
			_PromoTimer.enabled = flag;
			_PromoTimer.Format = TimerFormat.Full;
			if (flag)
			{
				if ((int)CounterController.Current.CounterPaymentPromo == 1)
				{
					_PromoTimer.Id = "PaymentPromoTimer";
				}
				else if ((int)CounterController.Current.CounterPaymentPromo2 == 1)
				{
					_PromoTimer.Id = "PaymentPromo2Timer";
				}
			}
			bool active = ShowPromoActiveEffectTimer();
			_PromoActiveEffectTimer.gameObject.SetActive(active);
			_PromoActiveEffectTimer.enabled = active;
			_PromoActiveEffectTimer.Id = "PromoActiveEffectTimer";
			_PromoActiveEffectTimer.Format = TimerFormat.Full;
		}

		public void OnTimerClick()
		{
			if ((int)CounterController.Current.CounterPaymentPromo == 1 || (int)CounterController.Current.CounterPaymentPromo2 == 1)
			{
				if (PaymentDialog.Current == null)
				{
					DialogNotificationManager.ShowPaymentDialog("Promo", 0);
				}
				else
				{
					PaymentDialog.Current.Close();
				}
			}
		}

		public void OnTimerPromoEffectClick()
		{
			if ((int)CounterController.Current.CounterPromoActiveEffect == 1)
			{
				DialogNotificationManager.ShowPromoEffectDialog(0);
			}
		}
	}
}
