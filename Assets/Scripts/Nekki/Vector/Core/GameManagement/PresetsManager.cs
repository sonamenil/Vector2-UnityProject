using System.Collections.Generic;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class PresetsManager
	{
		private const string _FileName = "presets.yaml";

		private static List<Preset> _AllPresets = new List<Preset>();

		private static bool _IsInited = false;

		public static void Init()
		{
			if (_IsInited)
			{
				return;
			}
			_IsInited = true;
			DebugUtils.Log("Parse Presets");
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "presets.yaml");
			_AllPresets.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_AllPresets.Add(Preset.Create(item, _AllPresets));
			}
		}

		public static void Reload()
		{
			_IsInited = false;
			Init();
		}

		public static Preset GetPresetByName(string p_name, bool toLower = false)
		{
			foreach (Preset allPreset in _AllPresets)
			{
				if (!toLower)
				{
					if (allPreset.Name == p_name)
					{
						return allPreset;
					}
				}
				else if (allPreset.Name.ToLower() == p_name)
				{
					return allPreset;
				}
			}
			DebugUtils.Dialog("Error: Preset " + p_name + " not found!", !toLower);
			DebugUtils.Log("Error: Preset " + p_name + " not found!");
			DebugUtils.Log("Presets Count = " + _AllPresets.Count);
			DebugUtils.Log("Presets:");
			foreach (Preset allPreset2 in _AllPresets)
			{
				DebugUtils.Log("- " + allPreset2.Name);
			}
			return null;
		}
	}
}
