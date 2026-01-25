using Nekki.Vector.Core;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.InputControllers;
using Nekki.Vector.GUI.MainScene;
using Nekki.Vector.GUI.Scenes.Run;
using Nekki.Vector.GUI.Scenes.Terminal;
using Nekki.Vector.GUI.ShopScene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nekki.Vector.GUI
{
	public static class Manager
	{
		private static SceneKind _MainScene = SceneKind.Loader;

		private static SceneKind _Scene = _MainScene;

		private static bool _IsInited = false;

		private static bool _NeedOpenCredits = false;

		private static bool _IsFakeFloor;

		private static bool _IsSettingsReset = false;

		private static bool _DialogActive = false;

		public static SceneKind Scene
		{
			get
			{
				return _Scene;
			}
			set
			{
				_Scene = value;
			}
		}

		public static bool IsInited
		{
			get
			{
				return _IsInited;
			}
		}

		public static bool NeedOpenCredits
		{
			get
			{
				return _NeedOpenCredits;
			}
			set
			{
				_NeedOpenCredits = value;
			}
		}

		public static bool IsFakeFloor
		{
			get
			{
				return _IsFakeFloor;
			}
			set
			{
				_IsFakeFloor = value;
			}
		}

		public static bool IsFloor
		{
			get
			{
				return RunMainController.IsRunNow;
			}
		}

		public static bool IsLoader
		{
			get
			{
				return _Scene == SceneKind.Loader;
			}
		}

		public static bool IsEquip
		{
			get
			{
				return _Scene == SceneKind.Main;
			}
		}

		public static bool IsRun
		{
			get
			{
				return _Scene == SceneKind.Run;
			}
		}

		public static bool IsShop
		{
			get
			{
				return _Scene == SceneKind.Shop;
			}
		}

		public static bool IsTerminal
		{
			get
			{
				return _Scene == SceneKind.Terminal;
			}
		}

		public static ScreenType CurrentScreen
		{
			get
			{
				if (IsEquip)
				{
					return Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.CurrentScreen;
				}
				if (IsShop)
				{
					return Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.CurrentScreen;
				}
				if (IsTerminal)
				{
					return ScreenType.Main;
				}
				return ScreenType.Unknown;
			}
		}

		public static ScreenType PrevScreen
		{
			get
			{
				if (IsEquip)
				{
					return Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.PrevScreen;
				}
				if (IsShop)
				{
					return Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.PrevScreen;
				}
				if (IsTerminal)
				{
					return ScreenType.Main;
				}
				return ScreenType.Unknown;
			}
		}

		public static Canvas SceneCanvas
		{
			get
			{
				if (IsEquip)
				{
					return Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.Canvas;
				}
				if (IsShop)
				{
					return Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.Canvas;
				}
				if (IsTerminal)
				{
					return Scene<TerminalScene>.Current.Canvas;
				}
				if (IsRun)
				{
					return Scene<RunScene>.Current.Canvas;
				}
				return null;
			}
		}

		public static KeyboardController SceneKeyboardController
		{
			get
			{
				if (IsEquip)
				{
					return (!(Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current != null)) ? null : Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.KeyboardController;
				}
				if (IsShop)
				{
					return (!(Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current != null)) ? null : Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.KeyboardController;
				}
				if (IsTerminal)
				{
					return (!(Scene<TerminalScene>.Current != null)) ? null : Scene<TerminalScene>.Current.KeyboardController;
				}
				if (IsRun)
				{
					return (!(Scene<RunScene>.Current != null)) ? null : Scene<RunScene>.Current.KeyboardController;
				}
				return null;
			}
		}

		public static bool SceneKeyboardControllerEnabled
		{
			get
			{
				KeyboardController sceneKeyboardController = SceneKeyboardController;
				return sceneKeyboardController != null && sceneKeyboardController.enabled;
			}
			set
			{
				KeyboardController sceneKeyboardController = SceneKeyboardController;
				if (sceneKeyboardController != null)
				{
					sceneKeyboardController.enabled = value;
				}
			}
		}

		public static bool IsSettingsReset
		{
			get
			{
				return _IsSettingsReset;
			}
			set
			{
				_IsSettingsReset = value;
			}
		}

		public static bool DialogActive
		{
			get
			{
				return _DialogActive;
			}
			set
			{
				_DialogActive = value;
			}
		}

		public static string ZoneLoaderBackground
		{
			get
			{
				switch (ZoneManager.CurrentZone)
				{
				case Zone.Zone1:
					return "bg_loader";
				case Zone.Zone2:
					return "bg_loader2";
				default:
					return "bg_loader";
				}
			}
		}

		public static string ZoneBackground
		{
			get
			{
				switch (ZoneManager.CurrentZone)
				{
				case Zone.Zone1:
					return "bg_main";
				case Zone.Zone2:
					return "bg_z2_main";
				default:
					return "bg_main";
				}
			}
		}

		public static string ZoneVisualName
		{
			get
			{
				return string.Format("^Chapters.Chapter{0}.Description^", (int)ZoneManager.CurrentZone);
			}
		}

		public static bool Init(SceneKind Scene)
		{
			if (!_IsInited)
			{
				_IsInited = true;
				if (Scene != 0)
				{
					Reset();
					return false;
				}
			}
			return true;
		}

		public static void Reset()
		{
			SceneManager.LoadSceneAsync(0);
		}

		public static void Load(SceneKind p_scene, bool p_stopMusic = true)
		{
			LoaderScene.PrevScene = _Scene;
			LoaderScene.NextScene = p_scene;
			LoaderScene.StopMusic = p_stopMusic;
			_Scene = p_scene;
			DebugUtils.SceneLoadStartTimer();
			SceneManager.LoadSceneAsync(1);
		}

		public static void Play()
		{
			if (!DataLocal.Current.IsPaidVersion)
			{
				EnergyManager.SpendEnergyLevel(1);
			}
			RunMainController.RunStart();
			Load(SceneKind.Run);
		}

		public static void PlayNextFloor()
		{
			DataLocal.Current.Save(true);
			DataLocal.UserDontSave = true;
			GeneratorHelper.PrepareNextFloor(MainRandom.Current.getRandom());
			Load(SceneKind.Run);
		}

		public static void Quit()
		{
			if ((int)CounterController.Current.CounterTutorialBasic == 1)
			{
				CounterController.Current.CounterTutorialBasicRemove();
			}
			DataLocal.Current.CounterController.CounterFloor = 0;
			RunMainController.RunEnd();
			GameRestorer.RemoveBackup();
			_IsFakeFloor = false;
			Load(SceneKind.Main);
		}

		public static void ShowCurrentScene()
		{
			SetCurrentSceneActive(true);
		}

		public static void HideCurrentScene()
		{
			SetCurrentSceneActive(false);
		}

		private static void SetCurrentSceneActive(bool p_active)
		{
			Canvas sceneCanvas = SceneCanvas;
			if (sceneCanvas != null)
			{
				sceneCanvas.enabled = p_active;
			}
		}

		public static void ShowDebugOptionsIfNeed()
		{
			if (_IsSettingsReset)
			{
				_IsSettingsReset = false;
				if (RunMainController.IsRunNow)
				{
					HudPanel module = UIModule.GetModule<HudPanel>();
					module.SetPauseState(true, true);
				}
			}
		}

		public static void OpenArchive()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenArchive();
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenArchive();
			}
		}

		public static void OpenArchive(SlotItem.Slot p_slot)
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenArchive(p_slot);
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenArchive(p_slot);
			}
		}

		public static void OpenArchiveCategory()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenArchiveCategory();
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenArchiveCategory();
			}
		}

		public static void OpenBoosterpack()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenBoosterpack();
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenBoosterpack();
			}
		}

		public static void OpenDetails()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenDetails();
			}
		}

		public static void OpenQuestLog()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenQuestLog();
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenQuestLog();
			}
		}

		public static void OpenQuestLog(string p_selectedQuest)
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenQuestLog(p_selectedQuest);
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenQuestLog(p_selectedQuest);
			}
		}

		public static void OpenMainScreen()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenMainScreen();
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenShopScreen();
			}
		}

		public static void OpenCredits()
		{
			if (IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.OpenCredits();
			}
			else if (IsShop)
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.OpenCredits();
			}
		}
	}
}
