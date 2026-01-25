using System;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Journal;
using Nekki.Vector.GUI.Scenes.Archive;
using Nekki.Vector.GUI.Scenes.ArchiveCategory;
using Nekki.Vector.GUI.Scenes.Boosterpack;
using Nekki.Vector.GUI.Scenes.Credits;
using Nekki.Vector.GUI.Scenes.Shop;
using Nekki.Vector.GUI.Tutorial;
using UnityEngine;

namespace Nekki.Vector.GUI.ShopScene
{
	public class ShopScene : Scene<ShopScene>
	{
		[SerializeField]
		private ModuleSwitcher _ModuleSwitcher;

		[SerializeField]
		private SceneBackground _Background;

		private ScreenType _PrevScreen;

		private ScreenType _CurrentScreen;

		private ShopPanel _ShopScreen;

		private TopPanel _TopPanel;

		private BottomPanel _BottomPanel;

		private UIModule _OpenedModule;

		public override SceneKind SceneId
		{
			get
			{
				return SceneKind.Shop;
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

		protected override void Init()
		{
			base.Init();
			_ShopScreen = GetModule<ShopPanel>();
			_TopPanel = GetModule<TopPanel>();
			_BottomPanel = GetModule<BottomPanel>();
			RefreshBG();
			AudioManager.PlayRandomMenuMusic();
			QuestManager.Current.CheckEvent(TQE_OnScreen.ScreenShopEvent);
			TimersManager.UpdateAllTimers();
			DebugUtils.SceneLoadStopTimer();
			if (LoaderScene.PrevScene == SceneKind.Run)
			{
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Shop_enter);
			}
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
				ShopDoYouLikeGameDialogIfNeed();
			}
		}

		protected override void Free()
		{
			base.Free();
		}

		public void RefreshBG()
		{
			_Background.AtlasName = Manager.ZoneBackground;
		}

		private void ShopDoYouLikeGameDialogIfNeed()
		{
			if ((int)CounterController.Current.CounterBoosterpacksBlock == 0 && (int)CounterController.Current.CounterStarterPackCoolness >= 2)
			{
				if ((int)CounterController.Current.CounterShowDoYouLikeGameDialog > 0 && (int)CounterController.Current.CounterShowDoYouLikeGameDialog <= (int)CounterController.Current.CounterRunCounter)
				{
					DialogNotificationManager.ShowDoYouLikeGameDialog(0);
				}
				else if ((int)CounterController.Current.CounterShowSetStarDialog > 0 && (int)CounterController.Current.CounterShowSetStarDialog <= (int)CounterController.Current.CounterRunCounter)
				{
					DialogNotificationManager.ShowSetStarDialog(0);
				}
			}
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
			case ScreenType.Journal:
				return OpenQuestLog;
			case ScreenType.Main:
				return OpenShopScreen;
			default:
				return delegate
				{
				};
			}
		}

		public void OpenShopScreen()
		{
			_ModuleSwitcher.Switch(SwitchToShopScreen);
		}

		private void SwitchToShopScreen()
		{
			CurrentScreen = ScreenType.Main;
			_ShopScreen.RefreshGadgetInfoAndCards();
			_OpenedModule.DeActivate();
			_ShopScreen.Activate();
			_BottomPanel.SwitchToNormalMode();
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
			if (_ShopScreen.IsActive)
			{
				_ShopScreen.DeActivate();
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
			_BottomPanel.BackButtonCallback = OpenArchiveCategory;
			_OpenedModule.DeActivate();
			_OpenedModule = GetModule<ArchivePanel>();
			_OpenedModule.Activate();
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Screen_enter);
		}

		private void OnArchiveSwitchOver()
		{
			ArchivePanel module = GetModule<ArchivePanel>();
			module.TryRunPromo2();
		}

		public void OpenQuestLog()
		{
			OpenQuestLog(null);
		}

		public void OpenQuestLog(string p_selectedQuest)
		{
			GetModule<JournalPanel>().SelectedQuest = p_selectedQuest;
			_ModuleSwitcher.Switch(SwitchQuestLog);
		}

		private void SwitchQuestLog()
		{
			CurrentScreen = ScreenType.Journal;
			_BottomPanel.SwitchToBackMode();
			if (_ShopScreen.IsActive)
			{
				_ShopScreen.DeActivate();
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

		public void OpenBoosterpack()
		{
			_ModuleSwitcher.Switch(SwitchBoosterpack);
		}

		private void SwitchBoosterpack()
		{
			CurrentScreen = ScreenType.Boosterpack;
			_BottomPanel.SwitchToBackMode();
			if (_ShopScreen.IsActive)
			{
				_ShopScreen.DeActivate();
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

		public void SwitchPage(System.Action p_updateAction)
		{
			_ModuleSwitcher.Switch(p_updateAction, 0.3f, 0.01f);
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
			if (_ShopScreen.IsActive)
			{
				_ShopScreen.DeActivate();
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
				if (DeviceInformation.IsAndroid || DeviceInformation.IsEmulator && (Nekki.Vector.GUI.Tutorial.Tutorial.Current == null || !Nekki.Vector.GUI.Tutorial.Tutorial.Current.Started) && _BottomPanel.IsInSecondaryScreen)
				{
					_BottomPanel.OnBackBtnClick();
				}
				break;
			case KeyCode.LeftBracket:
				if (!Core.Game.Settings.IsReleaseBuild)
				{

					if ((int)CounterController.Current.CounterFloor > 0)
					{
						--CounterController.Current.CounterFloor;
						UIModule.GetModule<BottomPanel>().SetFloorLabel();
					}
				}
				break;
			case KeyCode.RightBracket:
				if (!Core.Game.Settings.IsReleaseBuild)
				{
					++CounterController.Current.CounterFloor;
					UIModule.GetModule<BottomPanel>().SetFloorLabel();
				}
				break;
			case KeyCode.M:
				if (!Core.Game.Settings.IsReleaseBuild)
				{
					MissionsManager.CheckMissions();
					MissionsManager.GenerateMissions();
				}
				break;
			case KeyCode.L:
			{
				if (!Core.Game.Settings.IsReleaseBuild)
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
				}
				break;
			}
			}
		}
	}
}
