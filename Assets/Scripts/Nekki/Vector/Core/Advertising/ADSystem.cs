using System;
using Nekki.Vector.Core.Game;
using UnityEngine;

namespace Nekki.Vector.Core.Advertising
{
	public class ADSystem
	{
		public enum ADType
		{
			InterstitialAd = 0,
			RewardedVideo = 1,
			OfferWall = 2,
			TapjoyOfferWall = 3,
			Unknown = 4
		}

		public enum ADFinishedStatus
		{
			CloseFinished = 0,
			CloseAborted = 1,
			Error = 2
		}

		public static ADSystem Current;

		// private Ad _CachedInterstitialAd;

		// private Ad _CachedRewardedVideo;

		// private Ad _CachedOfferWall;

		public static string _TapjoyOfferwallName;

		// private TJPlacement _TapjoyOfferwall;

		private bool _isTJOfferWallReady;

		// private static Fyber _FyberSession;

		// public bool IsInterstitialADAvailable
		// {
		// 	get
		// 	{
		// 		return _CachedInterstitialAd != null;
		// 	}
		// }

		// public bool IsRewardedVideoAvailable
		// {
		// 	get
		// 	{
		// 		return _CachedRewardedVideo != null;
		// 	}
		// }

		// public bool IsOfferWallAvailable
		// {
		// 	get
		// 	{
		// 		return _CachedOfferWall != null;
		// 	}
		// }

		public bool IsOverrideMetodOnRevardVideoExist
		{
			get
			{
				return this.OverrideMetodOnRevardVideo != null;
			}
		}

		public event Action<ADType> OnAdReady;

		public event Action<ADType> OnAdLoadFail;

		public event Action<ADType> OnAdStarted;

		public event Action<ADType, ADFinishedStatus> OnAdFinished;

		public event Action OnReciveMoney;

		public event Action OnRequestError;

		public event Action OverrideMetodOnRevardVideo;

		public event Action<string, float> OnVirtualCurrencyRecived;

		protected ADSystem()
		{
			Current = this;
		}

		protected static void Init(string p_appID, string p_securityToken = null)
		{
			Current = new ADSystem();
			Current.Init();
			InitFyber(p_appID, p_securityToken);
		}

		protected static void InitFyber(string p_appID, string p_securityToken = null)
		{
			// if (_FyberSession != null)
			// {
			// 	return;
			// }
			if (InternetUtils.CheckInternetConnection())
			{
				// _FyberSession = Fyber.With(p_appID);
				// if (p_securityToken != null)
				// {
				// 	_FyberSession.WithSecurityToken(p_securityToken);
				// }
				// FyberPlugin.Settings settings = _FyberSession.Start();
				// settings.NotifyUserOnCompletion(false);
				// settings.NotifyUserOnReward(false);
			}
			else
			{
				Debug.Log("[ADSystem]: Fyber not connected! Internet is not available!");
			}
		}

		private void Init()
		{
			// FyberLogger.EnableLogging(!Nekki.Vector.Core.Game.Settings.IsReleaseBuild);
			// FyberCallback.AdAvailable += OnAdAvailableCB;
			// FyberCallback.AdStarted += OnAdStartedCB;
			// FyberCallback.AdFinished += OnAdFinishedCB;
			// FyberCallback.AdNotAvailable += OnAdNotAvailableCB;
			// FyberCallback.RequestFail += OnRequestFailCB;
			// FyberCallback.VirtualCurrencySuccess += OnCurrencyResponse;
			// FyberCallback.VirtualCurrencyError += OnCurrencyErrorResponse;
			// TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
			// TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
			// TJPlacement.OnContentShow += HandlePlacementContentShow;
			// TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
			// Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
		}

		~ADSystem()
		{
			// FyberCallback.AdAvailable -= OnAdAvailableCB;
			// FyberCallback.AdStarted -= OnAdStartedCB;
			// FyberCallback.AdFinished -= OnAdFinishedCB;
			// FyberCallback.AdNotAvailable -= OnAdNotAvailableCB;
			// FyberCallback.RequestFail -= OnRequestFailCB;
			// FyberCallback.VirtualCurrencySuccess -= OnCurrencyResponse;
			// FyberCallback.VirtualCurrencyError -= OnCurrencyErrorResponse;
			// TJPlacement.OnRequestSuccess -= HandlePlacementRequestSuccess;
			// TJPlacement.OnRequestFailure -= HandlePlacementRequestFailure;
		}

		public void RequestInterstitialAD()
		{
			// if (_CachedInterstitialAd != null)
			// {
			// 	SendAdReadyEvent(ADType.InterstitialAd);
			// }
			// else
			{
				// InterstitialRequester.Create().Request();
			}
		}

		public void RequestRewardedVideo(bool p_notificationUserOnCompletion)
		{
			// RewardedVideoRequester.Create().NotifyUserOnCompletion(p_notificationUserOnCompletion).Request();
		}

