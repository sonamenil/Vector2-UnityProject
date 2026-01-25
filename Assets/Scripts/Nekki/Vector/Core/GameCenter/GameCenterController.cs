namespace Nekki.Vector.Core.GameCenter
{
	public static class GameCenterController
	{
		private static GameCenterAbstract _Current;

		public static GameCenterAbstract Current
		{
			get
			{
				if (_Current == null)
				{
					Init();
				}
				return _Current;
			}
		}

		public static void Init()
		{
			_Current = new GameCenter_Android();
			_Current.Init();
		}

		public static void Free()
		{
			Current.Free();
		}

		public static bool IsSupported()
		{
			return _Current.IsSupported();
		}

		public static bool IsAutoSignInOnLaunch()
		{
			return _Current.IsAutoSignInOnLaunch();
		}

		public static void SignIn()
		{
			if (IsSupported())
			{
				Current.SignIn();
			}
		}

		public static void SignOut()
		{
			if (IsSupported())
			{
				Current.SignOut();
			}
		}

		public static bool IsSignedIn()
		{
			if (IsSupported())
			{
				return Current.IsSignedIn();
			}
			return false;
		}

		public static void UnlockAchievement(string id)
		{
			if (IsSignedIn())
			{
				Current.UnlockAchievement(id);
			}
		}

		public static void AchievementProgress(string id, int p_progress)
		{
			if (IsSignedIn())
			{
				Current.AchievementProgress(id, p_progress);
			}
		}

		public static void ShowAchiements()
		{
			if (IsSignedIn())
			{
				Current.ShowAchievements();
			}
		}

		public static void LoadAchievements()
		{
			if (IsSignedIn())
			{
				Current.LoadAchievements();
			}
		}

		public static void ResetAchievements()
		{
			if (IsSignedIn() || DeviceInformation.IsEmulator)
			{
				Current.ResetAchievements();
			}
		}
	}
}
