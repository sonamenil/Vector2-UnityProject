using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using UnityEngine.SocialPlatforms;

namespace Nekki.Vector.Core.GameCenter
{
	public static class AchievementsManager
	{
		private static List<AchievementData> _AchievementsList = new List<AchievementData>();

		private static Dictionary<string, List<AchievementData>> _AchievementByCounterName = new Dictionary<string, List<AchievementData>>();

		private static Dictionary<string, AchievementData> _AchievementById = new Dictionary<string, AchievementData>();

		private static List<string> _AchievementsNamespaces = new List<string>();

		public static string Filename
		{
			get
			{
				return VectorPaths.Achievements + "/android_data.xml";
			}
		}

		public static void Init()
		{
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
			GameCenterAbstract.OnLoadAchievements = (Action<IAchievement[]>)Delegate.Combine(GameCenterAbstract.OnLoadAchievements, new Action<IAchievement[]>(OnLoadAhievements));
			GameCenterAbstract.OnResetAchievements = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnResetAchievements, new Action<bool>(OnResetAchievements));
			LoadData(Filename);
			RegisterCounterHandlers();
		}

		public static void Reset()
		{
			UnregisterCounterHandlers();
			_AchievementsList.Clear();
			_AchievementByCounterName.Clear();
			_AchievementById.Clear();
			_AchievementsNamespaces.Clear();
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
			GameCenterAbstract.OnLoadAchievements = (Action<IAchievement[]>)Delegate.Remove(GameCenterAbstract.OnLoadAchievements, new Action<IAchievement[]>(OnLoadAhievements));
			GameCenterAbstract.OnResetAchievements = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnResetAchievements, new Action<bool>(OnResetAchievements));
		}

		public static void Reload()
		{
			Reset();
			Init();
		}

		public static double ConvertAchievementProgressToPercentCompleted(string p_achievementId, int p_progress)
		{
			AchievementData value = null;
			if (!_AchievementById.TryGetValue(p_achievementId, out value))
			{
				return -1.0;
			}
			return (double)p_progress / (double)value.Goal * 100.0;
		}

		private static void RegisterCounterHandlers()
		{
			foreach (string achievementsNamespace in _AchievementsNamespaces)
			{
				CounterController.Current.GetCounterNamespace(achievementsNamespace).OnCounterChanged += OnCounterChanged;
			}
		}

		private static void UnregisterCounterHandlers()
		{
			foreach (string achievementsNamespace in _AchievementsNamespaces)
			{
				CounterController.Current.GetCounterNamespace(achievementsNamespace).OnCounterChanged -= OnCounterChanged;
			}
		}

		public static void SyncAchievements()
		{
			GameCenterController.LoadAchievements();
		}

		private static void LoadData(string p_path)
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(p_path, string.Empty);
			if (xmlDocument == null)
			{
				DebugUtils.LogError("[AchievementsManager]: achievements file don't exists - " + p_path);
				return;
			}
			foreach (XmlNode childNode in xmlDocument["Achievements"].ChildNodes)
			{
				AddAchievement(childNode);
			}
		}

		private static void AddAchievement(XmlNode p_node)
		{
			AchievementData achievementData = new AchievementData(p_node);
			List<AchievementData> value = null;
			_AchievementsList.Add(achievementData);
			if (!_AchievementByCounterName.TryGetValue(achievementData.CounterName, out value))
			{
				value = new List<AchievementData>();
				_AchievementByCounterName.Add(achievementData.CounterName, value);
			}
			value.Add(achievementData);
			_AchievementById.Add(achievementData.Id, achievementData);
			if (!_AchievementsNamespaces.Contains(achievementData.CounterNamespace))
			{
				_AchievementsNamespaces.Add(achievementData.CounterNamespace);
			}
		}

		private static void OnCounterChanged(string p_counterName, string p_counterNamespace)
		{
			if (!_AchievementByCounterName.ContainsKey(p_counterName))
			{
				DebugUtils.Log("[AchievementsManager]: try to update unexisting achievement by counter - " + p_counterName);
				return;
			}
			List<AchievementData> list = _AchievementByCounterName[p_counterName];
			bool flag = false;
			foreach (AchievementData item in list)
			{
				if (!item.IsCompleted)
				{
					flag = flag || item.UpdateIncrementalAchievementStep();
					item.UpdateProgress(true);
				}
			}
			if (flag)
			{
				CounterController.Current.RemoveUserCounter(p_counterName, p_counterNamespace);
			}
		}

		private static void OnAuthenticate(bool p_authed)
		{
			if (!p_authed)
			{
				return;
			}
			foreach (AchievementData achievements in _AchievementsList)
			{
				achievements.SendProgress();
			}
		}

		private static void OnLoadAhievements(IAchievement[] p_achievementsData)
		{
			foreach (IAchievement achievementData in p_achievementsData)
			{
				SyncAchievementProgress(achievementData);
			}
		}

		private static void OnResetAchievements(bool p_success)
		{
			if (!p_success)
			{
				return;
			}
			CounterController.Current.GetCounterNamespace("ST_Achievements").Clear();
			foreach (string achievementsNamespace in _AchievementsNamespaces)
			{
				CounterController.Current.GetCounterNamespace(achievementsNamespace).Clear();
			}
			DataLocal.Current.Save(true);
			Reload();
		}

		private static void SyncAchievementProgress(IAchievement achievementData)
		{
			if (_AchievementById.ContainsKey(achievementData.id))
			{
				AchievementData achievementData2 = _AchievementById[achievementData.id];
				achievementData2.SyncProgress(achievementData);
			}
		}

		public static void Log(StringBuilder p_sb)
		{
			foreach (AchievementData achievements in _AchievementsList)
			{
				achievements.Log(p_sb);
			}
		}
	}
}
