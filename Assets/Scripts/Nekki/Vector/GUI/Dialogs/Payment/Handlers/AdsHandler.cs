using Nekki.Vector.Core;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.GUI.Dialogs.Payment.Handlers
{
	public class AdsHandler : BaseHandler
	{
		private const string _OfferwallId = "offerwall";

		private const string _VideoId = "videoads";

		private Product _CurrentProduct;

		public override void Init(PaymentDialog p_parent)
		{
			base.Init(p_parent);
			ApplicationController.OnAppPauseCallBack += OnPause;
			VectorADSystem.Reinit();
		}

		public override void Free()
		{
			base.Free();
			ApplicationController.OnAppPauseCallBack -= OnPause;
		}

		public override void UseProduct(Product p_product)
		{
			if (DeviceInformation.IsEmulator)
			{
				DataLocal current = DataLocal.Current;
				current.Money3 = (int)current.Money3 + 1;
				_Parent.Unblock();
				return;
			}
			_CurrentProduct = p_product;
			if (PermissionsChecker.ShowExplanationWithOpenSettings("android.permission.READ_PHONE_STATE", OnGranded, OnDenied, OnUserSkip) && PermissionsChecker.CheckPermission("android.permission.READ_PHONE_STATE", OnGranded, OnDenied, OnUserSkip))
			{
				GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Free", p_product.Id, 1L);
				if (p_product.Id == "offerwall")
				{
					ShowOfferwal();
				}
				else if (p_product.Id == "videoads")
				{
					ShowVideoAdvertisement();
				}
				else
				{
					DebugUtils.LogError("[AdsHandler]: try to use unknown ads product - " + p_product.Id);
				}
			}
		}

		private void ShowOfferwal()
		{
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "tjow_start" } });
			ADSystem.Current.RequestTapjoyOfferWall();
		}

		private void ShowVideoAdvertisement()
		{
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "tjow_start" } });
			ADSystem.Current.RequestRewardedVideoWithVirtualCurrency(false, "currency_video");
		}

		private void OnAdReady(ADSystem.ADType p_adType)
		{
			DebugUtils.Log("[AdsHandler]: OnAdReady - " + p_adType);
			switch (p_adType)
			{
			case ADSystem.ADType.OfferWall:
				ADSystem.Current.ShowOfferWall();
				break;
			case ADSystem.ADType.RewardedVideo:
				ADSystem.Current.ShowRewardedVideo();
				break;
			case ADSystem.ADType.TapjoyOfferWall:
				ADSystem.Current.ShowTapjoyOfferwall();
				break;
			}
		}

		private void OnAdStarted(ADSystem.ADType p_adType)
		{
			DebugUtils.Log("[AdsHandler]: OnAdStarted - " + p_adType);
			AudioManager.PauseMusic(true);
		}

		private void OnAdFinished(ADSystem.ADType p_adType, ADSystem.ADFinishedStatus p_status)
		{
			DebugUtils.Log("[AdsHandler]: OnAdFinished - " + p_adType.ToString() + "|" + p_status);
			switch (p_adType)
			{
			case ADSystem.ADType.TapjoyOfferWall:
				ADSystem.Current.RequestTapjoyCurrency();
				break;
			}
			AudioManager.PauseMusic(false);
			_Parent.Unblock();
		}

		private void OnAdLoadFail(ADSystem.ADType p_adType)
		{
			DebugUtils.Log("[AdsHandler]: OnAdLoadFail - " + p_adType);
			switch (p_adType)
			{
			case ADSystem.ADType.TapjoyOfferWall:
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "tjow_fail" } });
				break;
			case ADSystem.ADType.RewardedVideo:
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "rv_fail" } });
				break;
			}
			OnRequestError();
		}

		private void OnReciveMoney()
		{
			DebugUtils.Log("[AdsHandler]: OnRecieveMoney");
			_Parent.ShowNotification("^Payment.Events.ExternalSuccess^");
			_Parent.Unblock();
		}

		private void OnRequestError()
		{
			DebugUtils.Log("[AdsHandler]: OnRequestError");
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "fail" } });
			AudioManager.PauseMusic(false);
			_Parent.ShowNotification("^Payment.Events.Failed^");
			_Parent.Unblock();
		}

		private void OnPause(bool p_pause)
		{
			DebugUtils.Log("[AdsHandler]: OnPause - " + p_pause);
			_Parent.Unblock();
		}

		private void OnGranded(string p_permishen)
		{
			UseProduct(_CurrentProduct);
		}

		private void OnDenied(string p_permishen)
		{
			_Parent.Unblock();
		}

		private void OnUserSkip(string p_permishen)
		{
			_Parent.Unblock();
		}
	}
}
