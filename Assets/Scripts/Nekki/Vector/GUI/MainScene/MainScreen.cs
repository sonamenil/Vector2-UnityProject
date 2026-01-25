using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.MainScene
{
	public class MainScreen : UIModule
	{
		[SerializeField]
		private LabelAlias _ZoneName;

		[SerializeField]
		private SelectFloor _SelectFloor;

		[SerializeField]
		private SelectZone _SelectZone;

		[SerializeField]
		private Button _PremiumButton;

		private bool _ReselectStarterPackToBest = true;

		public bool ReselectStarterPackToBest
		{
			get
			{
				return _ReselectStarterPackToBest;
			}
			set
			{
				_ReselectStarterPackToBest = value;
			}
		}

		private bool IsNeedShowPremiuimUI
		{
			get
			{
				return !DataLocal.Current.IsPaidVersion && (int)CounterController.Current.СounterTutorialInProgress == 0;
			}
		}

		protected override void Init()
		{
			base.Init();
			RefreshPremiumUI();
			PaymentDialog.OnClose += RefreshPremiumUI;
			OptionsDialog.OnClose += RefreshPremiumUI;
			OptionsDialog.OnReturnToGameButton += RefreshPremiumUI;
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnClose -= RefreshPremiumUI;
			OptionsDialog.OnClose -= RefreshPremiumUI;
			OptionsDialog.OnReturnToGameButton -= RefreshPremiumUI;
		}

		protected override void OnActivated()
		{
			Refresh();
		}

		public void Refresh()
		{
			_SelectFloor.Refresh(_ReselectStarterPackToBest);
			_SelectZone.Refresh();
			_ZoneName.SetAlias(Manager.ZoneVisualName);
			_ReselectStarterPackToBest = true;
		}

		public void RefreshPremiumUI()
		{
			_PremiumButton.gameObject.SetActive(IsNeedShowPremiuimUI);
		}

		public void RefreshZoneUI()
		{
			_ZoneName.SetAlias(Manager.ZoneVisualName);
		}

		public void RefreshStarterPacksUI()
		{
			_SelectFloor.Refresh(true);
		}

		public void OnPremiumTap()
		{
			DebugUtils.Log("OnPremiumTap");
			if (PaymentDialog.Current == null)
			{
				DialogNotificationManager.ShowPaymentDialog("Premium");
			}
			else
			{
				PaymentDialog.Current.Close();
			}
		}
	}
}
