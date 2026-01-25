using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterByFloorManager
	{
		private const string _PreProcessFile = "pre_process.yaml";

		private const string _TerminalFile = "terminal_counters.yaml";

		private const string _BoosterPackFile = "boosterpack_counters.yaml";

		private const string _EndFloorFile = "end_floor_counters.yaml";

		private const string _SupplyFile = "supply_counters.yaml";

		private List<Preset> _PreProcessPresets = new List<Preset>();

		private List<Preset> _TerminalPresets = new List<Preset>();

		private List<Preset> _BoosterPackPresets = new List<Preset>();

		private List<Preset> _EndFloorPresets = new List<Preset>();

		private List<Preset> _SupplyPresets = new List<Preset>();

		private static CounterByFloorManager _Current;

		public static CounterByFloorManager Current
		{
			get
			{
				Init();
				return _Current;
			}
		}

		private CounterByFloorManager()
		{
			ParseZoneSpecificPresets(true);
			ParseCommonPresets();
		}

		public static void Init()
		{
			if (_Current == null)
			{
				_Current = new CounterByFloorManager();
			}
		}

		public static void Reset()
		{
			_Current = null;
		}

		public static void ReloadIfNeed()
		{
			if (_Current != null)
			{
				_Current.ParseZoneSpecificPresets(false);
			}
		}

		private void ParseZoneSpecificPresets(bool p_forceReload)
		{
			ParseIfNeed(_PreProcessPresets, "PreProcess", p_forceReload);
		}

		private void ParseCommonPresets()
		{
			Parse(_TerminalPresets, VectorPaths.GeneratorDataDefault + "/terminal_counters.yaml");
			Parse(_BoosterPackPresets, VectorPaths.GeneratorDataDefault + "/boosterpack_counters.yaml");
			Parse(_EndFloorPresets, VectorPaths.GeneratorDataDefault + "/end_floor_counters.yaml");
			Parse(_SupplyPresets, VectorPaths.GeneratorDataDefault + "/supply_counters.yaml");
		}

		public void SetCountersPreProcess()
		{
			SetCounters(_PreProcessPresets);
		}

		public void SetCountersTerminal()
		{
			SetCounters(_TerminalPresets);
		}

		public void SetCountersBoosterPack()
		{
			SetCounters(_BoosterPackPresets);
		}

		public void SetCountersEndFloor()
		{
			SetCounters(_EndFloorPresets);
		}

		public void SetCountersSupply()
		{
			SetCounters(_SupplyPresets);
		}

		private static void SetCounters(List<Preset> p_presets)
		{
			for (int i = 0; i < p_presets.Count; i++)
			{
				for (int j = 0; j < p_presets[i].ItemsCount.ValueInt; j++)
				{
					p_presets[i].RunPreset();
				}
			}
			DataLocal.Current.Save(false);
		}

		private static void ParseIfNeed(List<Preset> p_presets, string p_zoneResourceId, bool p_forceReload)
		{
			if (p_forceReload || ZoneManager.IsResourceNeedReload(p_zoneResourceId))
			{
				Parse(p_presets, VectorPaths.GeneratorData + "/" + ZoneManager.GetResourceFilePath(p_zoneResourceId));
			}
		}

		private static void Parse(List<Preset> p_presets, string p_path)
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(p_path, string.Empty);
			if (yamlDocumentNekki == null)
			{
				return;
			}
			p_presets.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				p_presets.Add(Preset.Create(item));
			}
		}
	}
}
