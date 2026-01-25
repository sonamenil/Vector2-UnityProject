using System;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.News;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.DetailsScreen;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Journal;
using Nekki.Vector.GUI.Scenes.Archive;
using Nekki.Vector.GUI.Scenes.ArchiveCategory;
using Nekki.Vector.GUI.Scenes.Boosterpack;
using Nekki.Vector.GUI.Scenes.Credits;
using Nekki.Vector.GUI.Tutorial;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class MainScene : Scene<MainScene>
	{
		[SerializeField]
		private ModuleSwitcher _ModuleSwitcher;

		[SerializeField]
		private SceneBackground _Background;

		private ScreenType _PrevScreen;

		private ScreenType _CurrentScreen;

		private MainScreen _MainScreen;

		private TopPanel _TopPanel;

		private BottomPanel _BottomPanel;

		private UIModule _OpenedModule;

		public override SceneKind SceneId
		{
			get
			{
				return SceneKind.Main;
			}
		}

		public ScreenType PrevScreen
		{
			get
			{
				return _PrevScreen;
			}
		}

		public ScreenType CurrentScreen
		{
			get
			{
				return _CurrentScreen;
			}
			set
			{
				if (_CurrentScreen != value)
				{
					_PrevScreen = _CurrentScreen;
					_CurrentScreen = value;
				}
			}
		}

		private static int NoQuestProgressRunCount
		{
			get
			{
				int result = 0;
				int.TryParse(BalanceManager.Current.GetBalance("NoQuestProgressDialog", "RunCount"), out result);
				return result;
			}
		}

		protected override void BeforeMountModules()
		{
			base.BeforeMountModules();
			StarterPacksManager.SelectBestAvaliableStarterPack();
			GameManager.CheckPromo1Start();
		}

		protected override void Init()
		{
			base.Init();
			_MainScreen = GetModule<MainScreen>();
			_TopPanel = GetModule<TopPanel>();
			_BottomPanel = GetModule<BottomPanel>();
			RefreshBG();
			AudioManager.PlayRandomMenuMusic();
			QuestManager.Current.CheckEvent(TQE_OnScreen.ScreenStartEvent);
			if (GameRestorer.IsRestoreBoosterpacksAvailable)
			{
				SwitchBoosterpack();
			}
			else if (Manager.NeedOpenCredits)
			{
				SwitchCredits();
				GetModule<CreditsPanel>().PlayAnimation();
			}
			else
			{
				ShowDialogsOnStart();
			}
			TimersManager.UpdateAllTimers();
			DebugUtils.SceneLoadStopTimer();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Main_scene_enter);
		}

		protected override void Free()
		{
			base.Free();
		}

		public void Refresh()
		{
			RefreshBG();
			_MainScreen.Refresh();
		}

		public void RefreshBG()
		{
			_Background.AtlasName = Manager.ZoneBackground;
		}

		public void RefreshStarterPacksUI()
		{
			_MainScreen.RefreshStarterPacksUI();
		}

		public void Play()
		{
			_ModuleSwitcher.Switch(SwitchToRunScene, false);
		}

		private void SwitchToRunScene()
		{
			Manager.Play();
		}

		public System.Action OpenScreenByType(ScreenType p_screenType)
		{
			switch (p_screenType)
			{
			case ScreenType.Archive:
				return OpenArchive;
			case ScreenType.ArchiveCategory:
				return OpenArchiveCategory;
			case ScreenType.Boosterpack:
				return OpenBoosterpack;
			case ScreenType.Credits:
				return OpenCredits;
			case ScreenType.Details:
				return OpenDetails;
			case ScreenType.Journal:
				return OpenQuestLog;
			case ScreenType.Main:
				return OpenMainScreen;
			default:
				return delegate
				{
				};
			}
		}

		public void OpenMainScreen()
		{
			_ModuleSwitcher.Switch(SwitchToMainScreen, ShowDialogsOnStart);
		}

		private void SwitchToMainScreen()
		{
			CurrentScreen = ScreenType.Main;
			if (_OpenedModule != null)
			{
				_OpenedModule.DeActivate();
			}
			_MainScreen.Activate();
			_BottomPanel.SwitchToNormalMode();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		public void OpenDetails()
		{
			_ModuleSwitcher.Switch(SwitchDetails);
		}

		private void SwitchDetails()
		{
			CurrentScreen = ScreenType.Details;
			_BottomPanel.SwitchToBackMode();
			_MainScreen.DeActivate();
			_OpenedModule = GetModule<DetailsPanel>();
			_OpenedModule.Activate();
			_MainScreen.ReselectStarterPackToBest = false;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		public void OpenQuestLog()
		{
			OpenQuestLog(null);
		}

		public void OpenQuestLog(string p_selectedQuest)
		{
			JournalPanel module = GetModule<JournalPanel>();
			module.SelectedQuest = p_selectedQuest;
			_ModuleSwitcher.Switch(SwitchQuestLog);
		}

		private void SwitchQuestLog()
		{
			CurrentScreen = ScreenType.Journal;
			_BottomPanel.SwitchToBackMode();
			if (_MainScreen.IsActive)
			{
				_MainScreen.DeActivate();
			}
			if (_OpenedModule != null && _OpenedModule.IsActive)
			{
				_OpenedModule.DeActivate();
			}
			if (_PrevScreen != 0)
			{
				_BottomPanel.BackButtonCallback = OpenScreenByType(_PrevScreen);
			}
			_OpenedModule = GetModule<JournalPanel>();
			_OpenedModule.Activate();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		public void OpenArchiveCategory()
		{
			_ModuleSwitcher.Switch(SwitchArchiveCategory);
		}

		private void SwitchArchiveCategory()
		{
			CurrentScreen = ScreenType.ArchiveCategory;
			_BottomPanel.SwitchToBackMode();
			if (_MainScreen.IsActive)
			{
				_MainScreen.DeActivate();
			}
			if (_OpenedModule != null && _OpenedModule.IsActive)
			{
				_OpenedModule.DeActivate();
			}
			_OpenedModule = GetModule<ArchiveCategoryPanel>();
			_OpenedModule.Activate();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		public void OpenArchive()
		{
			OpenArchive(SlotItem.Slot.Head);
		}

		public void OpenArchive(SlotItem.Slot p_slot)
		{
			ArchivePanel module = GetModule<ArchivePanel>();
			module.CurrentSlot = p_slot;
			_ModuleSwitcher.Switch(SwitchArchive, OnArchiveSwitchOver);
		}

		private void SwitchArchive()
		{
			CurrentScreen = ScreenType.Archive;
			_BottomPanel.SwitchToBackMode();
			_BottomPanel.BackButtonCallback = OpenArchiveCategory;
			if (_MainScreen.IsActive)
			{
				_MainScreen.DeActivate();
			}
			if (_OpenedModule != null && _OpenedModule.IsActive)
			{
				_OpenedModule.DeActivate();
			}
			_OpenedModule = GetModule<ArchivePanel>();
			_OpenedModule.Activate();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		private void OnArchiveSwitchOver()
		{
			ArchivePanel module = GetModule<ArchivePanel>();
			module.TryRunPromo2();
		}

		public void OpenBoosterpack()
		{
			_ModuleSwitcher.Switch(SwitchBoosterpack);
		}

		private void SwitchBoosterpack()
		{
			CurrentScreen = ScreenType.Boosterpack;
			_BottomPanel.SwitchToBackMode();
			if (_MainScreen.IsActive)
			{
				_MainScreen.DeActivate();
			}
			if (_OpenedModule != null && _OpenedModule.IsActive)
			{
				_OpenedModule.DeActivate();
			}
			BoosterpackPanel module = GetModule<BoosterpackPanel>();
			_BottomPanel.BackButtonCallback = module.OnBackButtonTap;
			_OpenedModule = module;
			_OpenedModule.Activate();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		public void OpenCredits()
		{
			_ModuleSwitcher.Switch(SwitchCredits, GetModule<CreditsPanel>().PlayAnimation);
		}

		private void SwitchCredits()
		{
			CurrentScreen = ScreenType.Credits;
			_TopPanel.DeActivate();
			_BottomPanel.Background.enabled = false;
			_BottomPanel.SwitchToBackMode();
			if (_MainScreen.IsActive)
			{
				_MainScreen.DeActivate();
			}
			_BottomPanel.BackButtonCallback = delegate
			{
				AudioManager.PlayRandomMenuMusic();
				_TopPanel.Activate();
				_BottomPanel.Background.enabled = true;
				OpenScreenByType(_PrevScreen)();
			};
			if (_OpenedModule != null && _OpenedModule.IsActive)
			{
				_OpenedModule.DeActivate();
			}
			_OpenedModule = GetModule<CreditsPanel>();
			_OpenedModule.Activate();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		public void OnKeyDown(KeyCode p_code)
		{
			switch (p_code)
			{
			case KeyCode.Escape:
				if (DeviceInformation.IsAndroid || DeviceInformation.IsEmulator && (Nekki.Vector.GUI.Tutorial.Tutorial.Current == null || !Nekki.Vector.GUI.Tutorial.Tutorial.Current.Started))
				{
					if (_BottomPanel.IsInSceneScreen)
					{
						DialogNotificationManager.ShowQuitDialog(0);
					}
					else
					{
						_BottomPanel.OnBackBtnClick();
					}
				}
				break;
			case KeyCode.L:
			{
				int num = DataLocalHelper.MakeCardsLevelUp();
				if (num > 0)
				{
					ArchivePanel module = UIModule.GetModule<ArchivePanel>();
					if (module != null && module.IsActive)
					{
						module.Refresh();
					}
					ArchiveCategoryPanel module2 = UIModule.GetModule<ArchiveCategoryPanel>();
					if (module2 != null && module2.IsActive)
					{
						module2.Refresh();
					}
					_BottomPanel.UpdateArchiveAnnounce();
				}
				break;
			}
			}
		}

		private void ShowDialogsOnStart()
		{
			if ((int)CounterController.Current.CounterPaymentPromo == 1)
			{
				DialogNotificationManager.ShowSaleDialog(0, false, 7);
			}
			if ((int)CounterController.Current.CounterPaymentPromo2 == 1)
			{
				DialogNotificationManager.ShowSaleDialog(1, false, 7);
			}
			NewsManager.ShowNews();
			if ((int)CounterController.Current.CounterShowNoQuestProgressDialog >= NoQuestProgressRunCount)
			{
				DialogNotificationManager.ShowNoQuestProgressDialog(0);
			}
		}

		public void SwitchZone(Zone p_zone)
		{
			_ModuleSwitcher.Switch(delegate
			{
				DebugUtils.StartTimer("SwitchZone");
				ZoneManager.CurrentZone = p_zone;
				Refresh();
				Resources.UnloadUnusedAssets();
				GC.Collect();
				DebugUtils.StopTimerWithMessage("SwitchZone: " + p_zone, "SwitchZone");
			}, 0.3f, 0.01f);
		}

		public void SwitchPage(System.Action p_updateAction)
		{
			_ModuleSwitcher.Switch(p_updateAction, 0.3f, 0.01f);
		}
	}
}