		public void RequestRewardedVideoWithVirtualCurrency(bool p_notificationUserOnCompletion, string p_currencyId = null)
		{
			// RewardedVideoRequester.Create().NotifyUserOnCompletion(p_notificationUserOnCompletion).WithVirtualCurrencyRequester(CreateVirtualCurrencyRequester(p_notificationUserOnCompletion, p_currencyId))
			// 	.Request();
		}

		public void RequestOfferWall()
		{
			// OfferWallRequester.Create().Request();
		}

		public void RequestTapjoyOfferWall()
		{
			// if (!Tapjoy.IsConnected)
			// {
			// 	DebugUtils.LogToConsole("[ADSystem]: Tapjoy not connected!");
			// 	SendAdLoadFailEvent(ADType.TapjoyOfferWall);
			// 	return;
			// }
			// if (_TapjoyOfferwall == null)
			// {
			// 	_TapjoyOfferwall = TJPlacement.CreatePlacement(_TapjoyOfferwallName);
			// }
			// _isTJOfferWallReady = false;
			// DebugUtils.LogToConsole("[ADSystem]: _TapjoyOfferwall.RequestContent");
			// _TapjoyOfferwall.RequestContent();
		}

		public void RequestVirtualCurrency(bool p_notificationUserOnRevard, string p_currencyId = null)
		{
			// CreateVirtualCurrencyRequester(p_notificationUserOnRevard, p_currencyId).Request();
		}

		public void RequestTapjoyCurrency()
		{
			// Tapjoy.GetCurrencyBalance();
		}

		// private static VirtualCurrencyRequester CreateVirtualCurrencyRequester(bool p_notificationUserOnRevard, string p_currencyId = null)
		// {
		// 	// VirtualCurrencyRequester virtualCurrencyRequester = VirtualCurrencyRequester.Create().NotifyUserOnReward(p_notificationUserOnRevard);
		// 	// if (p_currencyId != null)
		// 	// {
		// 	// 	virtualCurrencyRequester.ForCurrencyId(p_currencyId);
		// 	// }
		// 	// return virtualCurrencyRequester;
		// }

		public void ShowInterstitialAD()
		{
			// if (_CachedInterstitialAd != null)
			// {
			// 	_CachedInterstitialAd.Start();
			// 	_CachedInterstitialAd = null;
			// }
		}

		public void ShowRewardedVideo()
		{
			// if (_CachedRewardedVideo != null)
			// {
			// 	_CachedRewardedVideo.Start();
			// 	_CachedRewardedVideo = null;
			// }
		}

		public void ShowOfferWall()
		{
			// if (_CachedOfferWall != null)
			// {
			// 	_CachedOfferWall.Start();
			// 	_CachedOfferWall = null;
			// }
		}

		public void ShowTapjoyOfferwall()
		{
			// if (_isTJOfferWallReady && _TapjoyOfferwall != null)
			// {
			// 	_TapjoyOfferwall.ShowContent();
			// }
		}

		private void SendAdReadyEvent(ADType p_type)
		{
			if (this.OnAdReady != null)
			{
				this.OnAdReady(p_type);
			}
		}

		private void SendAdLoadFailEvent(ADType p_type)
		{
			if (this.OnAdLoadFail != null)
			{
				this.OnAdLoadFail(p_type);
			}
		}

		private void SendAdFinishedEvent(ADType p_type, ADFinishedStatus p_status)
		{
			if (this.OnAdFinished != null)
			{
				this.OnAdFinished(p_type, p_status);
			}
		}

		private void SendAdStartedEvent(ADType p_type)
		{
			if (this.OnAdStarted != null)
			{
				this.OnAdStarted(p_type);
			}
		}

		private void SendVirtualCurrencyRecived(string p_name, float p_count)
		{
			if (this.OnVirtualCurrencyRecived != null)
			{
				this.OnVirtualCurrencyRecived(p_name, p_count);
			}
		}

		// private void OnAdAvailableCB(Ad ad)
		// {
		// 	switch (ad.AdFormat)
		// 	{
		// 	case AdFormat.INTERSTITIAL:
		// 		_CachedInterstitialAd = ad;
		// 		SendAdReadyEvent(ADType.InterstitialAd);
		// 		break;
		// 	case AdFormat.REWARDED_VIDEO:
		// 		_CachedRewardedVideo = ad;
		// 		SendAdReadyEvent(ADType.RewardedVideo);
		// 		break;
		// 	case AdFormat.OFFER_WALL:
		// 		_CachedOfferWall = ad;
		// 		SendAdReadyEvent(ADType.OfferWall);
		// 		break;
		// 	}
		// }

		// private void OnAdNotAvailableCB(AdFormat adFormat)
		// {
		// 	switch (adFormat)
		// 	{
		// 	case AdFormat.INTERSTITIAL:
		// 		_CachedInterstitialAd = null;
		// 		SendAdLoadFailEvent(ADType.InterstitialAd);
		// 		break;
		// 	case AdFormat.REWARDED_VIDEO:
		// 		_CachedRewardedVideo = null;
		// 		SendAdLoadFailEvent(ADType.RewardedVideo);
		// 		break;
		// 	case AdFormat.OFFER_WALL:
		// 		_CachedOfferWall = null;
		// 		SendAdLoadFailEvent(ADType.OfferWall);
		// 		break;
		// 	}
		// }

