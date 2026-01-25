using System;
using System.IO;
using System.Linq;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI;

namespace Nekki.Vector.Core.User
{
	public static class UserSwitcher
	{
		private const string _SkipTutorialUser = "user_tutorial_end.xml";

		private const string _Zone2User = "user_z2_{0}.xml";

		private const string _EscapeUser = "user_before_escape.xml";

		public static void SwitchToTutorialEndUser()
		{
			string p_userFile = string.Format("{0}/{1}", VectorPaths.SavedUsers, "user_tutorial_end.xml");
			SwitchToUser(p_userFile);
			DataLocal.Current.IsPaidVersion = ApplicationController.IsPaidBundleID;
			DataLocal.Current.Save(false);
		}

		public static void SwitchToZone2User(string p_id)
		{
			string p_userFile = string.Format("{0}/{1}", VectorPaths.SavedUsers, string.Format("user_z2_{0}.xml", p_id));
			SwitchToUser(p_userFile);
			DataLocal.Current.IsPaidVersion = ApplicationController.IsPaidBundleID;
			DataLocal.Current.Save(false);
		}

		public static void SwitchToEscapeUser()
		{
			string p_userFile = string.Format("{0}/{1}", VectorPaths.SavedUsers, "user_before_escape.xml");
			SwitchToUser(p_userFile);
			DataLocal.Current.IsPaidVersion = ApplicationController.IsPaidBundleID;
			DataLocal.Current.Save(false);
		}

		public static void SwitchToUser(string p_userFile, bool p_forceExternal = false)
		{
			if (CopyUser(p_userFile, p_forceExternal))
			{
				AchievementsManager.Reset();
				VectorPaths.ResetBundleCache();
				int gamedataVersion = DataLocal.Current.GamedataVersion;
				DataLocal.Reload();
				DataLocal.Current.GamedataVersion = gamedataVersion;
				DataLocal.UserDontSave = false;
				GameRestorer.RemoveBackup();
				AchievementsManager.Init();
				AchievementsManager.SyncAchievements();
				QuestManager.Reset();
				AudioManager.RestoreAudioSettings();
				Manager.Load(SceneKind.Main);
			}
		}

		private static bool CopyUser(string p_userFile, bool p_forceExternal = false)
		{
			if (VectorPaths.UsingResources && !p_forceExternal)
			{
				XmlUtils.CopyXmlFromResourcesAndUpdateHash(p_userFile, DataLocal.FilePath);
				return true;
			}
			if (!FileUtils.FileExists(p_userFile))
			{
				return false;
			}
			FileUtils.CopyFileAndGenerateHash(p_userFile, DataLocal.FilePath);
			return true;
		}

		public static string SaveCurentUser()
		{
			string text = string.Format("{0}/user_{1}.xml", VectorPaths.SavedUsersExternal, DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss"));
			if (!Directory.Exists(VectorPaths.SavedUsersExternal))
			{
				Directory.CreateDirectory(VectorPaths.SavedUsersExternal);
			}
			File.Copy(DataLocal.FilePath, text);
			return text;
		}

		public static string[] GetUserList()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(VectorPaths.SavedUsersExternal);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			FileInfo[] array = (from t in directoryInfo.GetFiles("user*.xml", SearchOption.AllDirectories)
				orderby t.CreationTime
				select t).ToArray();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Name;
			}
			return array2;
		}

		public static void SwitchByIndex(int p_index)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(VectorPaths.SavedUsersExternal);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			FileInfo[] array = (from t in directoryInfo.GetFiles("user*.xml", SearchOption.AllDirectories)
				orderby t.CreationTime
				select t).ToArray();
			if (p_index < array.Length)
			{
				SwitchToUser(array[p_index].GetFullName());
			}
		}
	}
}
