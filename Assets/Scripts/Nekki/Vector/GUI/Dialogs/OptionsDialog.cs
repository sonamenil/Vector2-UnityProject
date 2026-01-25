using System;
using System.Collections.Generic;
using System.Linq;
using Nekki.Vector.Core;
using Nekki.Vector.Core.ABTest;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class OptionsDialog : BaseDialog
	{
		[SerializeField]
		private Image _Background;

		[SerializeField]
		private GameObject _GameView;

		[SerializeField]
		private Toggle2Image _OptimGraphics;

		[SerializeField]
		private Toggle2Image _GPG;

		[SerializeField]
		private Toggle2Image _Subtitles;

		[SerializeField]
		private Toggle2Image _MusicMute;

		[SerializeField]
		private Toggle2Image _SoundMute;

		[SerializeField]
		private Slider _MusicSlider;

		[SerializeField]
		private Slider _SoundSlider;

		[SerializeField]
		private Dropdown _SelectLanguage;

		[SerializeField]
		private ButtonUI _RestorePurchasesButton;

		[SerializeField]
		private GameObject _Blocker;

		[SerializeField]
		private LoadingCircle _LoadingCircle;

		[SerializeField]
		private Text _Version;

		[SerializeField]
		private Text _UserID;

		[SerializeField]
		private Button _CreditsButton;

        [SerializeField]
        private GameObject _CreditsObject;

        [SerializeField]
		private ButtonUI _BuyOSTButton;

		private List<SystemLanguage> _Languages = new List<SystemLanguage>();

        private List<Resolution> _Resolutions = new List<Resolution>();

        private int _CurrentLanguage;

        private int _CurrentResolution;

        private static OptionsDialog _Current;

		private float _SoundValue;

		private float _MusicValue;

		[SerializeField]
		private GameObject _DebugView;

		[SerializeField]
		private GameObject _DebugButtons;

		[SerializeField]
		private Toggle2Image _ShowDebugGUI;

		[SerializeField]
		private Toggle2Image _ShowFPS;

		[SerializeField]
		private Toggle2Image _ShowAverageFPS;

		[SerializeField]
		private Toggle2Image _PromtExitOnDeath;

		[SerializeField]
		private Toggle2Image _WriteGeneratorLogs;

		[SerializeField]
		private Toggle2Image _WriteRunLogs;

		[SerializeField]
		private Toggle2Image _PlayInBackground;

		[SerializeField]
		private Toggle2Image _ReloadMovesOnRestart;

		[SerializeField]
		private Toggle2Image _AutosaveDemo;

		[SerializeField]
		private Toggle2Image _SaveMeGodMode;

		[SerializeField]
		private Toggle2Image _Fullscreen;

		[SerializeField]
		private Dropdown _ResolutionsDropdown;

		public static OptionsDialog Current
		{
			get
			{
				return _Current;
			}
		}

		public static event System.Action OnOpen;

		public static event System.Action OnClose;

		public static event System.Action OnReturnToGameButton;

		static OptionsDialog()
		{
			OptionsDialog.OnOpen = delegate
			{
			};
			OptionsDialog.OnClose = delegate
			{
			};
			OptionsDialog.OnReturnToGameButton = delegate
			{
			};
		}

		public void Init()
		{
			_Current = this;
			_Background.gameObject.SetActive(Manager.IsRun);
			_CreditsObject.SetActive(!Manager.IsRun);
			_CreditsButton.gameObject.SetActive(!Manager.IsRun);
			_BuyOSTButton.gameObject.SetActive(!string.IsNullOrEmpty(UrlManager.OSTUrl));
			InitLanguages();
			InitResolutions();
			InitUserData();
			InitSocial();
			InitMusic();
			InitSound();
			InitPayment();
			UpdateGraphics();
			UpdateSubtitles();
			UpdateFullscreen();
			UpdateMusicUI();
			UpdateSoundUI();
			UpdateSocialUI(GameCenterController.IsSignedIn());
			InitDebug();
			OptionsDialog.OnOpen();
		}

		public void ReturnToGameButton()
		{
			OptionsDialog.OnReturnToGameButton();
			if (_Current != null)
			{
				GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(UpdateSocialUI));
				ResetPayment();
				Dismiss();
				_Current = null;
			}
		}

		public void Close()
		{
			OptionsDialog.OnClose();
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(UpdateSocialUI));
			ResetPayment();
			Dismiss();
			_Current = null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_Current != null)
			{
				GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(UpdateSocialUI));
				ResetPayment();
				_Current = null;
			}
		}

		private void InitLanguages()
		{
			_Languages = LocalizationManager.SupportedLanguages;
			_CurrentLanguage = _Languages.IndexOf(LocalizationManager.CurrentLanguage);
			List<string> list = new List<string>();
			foreach (SystemLanguage language in _Languages)
			{
				list.Add(language.ToString());
			}
			_SelectLanguage.AddOptions(list);
			_SelectLanguage.value = _CurrentLanguage;
		}

		private void InitResolutions()
		{
            _ResolutionsDropdown.ClearOptions();
			_CurrentResolution = ResolutionManager.CurrentResolutionIndex;
            List<string> list = new List<string>();
            foreach (Resolution resolution in ResolutionManager.UsedResolutions)
            {
                list.Add(resolution.width + "x" + resolution.height + " " + resolution.refreshRate + " Hz");
            }
            _ResolutionsDropdown.AddOptions(list);
            _ResolutionsDropdown.value = _CurrentResolution;

        }

		private void InitUserData()
		{
			string text = "v:" + ApplicationController.BuildPlusGamedataVersion;
			if (!string.IsNullOrEmpty(ABTestManager.UserABGroup))
			{
				text += "a";
			}
			_Version.text = text;
			_UserID.text = "Made by @Vision";
		}

		private void InitSocial()
		{
			_GPG.gameObject.transform.parent.gameObject.SetActive(GameCenterController.IsSupported() && !DeviceInformation.IsiOS);
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(UpdateSocialUI));
		}

		private void InitMusic()
		{
			UpdateMusicUI();
		}

		private void InitSound()
		{
			UpdateSoundUI();
		}

		private void InitPayment()
		{
			_RestorePurchasesButton.gameObject.SetActive(!ApplicationController.IsPaidBundleID);
			PaymentAbstract current = PaymentController.Current;
			current.OnRestorePurchasesSuccess = (System.Action)Delegate.Combine(current.OnRestorePurchasesSuccess, new System.Action(OnRestorePurchasesSuccess));
			PaymentAbstract current2 = PaymentController.Current;
			current2.OnRestorePurchasesFailed = (Action<string>)Delegate.Combine(current2.OnRestorePurchasesFailed, new Action<string>(OnRestorePurchasesFailed));
			PaymentAbstract current3 = PaymentController.Current;
			current3.OnPurchaseSuccess = (Action<Product>)Delegate.Combine(current3.OnPurchaseSuccess, new Action<Product>(OnPurchaseSuccess));
			Unblock();
		}

		private void ResetPayment()
		{
			PaymentAbstract current = PaymentController.Current;
			current.OnRestorePurchasesSuccess = (System.Action)Delegate.Remove(current.OnRestorePurchasesSuccess, new System.Action(OnRestorePurchasesSuccess));
			PaymentAbstract current2 = PaymentController.Current;
			current2.OnRestorePurchasesFailed = (Action<string>)Delegate.Remove(current2.OnRestorePurchasesFailed, new Action<string>(OnRestorePurchasesFailed));
			PaymentAbstract current3 = PaymentController.Current;
			current3.OnPurchaseSuccess = (Action<Product>)Delegate.Remove(current3.OnPurchaseSuccess, new Action<Product>(OnPurchaseSuccess));
		}

		private void ShowNotification(string p_text)
		{
			Notification.Parameters parameters = new Notification.Parameters();
			parameters.HideBy = Notification.HideBy.TimeDontBlockClicks;
			parameters.Image = string.Empty;
			parameters.Orientation = Notification.Orientation.Top;
			parameters.Text = p_text;
			parameters.QueueType = DialogQueueType.Notification;
			DialogNotificationManager.ShowSimpleNotification(parameters);
		}

		private void Block()
		{
			_Blocker.gameObject.SetActive(true);
			_LoadingCircle.Play();
		}

		private void Unblock()
		{
			_LoadingCircle.Stop();
			_Blocker.gameObject.SetActive(false);
		}

		public void UpdateGraphics()
		{
			_OptimGraphics.State = DataLocal.Current.Settings.UseLowResGraphics;
		}

		public void UpdateSubtitles()
		{
			_Subtitles.State = DataLocal.Current.Settings.SubtitlesOn;
		}

        public void UpdateFullscreen()
        {
            _Fullscreen.State = DataLocal.Current.Settings.Fullscreen;
        }

        public void UpdateMusicUI()
		{
			_MusicSlider.onValueChanged.SetPersistentAllListenersState(UnityEventCallState.Off);
			_MusicSlider.value = ((!DataLocal.Current.Settings.MuteMusic) ? DataLocal.Current.Settings.VolumeMusic : 0f);
			_MusicMute.State = !DataLocal.Current.Settings.MuteMusic;
			_MusicSlider.onValueChanged.SetPersistentAllListenersState(UnityEventCallState.RuntimeOnly);
		}

		public void UpdateSoundUI()
		{
			_SoundSlider.onValueChanged.SetPersistentAllListenersState(UnityEventCallState.Off);
			_SoundSlider.value = ((!DataLocal.Current.Settings.MuteSound) ? DataLocal.Current.Settings.VolumeSound : 0f);
			_SoundMute.State = !DataLocal.Current.Settings.MuteSound;
			_SoundSlider.onValueChanged.SetPersistentAllListenersState(UnityEventCallState.RuntimeOnly);
		}

		public void UpdateSocialUI(bool p_state)
		{
			_GPG.State = p_state;
		}

		public void UpdateDebugUI()
		{
			InitDebugToggleStates();
		}

		public void OnSoundVolumeChange(float p_value)
		{
			if (DataLocal.Current.Settings.VolumeSound != p_value)
			{
				DebugUtils.LogFormat("OnSoundVolumeChange {0}", p_value);
				AudioManager.SoundVolume = p_value;
				if (_SoundSlider.value > 0f && DataLocal.Current.Settings.MuteSound)
				{
					ChangeSoundMute(false);
				}
				else if (_SoundSlider.value == 0f && !DataLocal.Current.Settings.MuteSound)
				{
					ChangeSoundMute(true);
				}
			}
		}

		public void OnMusicVolumeChange(float p_value)
		{
			if (DataLocal.Current.Settings.VolumeMusic != p_value)
			{
				DebugUtils.LogFormat("OnMusicVolumeChange {0}", p_value);
				AudioManager.MusicVolume = p_value;
				if (p_value > 0f && DataLocal.Current.Settings.MuteMusic)
				{
					ChangeMusicMute(false);
				}
				else if (_MusicSlider.value == 0f && !DataLocal.Current.Settings.MuteMusic)
				{
					ChangeMusicMute(true);
				}
			}
		}

		public void OnSoundMuteChange(bool p_value)
		{
			DebugUtils.LogFormat("OnSoundMuteChange {0}", p_value);
			if (DataLocal.Current.Settings.MuteSound && DataLocal.Current.Settings.VolumeSound == 0f)
			{
				AudioManager.SoundVolume = DataLocal.Current.Settings.VolumeSound;
			}
			ChangeSoundMute(!DataLocal.Current.Settings.MuteSound);
		}

		public void OnMusicMuteChange(bool p_value)
		{
			DebugUtils.LogFormat("OnMusicMuteChange {0}", p_value);
			if (DataLocal.Current.Settings.MuteMusic && DataLocal.Current.Settings.VolumeMusic == 0f)
			{
				AudioManager.MusicVolume = DataLocal.Current.Settings.VolumeMusic;
			}
			ChangeMusicMute(!DataLocal.Current.Settings.MuteMusic);
		}

		private void ChangeMusicMute(bool p_isMute)
		{
			DataLocal.Current.Settings.MuteMusic = p_isMute;
			DataLocal.Current.SaveForcedToFile();
			if (p_isMute)
			{
				AudioManager.MuteMusic();
			}
			else
			{
				AudioManager.UnMuteMusic();
			}
			UpdateMusicUI();
		}

		private void ChangeSoundMute(bool p_isMute)
		{
			DataLocal.Current.Settings.MuteSound = p_isMute;
			DataLocal.Current.SaveForcedToFile();
			if (p_isMute)
			{
				AudioManager.MuteSounds();
			}
			else
			{
				AudioManager.UnMuteSounds();
			}
			UpdateSoundUI();
		}

		public void OnOptimizedGraphicsChange(bool p_value)
		{
			DebugUtils.LogFormat("OnOptimizedGraphicsChange {0}", p_value);
			DataLocal.Current.Settings.UseLowResGraphics = !DataLocal.Current.Settings.UseLowResGraphics;
			DataLocal.Current.SaveForcedToFile();
			DialogNotificationManager.ShowQualityChangeDialog(this);
		}

		public void OnOGPGChange(bool p_value)
		{
			DebugUtils.LogFormat("OnOGPGChange {0}", p_value);
			if (GameCenterController.IsSignedIn())
			{
				GameCenterController.SignOut();
			}
			else
			{
				GameCenterController.SignIn();
			}
		}

		public void OnSubtitlesChange(bool p_value)
		{
			DebugUtils.LogFormat("OnSubtitlesChange {0}", p_value);
			DataLocal.Current.Settings.SubtitlesOn = p_value;
			DataLocal.Current.Save(false);
			DataLocal.Current.SaveForcedToFile();
		}

        public void OnFullscreenChange(bool p_value)
        {
            DebugUtils.LogFormat("OnFullscreenChange {0}", p_value);
            DataLocal.Current.Settings.Fullscreen = p_value;
			Screen.SetResolution(ResolutionManager.UsedResolutions[ResolutionManager.CurrentResolutionIndex].width, ResolutionManager.UsedResolutions[ResolutionManager.CurrentResolutionIndex].height, p_value);
            DataLocal.Current.Save(false);
            DataLocal.Current.SaveForcedToFile();
        }

        public void OnLanguageChange()
		{
			AudioManager.PlaySound("blue_button");
			_CurrentLanguage++;
			if (_CurrentLanguage >= _Languages.Count)
			{
				_CurrentLanguage = 0;
			}
			LocalizationManager.CurrentLanguage = _Languages[_CurrentLanguage];
		}

		public void OnResolutionChange(int p_value)
		{
            AudioManager.PlaySound("blue_button");
			Resolution resolution = ResolutionManager.UsedResolutions[p_value];
			ResolutionManager.CurrentResolution = resolution;
        }

		public void OnCreditsClicked()
		{
			Close();
			Manager.OpenCredits();
		}

		public void OnFAQClicked()
		{
			ApplicationController.OpenURL(UrlManager.FAQUrl);
		}

		public void OnRestorePurchasesClicked()
		{
			Block();
			PaymentController.Current.RestorePurchases();
		}

		public void OnBuyOSTClicked()
		{
			ApplicationController.OpenURL(UrlManager.OSTUrl);
		}

		public void OnKeyDown(KeyCode p_code)
		{
			if (p_code == KeyCode.Escape && DeviceInformation.IsAndroid && !_Blocker.gameObject.activeSelf)
			{
				Close();
			}
		}

		private void OnRestorePurchasesSuccess()
		{
			Unblock();
		}

		private void OnRestorePurchasesFailed(string p_error)
		{
			ShowNotification("^Payment.Events.Failed^");
			Unblock();
		}

		private void OnPurchaseSuccess(Product p_product)
		{
			ShowNotification("^Payment.Events.Success^");
			Unblock();
		}

		private void InitDebug()
		{
			_GameView.SetActive(true);
			_DebugView.SetActive(false);
			if (Settings.IsReleaseBuild)
			{
				_DebugButtons.SetActive(false);
				return;
			}
			_DebugButtons.SetActive(true);
			InitDebugToggleStates();
		}

		private void InitDebugToggleStates()
		{
			_ShowDebugGUI.State = Settings.Visual.ShowDebugGUI;
			_ShowFPS.State = Settings.Visual.ShowFPS;
			_ShowAverageFPS.State = Settings.Visual.ShowAverageFPS;
			_PromtExitOnDeath.State = Settings.PromtExitOnDeath;
			_WriteGeneratorLogs.State = Settings.WriteGeneratorLogs;
			_WriteRunLogs.State = Settings.WriteRunLogs;
			_PlayInBackground.State = Settings.PlayInBackground;
			_ReloadMovesOnRestart.State = Settings.ReloadMovesOnRestart;
			_AutosaveDemo.State = Settings.AutosaveDemo;
			_SaveMeGodMode.State = Settings.SaveMeGodMode;
		}

		public void OnGameOptionsClicked()
		{
			SwitchToGameView();
		}

		public void OnDebugOptionsClicked()
		{
			SwitchToDebugView();
		}

		public void OnSkipTutorialClicked()
		{
			UserSwitcher.SwitchToTutorialEndUser();
			DataLocal.Current.CounterController.CounterFloor = 0;
		}

		public void OnResetProgressClicked()
		{
			ApplicationController.ResetAllProgress();
		}

		private void SwitchToDebugView()
		{
			_GameView.SetActive(false);
			_DebugView.SetActive(true);
		}

		private void SwitchToGameView()
		{
			_GameView.SetActive(true);
			_DebugView.SetActive(false);
		}

		public void OnShowDebugGUIChange(bool p_value)
		{
			Settings.Visual.ShowDebugGUI = p_value;
			Settings.Save();
			if (DebugUI.RunDebugPanel != null)
			{
				DebugUI.RunDebugPanel.UpdateVisiblity();
			}
		}

		public void OnShowFPSChange(bool p_value)
		{
			Settings.Visual.ShowFPS = p_value;
			Settings.Save();
			if (DebugUI.FPSMeter != null)
			{
				DebugUI.FPSMeter.UpdateVisiblity();
			}
			if (DebugUI.RunFPSMeter != null)
			{
				DebugUI.RunFPSMeter.UpdateVisiblity();
			}
		}

		public void OnShowAverageFPSChange(bool p_value)
		{
			Settings.Visual.ShowAverageFPS = p_value;
			Settings.Save();
		}

		public void OnPromtExitOnDeathChange(bool p_value)
		{
			Settings.PromtExitOnDeath = p_value;
			Settings.Save();
		}

		public void OnWriteGeneratorLogsChange(bool p_value)
		{
			Settings.WriteGeneratorLogs = p_value;
			Settings.Save();
		}

		public void OnWriteRunLogsChange(bool p_value)
		{
			Settings.WriteRunLogs = p_value;
			Settings.Save();
		}

		public void OnPlayInBackgroundChange(bool p_value)
		{
			Application.runInBackground = (Settings.PlayInBackground = p_value);
			Settings.Save();
		}

		public void OnReloadMovesOnRestartChange(bool p_value)
		{
			Settings.ReloadMovesOnRestart = p_value;
			Settings.Save();
		}

		public void OnAutosaveDemoChange(bool p_value)
		{
			Settings.AutosaveDemo = p_value;
			Settings.Save();
		}

		public void OnSaveMeGodModeChange(bool p_value)
		{
			Settings.SaveMeGodMode = p_value;
			Settings.Save();
		}
	}
}
