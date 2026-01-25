using System;
using System.Collections.Generic;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Dialogs.Payment;
using Nekki.Vector.GUI.Scenes.Terminal;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class BottomPanel : UIModule
	{
		[SerializeField]
		private UIPoligon _Background;

		[SerializeField]
		private List<GameObject> _AllButtons;

		[SerializeField]
		private List<GameObject> _HideOnShop;

		[SerializeField]
		private List<GameObject> _HideOnMain;

		[SerializeField]
		private List<GameObject> _HideOnTerminal;

		[SerializeField]
		private Button _AchivementsButton;

		[SerializeField]
		private GameObject _StartButton;

		[SerializeField]
		private GameObject _ContinueButton;

		[SerializeField]
		private GameObject _BackButton;

		[SerializeField]
		private GameObject _BannerButtonAnchor;

		[SerializeField]
		private MultiStateButtonUI _MultiStateButtonPrefab;

		[SerializeField]
		private LabelAlias _LevelNumber;

		[SerializeField]
		private GameObject _ArchiveNewAnnounce;

		[SerializeField]
		private GameObject _ArchiveLevelUpAnnounce;

		[SerializeField]
		private GameObject _QuestLogAnnounce;

		[SerializeField]
		private GameObject _MissionsBtn;

		[SerializeField]
		private GameObject _BoosterpackAnnounce;

		private float _BackgroundHeightDelta = 45f;

		private System.Action _BackButtonCallback;

		private bool _IsInSceneScreen = true;

		public UIPoligon Background
		{
			get
			{
				return _Background;
			}
		}

		public System.Action BackButtonCallback
		{
			get
			{
				return _BackButtonCallback;
			}
			set
			{
				_BackButtonCallback = value;
			}
		}

		public bool IsInSceneScreen
		{
			get
			{
				return _IsInSceneScreen;
			}
		}

		public bool IsInSecondaryScreen
		{
			get
			{
				return !_IsInSceneScreen;
			}
		}

		protected override void Init()
		{
			base.Init();
			UpdateAchivementsButton();
			if (Manager.IsEquip)
			{
				InitMainMenuMode();
			}
			else if (Manager.IsShop)
			{
				InitShopMode();
			}
			else
			{
				InitTerminalMode();
			}
			ChangeBG();
			InitBannerButton();
			UpdateArchiveAnnounce();
			UpdateQuestLogAnnounce();
			if (Manager.IsEquip || Manager.IsShop)
			{
				UpdateBoosterpackAnnounce();
			}
			PaymentDialog.OnClose += OnPaymentClose;
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnGameCenterAuthenticate));
		}

		protected override void Free()
		{
			base.Free();
			PaymentDialog.OnClose -= OnPaymentClose;
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnGameCenterAuthenticate));
		}

		private void InitBannerButton()
		{
			MultiStateButtonUI multiStateButtonUI = UnityEngine.Object.Instantiate(_MultiStateButtonPrefab);
			MultiStateButtonUIParameters multiStateButtonUIParameters = new MultiStateButtonUIParameters();
			multiStateButtonUIParameters.ChangeStateAnimationTime = 2f;
			multiStateButtonUIParameters.ShowTime = 5f;
			List<ButtonData> list = new List<ButtonData>();
			list.Add(new ButtonData("main_menu.sf2_promo", OnBannerSF2Tap));
			list.Add(new ButtonData("main_menu.sf3_promo", OnBannerSF3Tap));
			multiStateButtonUI.Init(multiStateButtonUIParameters, list);
			multiStateButtonUI.transform.SetParent(_BannerButtonAnchor.transform, false);
		}

		private void InitShopMode()
		{
			SetFloorLabel();
			_ContinueButton.SetActive(false);
			_StartButton.SetActive(true);
			_AchivementsButton.interactable = GameCenterController.IsSignedIn();
			SetStateButtons(_HideOnShop, false);
			_BannerButtonAnchor.SetActive(false);
			_MissionsBtn.SetActive((int)CounterController.Current.CounterMissionsBlock == 0);
		}

		private void InitMainMenuMode()
		{
			_ContinueButton.SetActive(false);
			_StartButton.SetActive(false);
			_MissionsBtn.SetActive(false);
			_AchivementsButton.interactable = GameCenterController.IsSignedIn();
			SetStateButtons(_HideOnMain, false);
			_BannerButtonAnchor.SetActive((int)CounterController.Current.СounterTutorialInProgress == 0);
		}

		private void InitTerminalMode()
		{
			_ContinueButton.SetActive(true);
			_StartButton.SetActive(false);
			_AchivementsButton.gameObject.SetActive(false);
			SetStateButtons(_HideOnTerminal, false);
			_BannerButtonAnchor.SetActive(false);
		}

		public void SetFloorLabel()
		{
			_LevelNumber.SetAlias(((int)CounterController.Current.CounterFloor >= 9) ? ((int)CounterController.Current.CounterFloor + 1).ToString() : ("0" + ((int)CounterController.Current.CounterFloor + 1)));
		}

		private void ChangeBG()
		{
			int num = 0;
			for (int i = 0; i < _AllButtons.Count; i++)
			{
				if (_AllButtons[i].activeSelf)
				{
					num++;
				}
			}
			float num2 = 40f;
			float num3 = num * 200 / 2;
			_Background.Points[3] = new Vector2(0f - num3 - num2, _Background.Points[3].y);
			if (num > 0)
			{
				_Background.Points[4] = new Vector2(0f - num3, _Background.Points[3].y + _BackgroundHeightDelta);
				_Background.Points[5] = new Vector2(num3, _Background.Points[6].y + _BackgroundHeightDelta);
			}
			else
			{
				_Background.Points[4] = new Vector2(0f - num3, _Background.Points[3].y);
				_Background.Points[5] = new Vector2(num3, _Background.Points[6].y);
			}
			_Background.Points[6] = new Vector2(num3 + num2, _Background.Points[6].y);
			_Background.Refresh();
		}

		public void SwitchToNormalMode()
		{
			SetStateButtons(_AllButtons, true);
			UpdateAchivementsButton();
			if (Manager.IsEquip)
			{
				InitMainMenuMode();
			}
			else if (Manager.IsShop)
			{
				InitShopMode();
			}
			_BackButton.SetActive(false);
			_IsInSceneScreen = true;
			ChangeBG();
		}

		public void SwitchToBackMode()
		{
			_BackButtonCallback = null;
			SetStateButtons(_AllButtons, false);
			_StartButton.SetActive(false);
			ChangeBG();
			_IsInSceneScreen = false;
			_BackButton.SetActive(true);
			_BannerButtonAnchor.SetActive(false);
		}

		public void OnPlayNextFloorTap()
		{
			Manager.PlayNextFloor();
			Manager.IsFakeFloor = false;
		}

		public void OnContinueTap()
		{
			Scene<TerminalScene>.Current.ContinueTap();
		}

		public void OnBackTap()
		{
			UpdateArchiveAnnounce();
			UpdateQuestLogAnnounce();
			UpdateBoosterpackAnnounce();
			if (_BackButtonCallback != null)
			{
				_BackButtonCallback();
			}
			else
			{
				OpenSceneScreen();
			}
		}

		public void OpenSceneScreen()
		{
			if (_BackButton.activeSelf)
			{
				Manager.OpenMainScreen();
			}
		}

		public void OnBackBtnClick()
		{
			if (_BackButton.activeSelf)
			{
				Button component = _BackButton.GetComponent<Button>();
				if (component.interactable)
				{
					component.onClick.Invoke();
				}
			}
		}

		public void OnGoogleAchivmentTap()
		{
			GameCenterController.ShowAchiements();
		}

		public void OnArchiveTap()
		{
			Manager.OpenArchiveCategory();
		}

		public void OnQuestLogTap()
		{
			Manager.OpenQuestLog();
		}

		public void OnBoosterpackTap()
		{
			Manager.OpenBoosterpack();
		}

		public void OnBannerSF2Tap()
		{
			ApplicationController.OpenURL(UrlManager.BannerSF2Url);
		}

		public void OnBannerSF3Tap()
		{
			ApplicationController.OpenURL(UrlManager.BannerSF3Url);
		}

		private void OnPaymentClose()
		{
			if (Manager.IsEquip || Manager.IsShop)
			{
				UpdateBoosterpackAnnounce();
			}
		}

		private void OnGameCenterAuthenticate(bool p_auth)
		{
			_AchivementsButton.interactable = p_auth;
		}

		public void UpdateArchiveAnnounce()
		{
			_ArchiveNewAnnounce.SetActive(false);
			_ArchiveLevelUpAnnounce.SetActive(false);
			if (DataLocalHelper.HasLevelUpCards)
			{
				_ArchiveLevelUpAnnounce.SetActive(true);
			}
			else if (DataLocalHelper.HasNewCards)
			{
				_ArchiveNewAnnounce.SetActive(true);
			}
		}

		public void UpdateQuestLogAnnounce()
		{
			bool hasNewQuest = QuestManager.Current.HasNewQuest;
			_QuestLogAnnounce.SetActive(hasNewQuest);
		}

		public void UpdateBoosterpackAnnounce()
		{
			bool active = BoosterpacksManager.BoosterpackQuantity > 0;
			_BoosterpackAnnounce.SetActive(active);
		}

		public void UpdateBannerButton()
		{
			if (_IsInSceneScreen && Manager.IsEquip)
			{
				_BannerButtonAnchor.SetActive((int)CounterController.Current.СounterTutorialInProgress == 0);
			}
		}

		private void UpdateAchivementsButton()
		{
			_AchivementsButton.gameObject.SetActive(GameCenterController.IsSupported());
		}

		private static void SetStateButtons(List<GameObject> p_buttons, bool p_state)
		{
			for (int i = 0; i < p_buttons.Count; i++)
			{
				p_buttons[i].SetActive(p_state);
			}
		}

		public void SetBackButtonActive(bool p_value)
		{
			_BackButton.gameObject.SetActive(p_value);
		}

		public void OnMissionsTap()
		{
			DialogNotificationManager.ShowMissionsDialog(false);
		}
	}
}
