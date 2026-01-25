using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class MissionsManager
	{
		private const string _GeneratorFileName = "missions_generator.yaml";

		private const string _CheckerFileName = "missions_presets.yaml";

		private const string _PresetMissionCheck = "MissionsChecker";

		private static List<Preset> _GeneratorPresets = new List<Preset>();

		private static List<Preset> _MissionPresets = new List<Preset>();

		private static List<MissionItem> _MissionItems;

		private static List<MissionItem> _PrevMissionItems;

		public static bool IsMissionsEnabled
		{
			get
			{
				return !Demo.IsPlaying && (int)CounterController.Current.StartFloor != (int)CounterController.Current.CounterFloor && (int)CounterController.Current.CounterMissionsBlock == 0 && (int)CounterController.Current.CounterPlayCommand == 0;
			}
		}

		public static List<MissionItem> MissionItems
		{
			get
			{
				return _MissionItems;
			}
		}

		public static List<MissionItem> PrevMissionItems
		{
			get
			{
				return _PrevMissionItems;
			}
		}

		public static void Parse()
		{
			ParseBlock("missions_generator.yaml", _GeneratorPresets);
			ParseBlock("missions_presets.yaml", _MissionPresets);
			StarsManager.ParseRewardsPresets();
		}

		private static void ParseBlock(string p_filename, List<Preset> p_presets)
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, p_filename);
			p_presets.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				p_presets.Add(Preset.Create(item, p_presets));
			}
		}

		public static void Reset(List<Preset> p_presets)
		{
			p_presets.Clear();
		}

		public static void GenerateMissions()
		{
			CounterController.Current.CreateCounterOrSetValue("Rank", RankManager.Rank);
			StringBuffer.AddString("ProtocolName", StarterPacksManager.SelectedStarterPack.Name);
			_MissionItems = RunGeneratorPreset();
			_MissionItems.Reverse();
		}

		public static void CheckMissions()
		{
			RunCheckerPreset();
			_PrevMissionItems = _MissionItems;
		}

		public static void RestoreMissions()
		{
			List<MissionItem> list = new List<MissionItem>();
			foreach (UserItem item in DataLocal.Current.Stash)
			{
				MissionItem missionItem = MissionItem.Create(item);
				if (missionItem != null && missionItem.StarterpackName == StarterPacksManager.SelectedStarterPack.Name)
				{
					list.Add(missionItem);
				}
			}
			list.Sort((MissionItem m1, MissionItem m2) => (m1.Difficulty >= m2.Difficulty) ? 1 : (-1));
			_MissionItems = list;
			if (list.Count > 0)
			{
				RunCounterRestorer();
			}
		}

		private static List<MissionItem> RunGeneratorPreset()
		{
			PresetResult presetResult = new PresetResult();
			List<MissionItem> list = new List<MissionItem>();
			foreach (Preset generatorPreset in _GeneratorPresets)
			{
				int i = 0;
				for (int valueInt = generatorPreset.ItemsCount.ValueInt; i < valueInt; i++)
				{
					presetResult = generatorPreset.RunPreset();
					if (presetResult.Item != null)
					{
						list.Add(MissionItem.Create(presetResult.Item));
					}
				}
			}
			DataLocal.Current.Save(true);
			return list;
		}

		private static void RunCheckerPreset()
		{
			StringBuffer.AddString("ProtocolName", StarterPacksManager.SelectedStarterPack.Name);
			RunPresetByName("MissionsChecker");
			DataLocal.Current.Save(true);
		}

		public static PresetResult RunCounterRestorer()
		{
			StringBuffer.AddString("ProtocolName", StarterPacksManager.SelectedStarterPack.Name);
			return RunPresetByName("CounterRestore");
		}

		public static PresetResult RunMissionCardsRefresher()
		{
			StringBuffer.AddString("ProtocolName", StarterPacksManager.SelectedStarterPack.Name);
			return RunPresetByName("MissionCardsRefresher");
		}

		private static PresetResult RunPresetByName(string p_name)
		{
			PresetResult result = new PresetResult();
			foreach (Preset missionPreset in _MissionPresets)
			{
				if (missionPreset.Name == p_name)
				{
					int i = 0;
					for (int valueInt = missionPreset.ItemsCount.ValueInt; i < valueInt; i++)
					{
						result = missionPreset.RunPreset();
					}
					break;
				}
			}
			return result;
		}
	}
}
