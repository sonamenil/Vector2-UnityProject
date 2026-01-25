
using Nekki.Vector.Core.Game;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Nekki.Vector.Core.GameCenter
{
	public class GameCenter_Android : GameCenterAbstract
	{
		public override void Init()
		{
			//Log("[GameCenter_Android]: Init");
			//PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().Build();
			//PlayGamesPlatform.InitializeInstance(configuration);
			//PlayGamesPlatform.DebugLogEnabled = !Settings.IsReleaseBuild;
			//PlayGamesPlatform.Activate();
		}

		public override void Free()
		{
			Log("[GameCenter_Android]: Free");
		}

		private void CB_Authenticate(bool authed)
		{
			Log("[GameCenter_Android]: CB_Authenticate, authed = " + authed);
			if (authed)
			{
				LoadAchievements();
			}
			if (GameCenterAbstract.OnAuthenticate != null)
			{
				GameCenterAbstract.OnAuthenticate(authed);
			}
		}

		private void CB_LoadAchievements(IAchievement[] items)
		{
			Log("[GameCenter_Android]: CB_LoadAchievements");
			if (GameCenterAbstract.OnLoadAchievements != null)
			{
				GameCenterAbstract.OnLoadAchievements(items);
			}
		}

		private void CB_AuthenticateAndShowAchievementsUI(bool authed)
		{
			Log("[GameCenter_Android]: CB_AuthenticateAndShowAchievementsUI, authed = " + authed);
			if (authed)
			{
				UnityEngine.Social.ShowAchievementsUI();
			}
		}

		private void CB_AchievementUnlocked(string p_id)
		{
			Log("[GameCenter_Android]: CB_AchievementUnlocked, id = " + p_id);
			if (GameCenterAbstract.OnAchievementUnlocked != null)
			{
				GameCenterAbstract.OnAchievementUnlocked(p_id);
			}
		}

		private void CB_AchievementProgress(string p_id, int p_progress)
		{
			Log("[GameCenterAndroid]: OnAchievementProgess id = " + p_id + " progress = " + p_progress);
			if (GameCenterAbstract.OnAchievementProgress != null)
			{
				GameCenterAbstract.OnAchievementProgress(p_id, p_progress);
			}
		}

		public override bool IsSupported()
		{
			return true;
		}

		public override bool IsAutoSignInOnLaunch()
		{
			return false;
		}

		public override void SignIn()
		{
			if (DeviceInformation.IsAndroid)
			{
				if (!UnityEngine.Social.localUser.authenticated)
				{
					UnityEngine.Social.localUser.Authenticate(CB_Authenticate);
				}
			}
		}

		public override void SignOut()
		{
			if (UnityEngine.Social.localUser.authenticated)
			{
				//PlayGamesPlatform.Instance.SignOut();
				//CB_Authenticate(IsSignedIn());
			}
		}

		public override bool IsSignedIn()
		{
			//return PlayGamesPlatform.Instance.IsAuthenticated();
			return false;
		}

		public override void LoadAchievements()
		{
			UnityEngine.Social.LoadAchievements(CB_LoadAchievements);
		}

		public override void ShowAchievements()
		{
			bool authenticated = UnityEngine.Social.localUser.authenticated;
			if (authenticated)
			{
				UnityEngine.Social.localUser.Authenticate(CB_AuthenticateAndShowAchievementsUI);
			}
			else
			{
				CB_AuthenticateAndShowAchievementsUI(authenticated);
			}
		}

		public override void ResetAchievements()
		{
		}

		public override void UnlockAchievement(string id)
		{
			//Achievement achievement = PlayGamesPlatform.Instance.GetAchievement(id);
			//if (achievement == null)
			//{
			//	Log("[GameCenter_Android]: Error - Achievement null! Info: " + id);
			//	return;
			//}
			//if (achievement.IsUnlocked)
			//{
			//	Log("[GameCenter_Android]: Error - Achievement already unlocked! Info: " + achievement.ToString());
			//	return;
			//}
			//IAchievement achievement2 = PlayGamesPlatform.Instance.CreateAchievement();
			//achievement2.id = id;
			//achievement2.percentCompleted = 100.0;
			//achievement2.ReportProgress(delegate(bool success)
			//{
			//	if (success)
			//	{
			//		Log("[GameCenter_Android]:  Achievement successfully unlocked! Info: " + achievement.ToString());
			//		CB_AchievementUnlocked(id);
			//	}
			//	else
			//	{
			//		Log("[GameCenter_Android]:  Error - failed to unlock Achievement! Info: " + achievement.ToString());
			//	}
			//});
		}

		public override void AchievementProgress(string p_id, int p_progress)
		{
			//Achievement achievement = PlayGamesPlatform.Instance.GetAchievement(p_id);
			//if (achievement == null)
			//{
			//	Log("[GameCenter_Android]: Error - Achievement null! Info: " + p_id);
			//}
			//else if (achievement.IsUnlocked)
			//{
			//	Log("[GameCenter_Android]: Error - Achievement already unlocked! Info: " + achievement.ToString());
			//	return;
			//}
			//PlayGamesPlatform.Instance.SetStepsAtLeast(p_id, p_progress, delegate(bool success)
			//{
			//	if (success)
			//	{
			//		Log("[GameCenter_Android]:  Achievement successfully progress! Id: " + p_id + " Progress: " + p_progress);
			//		CB_AchievementProgress(p_id, p_progress);
			//	}
			//	else
			//	{
			//		Log("[GameCenter_Android]:  Error - failed to progress Achievement! Id: " + p_id + " Progress: " + p_progress);
			//	}
			//});
		}
	}
}
