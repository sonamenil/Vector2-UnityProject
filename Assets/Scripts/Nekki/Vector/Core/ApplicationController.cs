using System;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Notifications;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public class ApplicationController : MonoBehaviour
	{
		public const string _BuildVersion = "1.1.1";

		private const string _BetaTestBundleId = "com.nekki.vector2demo";

		private const string _PaidBundleId = "com.nekki.vector2.paid";

		private static string _EditorSetedBundleId = "NOT_SET";

		private static string _EditorSetedBuildVersion = "NOT_SET";

		private static ApplicationController _Current;

		private static bool _IsPaused;

		private int _LastTouchCount;

		public static ApplicationController Current
		{
			get
			{
				return _Current;
			}
		}

		public static string BundleId
		{
			get
			{
				return Application.identifier;
			}
		}

		public static string BuildVersion
		{
			get
			{
				return "1.1.1";
			}
		}

		public static string BuildPlusGamedataVersion
		{
			get
			{
				return string.Format("{0}:{1}{2}", BuildVersion, DataLocal.Current.GamedataVersion, (!IsCBTBuild) ? string.Empty : "d");
			}
		}

		public static bool IsCBTBuild
		{
			get
			{
				return BundleId == "com.nekki.vector2demo";
			}
		}

		public static bool IsPaidBundleID
		{
			get
			{
				return BundleId == "com.nekki.vector2.paid";
			}
		}

		public static bool IsPaused
		{
			get
			{
				return _IsPaused;
			}
		}

		public static event Action<bool> OnAppPauseCallBack;

		public static event System.Action OnAppUpdateCallBack;

		public static void Init()
		{
			GameObject gameObject = new GameObject("[ApplicationController]");
			_Current = gameObject.AddComponent<ApplicationController>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}

		public static void Quit()
		{
			Debug.Log("Quit");
			Application.Quit();
		}

		public static void OpenURL(string p_url)
		{
			Debug.Log("OpenURL: " + p_url);
			Application.OpenURL(p_url);
		}

		private void OnApplicationFocus(bool p_focusStatus)
		{
			_IsPaused = !p_focusStatus;
			if (!DataLocal.InitTabu && LocalizationManager.IsInited && Preloader.IsInited)
			{
				if (!_IsPaused)
				{
					LocalNotificationManager.Current.CancelAllNotifications();
				}
				else
				{
					LocalNotificationManager.Current.SetAllNotification();
				}
			}
			if (p_focusStatus)
			{
				ChitProtector.ChangeCryptoKeyOnFocus();
			}
			if (ApplicationController.OnAppPauseCallBack != null)
			{
				ApplicationController.OnAppPauseCallBack(_IsPaused);
			}
		}

		public static void ResetAllProgress()
		{
			DataLocal.Reset();
			DataLocal.UserDontSave = false;
			GameRestorer.RemoveBackup();
			QuestManager.Reset();
			AudioManager.Init();
			StarterPacksManager.PrepareStarterPacks();
			Manager.Load(SceneKind.Main);
		}

		private void Update()
		{
			ProcessTouches();
			if (ApplicationController.OnAppUpdateCallBack != null)
			{
				ApplicationController.OnAppUpdateCallBack();
			}
		}

		private void ProcessTouches()
		{
			int num = Input.touches.Length;
			if (_LastTouchCount != num)
			{
				if (_LastTouchCount < num)
				{
					Debug.Log(num);
					TurnToDebug.NewTouchesCount(num);
				}
				_LastTouchCount = num;
			}
		}
	}
}
