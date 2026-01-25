using System;
using System.IO;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public static class VectorPaths
	{
		private const string _GameData = "/gamedata";

		private const string _UserData = "/userdata";

		private const string _AnimationsBin = "/animations";

		private const string _Demo = "/demo";

		private const string _GeneratorData = "/generator_data";

		private const string _GeneratorDataDefault = "/default";

		private const string _GeneratorTemp = "/generator_temp";

		private const string _Logs = "/logs";

		private const string _RunData = "/run_data";

		private const string _Settings = "/settings";

		private const string _Sounds = "/sounds";

		private const string _Textures = "/textures";

		private const string _Statistics = "/statistics";

		private const string _InApps = "/in_apps";

		private const string _Achievements = "/achievements";

		private const string _Credits = "/text/credits";

		private const string _Localization = "/text/localization";

		private const string _Config = "/settings";

		private const string _CorruptedFiles = "/corrupted_files";

		private const string _SavedUser = "/saved_users";

		private const string _UserUpdate = "/user_update";

		private const string _News = "/cached_news";

		private const string _ABTest = "/cached_abtest";

		private const string _Offers = "/cached_offers";

		private const string _Bundles = "/cached_bundles";

		private const string _BundlesBackup = "/bundles_backup";

		private const string _Temp = "/tmp";

		public static string CurrentResources = string.Empty;

		public static string CurrentStorage = string.Empty;

		public static string CurrentCachedStorage = string.Empty;

		public static bool UsingResources = true;

		private static bool _IsInitialized;

		public static string GameData
		{
			get
			{
				return CurrentResources + "/gamedata";
			}
		}

		public static string ExternalCachedGameData
		{
			get
			{
				return CurrentCachedStorage + "/gamedata";
			}
		}

		public static string AnimationBinary
		{
			get
			{
				return GameData + "/animations";
			}
		}

		public static string GeneratorData
		{
			get
			{
				return GameData + "/generator_data";
			}
		}

		public static string GeneratorDataDefault
		{
			get
			{
				return GeneratorData + "/default";
			}
		}

		public static string GeneratorTemp
		{
			get
			{
				return GameData + "/generator_temp";
			}
		}

		public static string RunData
		{
			get
			{
				return GameData + "/run_data";
			}
		}

		public static string Rooms
		{
			get
			{
				return RunData + "/rooms";
			}
		}

		public static string Models
		{
			get
			{
				return RunData + "/models";
			}
		}

		public static string RunDataLibs
		{
			get
			{
				return RunData + "/libraries";
			}
		}

		public static string RunDataTemplates
		{
			get
			{
				return RunData + "/templates";
			}
		}

		public static string Settings
		{
			get
			{
				return GameData + "/settings";
			}
		}

		public static string Sounds
		{
			get
			{
				return GameData + "/sounds";
			}
		}

		public static string Textures
		{
			get
			{
				return GameData + "/textures";
			}
		}

		public static string TexturesInGame
		{
			get
			{
				return GameData + "/textures/in_game";
			}
		}

		public static string InApps
		{
			get
			{
				return GameData + "/in_apps";
			}
		}

		public static string Achievements
		{
			get
			{
				return GameData + "/achievements";
			}
		}

		public static string Credits
		{
			get
			{
				return GameData + "/text/credits";
			}
		}

		public static string Localization
		{
			get
			{
				return GameData + "/text/localization";
			}
		}

		public static string SaveUserFolderName
		{
			get
			{
				return "/saved_users";
			}
		}

		public static string SavedUsers
		{
			get
			{
				return GameData + "/saved_users";
			}
		}

		public static string UserUpdate
		{
			get
			{
				return GameData + "/user_update";
			}
		}

		public static string StorageUserData
		{
			get
			{
				return CurrentStorage + "/userdata";
			}
		}

		public static string SettingsExternal
		{
			get
			{
				return StorageUserData + "/settings";
			}
		}

		public static string GeneratorDataExternal
		{
			get
			{
				return StorageUserData + "/generator_data";
			}
		}

		public static string SavedUsersExternal
		{
			get
			{
				return StorageUserData + "/saved_users";
			}
		}

		public static string Statistics
		{
			get
			{
				return StorageUserData + "/statistics";
			}
		}

		public static string CorruptedFiles
		{
			get
			{
				return StorageUserData + "/corrupted_files";
			}
		}

		public static string ConfigExternal
		{
			get
			{
				return StorageUserData + "/settings";
			}
		}

		public static string NewsExternal
		{
			get
			{
				return ExternalCachedGameData + "/cached_news";
			}
		}

		public static string BundlesExternal
		{
			get
			{
				return ExternalCachedGameData + "/cached_bundles";
			}
		}

		public static string BundlesBackup
		{
			get
			{
				return CurrentCachedStorage + "/bundles_backup";
			}
		}

		public static string Demo
		{
			get
			{
				return StorageUserData + "/demo";
			}
		}

		public static string Logs
		{
			get
			{
				return StorageUserData + "/logs";
			}
		}

		public static string TempExternal
		{
			get
			{
				return CurrentCachedStorage + "/tmp";
			}
		}

		public static string ABTestExternal
		{
			get
			{
				return ExternalCachedGameData + "/cached_abtest";
			}
		}

		public static string OffersExternal
		{
			get
			{
				return ExternalCachedGameData + "/cached_offers";
			}
		}

		public static string CustomAnimations
		{
			get
			{
				return GameData + "/textures/in_game/sequence";
			}
		}

		public static bool IsInitialized
		{
			get
			{
				return _IsInitialized;
			}
		}

		public static void Init()
		{
			if (_IsInitialized)
			{
				return;
			}
			_IsInitialized = true;
			CurrentResources = string.Empty;
			CurrentStorage = Application.persistentDataPath;
			CurrentCachedStorage = Application.persistentDataPath;
			UsingResources = true;
			string androidNativePath = GetAndroidNativePath();
			DebugUtils.Log("Path1:" + CurrentStorage);
			DebugUtils.Log("Path2:" + androidNativePath);
			if (string.IsNullOrEmpty(CurrentStorage))
			{
				CurrentStorage = (CurrentCachedStorage = androidNativePath);
			}
			else
			{
				try
				{
					if (!Directory.Exists(CurrentStorage + "/userdata") && Directory.Exists(androidNativePath + "/userdata"))
					{
						CurrentStorage = (CurrentCachedStorage = androidNativePath);
					}
				}
				catch
				{
				}
			}
			UserDataCreator.CopyFromGamedataToUserdata();
			CreateExternalFolders();
		}

		public static void CreateExternalFolders()
		{
			if (!Directory.Exists(StorageUserData))
			{
				Directory.CreateDirectory(StorageUserData);
			}
			if (!Directory.Exists(ExternalCachedGameData))
			{
				Directory.CreateDirectory(ExternalCachedGameData);
			}
			if (!Directory.Exists(SettingsExternal))
			{
				Directory.CreateDirectory(SettingsExternal);
			}
			if (!Directory.Exists(Statistics))
			{
				Directory.CreateDirectory(Statistics);
			}
			if (!Directory.Exists(OffersExternal))
			{
				Directory.CreateDirectory(OffersExternal);
			}
			if (!Directory.Exists(NewsExternal))
			{
				Directory.CreateDirectory(NewsExternal);
			}
			if (!Directory.Exists(BundlesExternal))
			{
				Directory.CreateDirectory(BundlesExternal);
			}
		}

		public static void ClearTempFolder()
		{
			if (Directory.Exists(TempExternal))
			{
				Directory.Delete(TempExternal, true);
			}
			Directory.CreateDirectory(TempExternal);
		}

		public static string CombineExternalCachedPath(string p_path)
		{
			return string.Format("{0}/{1}", CurrentCachedStorage, p_path);
		}

		public static void ResetExternalGamedata()
		{
			if (Directory.Exists(ExternalCachedGameData))
			{
				Directory.Delete(ExternalCachedGameData, true);
				CreateExternalFolders();
				TimersManager.RemoveOffersTimers();
			}
		}

		public static void BackupBundles()
		{
			if (Directory.Exists(BundlesExternal))
			{
				if (Directory.Exists(BundlesBackup))
				{
					Directory.Delete(BundlesBackup, true);
				}
				Directory.Move(BundlesExternal, BundlesBackup);
			}
		}

		public static void RestoreBundles()
		{
			if (Directory.Exists(BundlesBackup))
			{
				if (Directory.Exists(BundlesExternal))
				{
					Directory.Delete(BundlesExternal, true);
				}
				Directory.Move(BundlesBackup, BundlesExternal);
				if (Directory.Exists(BundlesBackup))
				{
					Directory.Delete(BundlesBackup, true);
				}
			}
		}

		public static void TEMP_ResetExternalGamedataAndroid()
		{
			string path = Application.temporaryCachePath + "/gamedata";
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		public static void ResetBundleCache()
		{
			FileUtils.RecreateDirectory(BundlesExternal);
		}

		public static string GetAndroidNativePath()
		{
			string text = string.Empty;
			try
			{
				IntPtr javaClass = AndroidJNI.FindClass("android/content/ContextWrapper");
				IntPtr methodID = AndroidJNIHelper.GetMethodID(javaClass, "getFilesDir", "()Ljava/io/File;");
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						IntPtr obj = AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, new jvalue[0]);
						IntPtr javaClass2 = AndroidJNI.FindClass("java/io/File");
						IntPtr methodID2 = AndroidJNIHelper.GetMethodID(javaClass2, "getAbsolutePath", "()Ljava/lang/String;");
						text = AndroidJNI.CallStringMethod(obj, methodID2, new jvalue[0]);
						if (text == null)
						{
							Debug.Log("Using fallback path");
							text = "/data/data/com.nekki.vector2/files";
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
			return text;
		}
	}
}
