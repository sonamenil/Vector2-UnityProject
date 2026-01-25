using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.Core.Controllers
{
	public static class ControllerTutorial
	{
		private static bool _IsShow;

		private static Key _Key;

		public static bool IsShow
		{
			get
			{
				return _IsShow;
			}
		}

		public static void ShowTutorial(string p_key, string p_textMobile, string p_textKeyboard, bool p_lock)
		{
			if (!_IsShow)
			{
				_IsShow = true;
				_Key = KeyVariables.Parse(p_key);
				LockGame(p_lock);
				TutorialUI orMountModule = Scene<RunScene>.Current.GetOrMountModule<TutorialUI>();
				orMountModule.SetTutorialStep(_Key, p_textMobile, p_textKeyboard);
				orMountModule.Activate();
			}
		}

		public static void LockGame(bool p_lock)
		{
			if (_IsShow)
			{
				RunMainController.Player.ControllerControl.Enable = false;
				if (p_lock)
				{
					RunMainController.CanPause = true;
					RunMainController.IsPause(true, true);
				}
				else
				{
					RunMainController.Scene.SlowMode = true;
				}
				RunMainController.CanPause = false;
			}
		}

		private static void DismissTutorial()
		{
			_IsShow = false;
			if (RunMainController.Scene != null)
			{
				RunMainController.Scene.SlowMode = false;
				RunMainController.CanPause = true;
				RunMainController.IsPause(false, true);
				TutorialUI module = UIModule.GetModule<TutorialUI>();
				module.DeActivate();
			}
		}

		public static bool Check(KeyVariables p_key)
		{
			if (_IsShow && p_key.Key == _Key)
			{
				DismissTutorial();
				return true;
			}
			return false;
		}

		public static void Reset()
		{
			if (_IsShow)
			{
				DismissTutorial();
			}
		}
	}
}
