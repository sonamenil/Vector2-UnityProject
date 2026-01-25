using System;
using UnityEngine.SocialPlatforms;

namespace Nekki.Vector.Core.GameCenter
{
	public abstract class GameCenterAbstract
	{
		public static Action<bool> OnAuthenticate;

		public static Action<IAchievement[]> OnLoadAchievements;

		public static Action<bool> OnResetAchievements;

		public static Action<string> OnAchievementUnlocked;

		public static Action<string, int> OnAchievementProgress;

		public abstract void Init();

		public abstract void Free();

		public abstract void SignIn();

		public abstract void SignOut();

		public abstract bool IsSignedIn();

		public abstract void LoadAchievements();

		public abstract void ShowAchievements();

		public abstract void ResetAchievements();

		public abstract void UnlockAchievement(string id);

		public abstract void AchievementProgress(string p_id, int p_progress);

		public abstract bool IsSupported();

		public abstract bool IsAutoSignInOnLaunch();

		protected void Log(string p_str)
		{
			DebugUtils.Log(p_str);
			DebugUtils.LogToConsole(p_str);
		}
	}
}
