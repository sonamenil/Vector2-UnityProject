using System;
using System.Collections;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using Nekki.Vector.GUI.Scenes.Run;
using UnityEngine;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerSaveMe
	{
		private const float _CameraMoveTime = 2.5f;

		private const float _ModelEffectTime = 1f;

		private const float _CameraReconnectTime = 0.2f;

		private const float _CountdownTime = 3f;

		private ModelHuman _ModelHuman;

		private float _Time;

		private BaseUIDialog _CurrentDialog;

		private bool _IsVideoRewardRecieved;

		private IEnumerator _RessurectRoutine;

		private IEnumerator _AdsRoutine;

		public static int Attempt
		{
			get
			{
				return CounterController.Current.CounterSaveMeAttempt;
			}
			private set
			{
				CounterController.Current.CounterSaveMeAttempt = value;
			}
		}

		public static int SaveMePrice
		{
			get
			{
				int result = 0;
				int.TryParse(BalanceManager.Current.GetBalance("SaveMe", "Attempt", Attempt.ToString(), "Price"), out result);
				return result;
			}
		}

		public static int SaveMeLimit
		{
			get
			{
				int result = 0;
				int.TryParse(BalanceManager.Current.GetBalance("SaveMe", "Limit"), out result);
				return result;
			}
		}

		public static bool SaveMeGodMode
		{
			get
			{
				return Settings.SaveMeGodMode;
			}
			set
			{
				Settings.SaveMeGodMode = value;
				Settings.Save();
			}
		}

		public static bool IsSaveMeAvaliable
		{
			get
			{
				return SaveMeGodMode || ((int)CounterController.Current.CounterSavemeBlock == 0 && Attempt <= SaveMeLimit);
			}
		}

		public static event System.Action Show;

		public static event System.Action Close;

		public ControllerSaveMe()
		{
			PaymentDialog.OnClose += ShowSaveMeDialog;
		}

		public void End()
		{
			PaymentDialog.OnClose -= ShowSaveMeDialog;
			if (_RessurectRoutine != null)
			{
				CoroutineManager.Current.StopRoutine(_RessurectRoutine);
			}
			if (_AdsRoutine != null)
			{
				CoroutineManager.Current.StopRoutine(_AdsRoutine);
			}
		}

		public static void ResetAttemp()
		{
			CounterController.Current.CounterSaveMeAttempt = 1;
		}

		public void OnLoss(float p_time)
		{
			_ModelHuman = RunMainController.Models[0];
			_Time = p_time;
			if (Demo.IsPlaying || !IsSaveMeAvaliable)
			{
				RunMainController.Loss(ModelHuman.ModelState.Death, _ModelHuman, _Time);
				return;
			}
			RunMainController.IsPause(true, true);
			FloatingText.StopAllActive();
			FloatingText.FreeAllActive();
			VectorADSystem.Reinit();
			ShowSaveMeDialog();
		}

		private void TryResurrect(BaseDialog p_dialog)
		{
			if (TryBuy())
			{
				Resurrect();
			}
			else
			{
				ShowNotEnoughMoneyDialog();
			}
			p_dialog.Dismiss();
		}

		private void Resurrect()
		{
			GameRestorer.RemoveOnLaunch = false;
			Attempt++;
			RunMainController.IsPause(false, true);
			HudPanel module = UIModule.GetModule<HudPanel>();
			if (module != null)
			{
				module.CurrentUnpauseMode = HudPanel.UnpauseMode.Instant;
			}
			if (ControllerSaveMe.Close != null)
			{
				ControllerSaveMe.Close();
			}
			SpawnRunner nearestSaveMe = RunMainController.Location.ControllerSpawns.GetNearestSaveMe(RunMainController.Models[0].DeathPosition.x);
			_ModelHuman.ResurrectAt(nearestSaveMe);
			Nekki.Vector.Core.Camera.Camera.Current.ResetOffsets();
			Nekki.Vector.Core.Camera.Camera.Current.MoveToPosition(_ModelHuman, 2.5f);
			Nekki.Vector.Core.Camera.Camera.Current.Render();
			_RessurectRoutine = RessurectCoroutine();
			CoroutineManager.Current.StartRoutine(_RessurectRoutine);
		}

		private IEnumerator RessurectCoroutine()
		{
			yield return new WaitForTime(2.5f);
			Nekki.Vector.Core.Camera.Camera.Current.ResetFollowSpeedToDefault();
			Nekki.Vector.Core.Camera.Camera.Current.Stop();
			_ModelHuman.IsEnabled = true;
			_ModelHuman.ControllerControl.Enable = false;
			for (int i = 0; i < 3; i++)
			{
				_ModelHuman.Render(null);
			}
			_ModelHuman.PlayBurnLaserReversed();
			RunMainController.Render();
			HudPanel hudPanel = UIModule.GetModule<HudPanel>();
			if (hudPanel != null)
			{
				yield return new WaitForTime(1f);
				hudPanel.ShowCountdown();
				yield return new WaitForTime(3f);
				MakeModelRunFromInhibition();
				AudioManager.PauseMusic(false);
				yield return new WaitForTime(0.2f);
				Nekki.Vector.Core.Camera.Camera.Current.CameraNode = _ModelHuman.ModelObject.CameraNode;
			}
			_RessurectRoutine = null;
		}

		private void ShowSaveMeDialog(BaseDialog p_dialog)
		{
			ShowSaveMeDialog();
			p_dialog.Dismiss();
		}

		private void ShowSaveMeDialog()
		{
			if (ControllerSaveMe.Show != null)
			{
				ControllerSaveMe.Show();
			}
			GameRestorer.RemoveOnLaunch = true;
			if (SaveMeGodMode || Attempt == 1)
			{
				DialogNotificationManager.ShowSaveMeDialog(SaveMePrice.ToString(), TryResurrect, Loss);
			}
			else
			{
				DialogNotificationManager.ShowSaveMeDialog(SaveMePrice.ToString(), TryResurrect, Loss);
			}
		}

		private void ShowNotEnoughMoneyDialog()
		{
			DialogNotificationManager.ShowNotEnoughtCurrencyDialog(CurrencyType.Money3, ShowSaveMeDialog);
		}

		private void Loss(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
			RunMainController.Loss(ModelHuman.ModelState.Death, _ModelHuman, _Time);
		}

		private bool TryBuy()
		{
			int saveMePrice = SaveMePrice;
			if (CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.Saveme))
			{
				CouponsManager.SpendCoupon(CouponsManager.CouponType.Saveme);
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Save_me, new ArgsDict { { "coupon", true } });
				return true;
			}
			if ((int)DataLocal.Current.Money3 >= saveMePrice)
			{
				DataLocal current = DataLocal.Current;
				current.Money3 = (int)current.Money3 - saveMePrice;
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Save_me, new ArgsDict { { "coupon", false } });
				return true;
			}
			return false;
		}

		private void MakeModelRunFromInhibition()
		{
			_ModelHuman.ControllerControl.Enable = true;
			_ModelHuman.ControllerControl.ClearKey();
			_ModelHuman.ControllerControl.SetKeyVariable_force(new KeyVariables("Right"));
		}

		private void WatchVideo(BaseDialog p_dialog)
		{
			_CurrentDialog = p_dialog as BaseUIDialog;
			if (PermissionsChecker.ShowExplanationWithOpenSettings("android.permission.READ_PHONE_STATE", OnGranded, OnDenied, OnUserSkip) && PermissionsChecker.CheckPermission("android.permission.READ_PHONE_STATE", OnGranded, OnDenied, OnUserSkip))
			{
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "saveme_start" } });
				GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Free", "VideoFromSMDStart", 1L);
				_CurrentDialog.HideButtons();
				if (DeviceInformation.IsEmulator)
				{
					_CurrentDialog.Dismiss();
					Resurrect();
				}
				else
				{
					InitAdEvents();
					_IsVideoRewardRecieved = false;
					ADSystem.Current.RequestRewardedVideoWithVirtualCurrency(false, "currency_video");
				}
			}
		}

		private void OnAdReady(ADSystem.ADType p_type)
		{
			if (p_type == ADSystem.ADType.RewardedVideo)
			{
				GameRestorer.RemoveOnLaunch = false;
				ADSystem.Current.ShowRewardedVideo();
			}
		}

		private void OnAdLoadFail(ADSystem.ADType p_type)
		{
			OnRequestError();
		}

		private void OnAdFinished(ADSystem.ADType p_type, ADSystem.ADFinishedStatus p_status)
		{
			_AdsRoutine = WaitAndUnlock(8f, p_status == ADSystem.ADFinishedStatus.CloseFinished);
			CoroutineManager.Current.StartRoutine(_AdsRoutine);
		}

		private void OnRequestError()
		{
			GameRestorer.RemoveOnLaunch = true;
			FreeAdEvents();
			_CurrentDialog.ShowButtons();
			Notification.Parameters parameters = new Notification.Parameters();
			parameters.Image = string.Empty;
			parameters.Text = "^Payment.Events.Failed^";
			parameters.Orientation = Notification.Orientation.Top;
			parameters.HideBy = Notification.HideBy.TimeDontBlockClicks;
			parameters.QueueType = DialogQueueType.Notification;
			DialogNotificationManager.ShowSimpleNotification(parameters);
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "saveme_fail" } });
		}

		private void OnVideoEnd()
		{
			if (!_IsVideoRewardRecieved)
			{
				RecieveVideoReward();
			}
		}

		private void RecieveVideoReward()
		{
			_IsVideoRewardRecieved = true;
			_CurrentDialog.Dismiss();
			GameRestorer.RemoveOnLaunch = true;
			FreeAdEvents();
			GoogleAnalyticsV4.getInstance().LogEvent("Payment", "Free", "VideoFromSMDEnd", 1L);
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Free, new ArgsDict { { "type", "saveme_reward" } });
			Resurrect();
		}

		private IEnumerator WaitAndUnlock(float p_timeInSeconds, bool p_isVideoCompleted)
		{
			yield return new WaitForSeconds(p_timeInSeconds);
			if (!p_isVideoCompleted)
			{
				GameRestorer.RemoveOnLaunch = true;
				FreeAdEvents();
				_CurrentDialog.ShowButtons();
			}
			else if (p_isVideoCompleted && !_IsVideoRewardRecieved)
			{
				RecieveVideoReward();
			}
			_AdsRoutine = null;
		}

		private void InitAdEvents()
		{
			ADSystem.Current.OnAdReady += OnAdReady;
			ADSystem.Current.OnAdLoadFail += OnAdLoadFail;
			ADSystem.Current.OnRequestError += OnRequestError;
			ADSystem.Current.OnAdFinished += OnAdFinished;
			ADSystem.Current.OverrideMetodOnRevardVideo += OnVideoEnd;
		}

		private void FreeAdEvents()
		{
			ADSystem.Current.OnAdReady -= OnAdReady;
			ADSystem.Current.OnAdLoadFail -= OnAdLoadFail;
			ADSystem.Current.OnRequestError -= OnRequestError;
			ADSystem.Current.OnAdFinished -= OnAdFinished;
			ADSystem.Current.OverrideMetodOnRevardVideo -= OnVideoEnd;
		}

		private void OnGranded(string p_permishen)
		{
			WatchVideo(_CurrentDialog);
		}

		private void OnDenied(string p_permishen)
		{
			_CurrentDialog.ShowButtons();
		}

		private void OnUserSkip(string p_permishen)
		{
			_CurrentDialog.ShowButtons();
		}
	}
}
