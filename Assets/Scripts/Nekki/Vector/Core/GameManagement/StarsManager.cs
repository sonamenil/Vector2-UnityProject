using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class StarsManager
	{
		private const string _StarRewardsFileName = "star_rewards.yaml";

		private static List<StarReward> _StarsRewards = new List<StarReward>();

		private static bool _LastIsComplete = true;

		private static bool _IsShowSequence;

		private static Dictionary<string, Preset> _RewardsPresets = new Dictionary<string, Preset>();

		private static int _CurrentStars = 0;

		public static List<StarReward> StarRewards
		{
			get
			{
				return _StarsRewards;
			}
		}

		public static bool LastIsComplete
		{
			get
			{
				return _LastIsComplete;
			}
		}

		public static bool IsShowSequence
		{
			get
			{
				return _IsShowSequence;
			}
		}

		public static int CurrentStars
		{
			get
			{
				return _CurrentStars;
			}
		}

		public static void Init(bool p_isGameRestoreActive)
		{
			_CurrentStars = CounterController.Current.CounterCurrentMissionStars;
			CheckMissions();
			if (!p_isGameRestoreActive)
			{
				ClearBuffs();
			}
			_IsShowSequence = !p_isGameRestoreActive && _CurrentStars != 0;
		}

		public static void CheckMissions()
		{
			Mapping balanceMapping = ZoneResource<ZoneBalanceManager>.Current.GetBalanceMapping("StarRewards");
			int coolness = StarterPacksManager.GetBestAvailableStarterPack().Coolness;
			foreach (Mapping item in balanceMapping)
			{
				if (!CheckMission(item, coolness))
				{
					_LastIsComplete = false;
					break;
				}
			}
		}

		private static bool CheckMission(Mapping p_node, int p_starterPackCoolness)
		{
			string key = p_node.key;
			StarReward.Type p_type = ((p_node.GetNodesSize() <= 1 || IsRewardAquired(key) || p_starterPackCoolness < StarReward.GetCoolnessMin(key)) ? StarReward.Type.Repeated : StarReward.Type.Single);
			StarReward starReward = new StarReward(key, p_type);
			_StarsRewards.Add(starReward);
			return IsEnoughMoney(starReward);
		}

		public static void GetAllRewards()
		{
			int i = 0;
			for (int num = ((!_LastIsComplete) ? (_StarsRewards.Count - 1) : _StarsRewards.Count); i < num; i++)
			{
				GetReward(_StarsRewards[i].Preset);
			}
			DataLocal.Current.Save(false);
		}

		private static void ClearBuffs()
		{
			CounterController.Current.ClearCounterNamespace("StarBuffs");
		}

		private static bool IsRewardAquired(string rewardName)
		{
			string balance = ZoneResource<ZoneBalanceManager>.Current.GetBalance("StarRewards", rewardName, StarReward.Type.Single.ToString(), "Preset");
			return (int)CounterController.Current.GetUserCounter(balance, "StarRewards") == 1;
		}

		private static bool IsEnoughMoney(StarReward starReward)
		{
			if (_CurrentStars > starReward.Cost)
			{
				_CurrentStars -= starReward.Cost;
				return true;
			}
			return false;
		}

		public static void ClearMissionStars()
		{
			_StarsRewards.Clear();
		}

		public static void ParseRewardsPresets()
		{
			_RewardsPresets.Clear();
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "star_rewards.yaml");
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				Preset preset = Preset.Create(item);
				if (preset == null)
				{
					DebugUtils.Dialog(string.Format("Can't create start items preset from - {0}", item.key), false);
				}
				else
				{
					_RewardsPresets.Add(preset.Name, preset);
				}
			}
		}

		public static void GetReward(string p_presetName)
		{
			if (_RewardsPresets.ContainsKey(p_presetName))
			{
				Preset preset = _RewardsPresets[p_presetName];
				int i = 0;
				for (int valueInt = preset.ItemsCount.ValueInt; i < valueInt; i++)
				{
					preset.RunPreset();
				}
			}
		}
	}
}