		// private void OnRequestFailCB(RequestError error)
		// {
		// 	if (this.OnRequestError != null)
		// 	{
		// 		this.OnRequestError();
		// 	}
		// 	Debug.Log("-->OnRequestError: " + error.Description);
		// }

		// private void OnAdStartedCB(Ad ad)
		// {
		// 	switch (ad.AdFormat)
		// 	{
		// 	case AdFormat.INTERSTITIAL:
		// 		SendAdStartedEvent(ADType.InterstitialAd);
		// 		break;
		// 	case AdFormat.REWARDED_VIDEO:
		// 		SendAdStartedEvent(ADType.RewardedVideo);
		// 		break;
		// 	case AdFormat.OFFER_WALL:
		// 		SendAdStartedEvent(ADType.OfferWall);
		// 		break;
		// 	}
		// }

		// private void OnAdFinishedCB(AdResult result)
		// {
		// 	ADType adTypeFromAdResult = GetAdTypeFromAdResult(result);
		// 	ADFinishedStatus adFinishedStatusFromAdResult = GetAdFinishedStatusFromAdResult(result);
		// 	if (adTypeFromAdResult != ADType.Unknown)
		// 	{
		// 		SendAdFinishedEvent(adTypeFromAdResult, adFinishedStatusFromAdResult);
		// 	}
		// }

		// private ADType GetAdTypeFromAdResult(AdResult p_adResult)
		// {
		// 	switch (p_adResult.AdFormat)
		// 	{
		// 	case AdFormat.INTERSTITIAL:
		// 		return ADType.InterstitialAd;
		// 	case AdFormat.REWARDED_VIDEO:
		// 		return ADType.RewardedVideo;
		// 	case AdFormat.OFFER_WALL:
		// 		return ADType.OfferWall;
		// 	default:
		// 		return ADType.Unknown;
		// 	}
		// }

		// private ADFinishedStatus GetAdFinishedStatusFromAdResult(AdResult p_adResult)
		// {
		// 	switch (p_adResult.Message)
		// 	{
		// 	case "CLOSE_FINISHED":
		// 		return ADFinishedStatus.CloseFinished;
		// 	case "CLOSE_ABORTED":
		// 		return ADFinishedStatus.CloseAborted;
		// 	default:
		// 		return ADFinishedStatus.Error;
		// 	}
		// }

		// public void OnCurrencyResponse(VirtualCurrencyResponse response)
		// {
		// 	SendVirtualCurrencyRecived(response.CurrencyId, (float)response.DeltaOfCoins);
		// }

		// public void OnCurrencyErrorResponse(VirtualCurrencyErrorResponse vcsError)
		// {
		// 	Debug.Log(string.Format("Delta of coins request failed.\nError Type: {0}\nError Code: {1}\nError Message: {2}", vcsError.Type, vcsError.Code, vcsError.Message));
		// }

		// public void HandlePlacementRequestSuccess(TJPlacement placement)
		// {
		// 	if (placement.IsContentAvailable() && placement.GetName() == _TapjoyOfferwallName)
		// 	{
		// 		_isTJOfferWallReady = true;
		// 		SendAdReadyEvent(ADType.TapjoyOfferWall);
		// 	}
		// }

		// public void HandlePlacementRequestFailure(TJPlacement placement, string error)
		// {
		// 	if (placement.GetName() == _TapjoyOfferwallName)
		// 	{
		// 		_isTJOfferWallReady = false;
		// 		SendAdLoadFailEvent(ADType.TapjoyOfferWall);
		// 	}
		// }

		// public void HandlePlacementContentShow(TJPlacement placement)
		// {
		// 	if (placement.GetName() == _TapjoyOfferwallName)
		// 	{
		// 		SendAdStartedEvent(ADType.TapjoyOfferWall);
		// 	}
		// }

		// public void HandlePlacementContentDismiss(TJPlacement placement)
		// {
		// 	if (placement.GetName() == _TapjoyOfferwallName)
		// 	{
		// 		SendAdFinishedEvent(ADType.TapjoyOfferWall, ADFinishedStatus.CloseFinished);
		// 	}
		// }

		// public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
		// {
		// 	SendVirtualCurrencyRecived(currencyName, balance);
		// 	Tapjoy.SpendCurrency(balance);
		// }

		// public void CallReciveMoneyAction()
		// {
		// 	if (this.OnReciveMoney != null)
		// 	{
		// 		this.OnReciveMoney();
		// 	}
		// }

		// public void CallOverrideMetodOnRevardVideo()
		// {
		// 	if (this.OverrideMetodOnRevardVideo != null)
		// 	{
		// 		this.OverrideMetodOnRevardVideo();
		// 	}
		// }
	}
}
