using System.Collections.Generic;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class CardPresetsManager
	{
		private const string _FileName = "cards_presets.yaml";

		private static List<Preset> _Presets = new List<Preset>();

		public static void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "cards_presets.yaml");
			_Presets.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_Presets.Add(Preset.Create(item, _Presets));
			}
		}

		public static void Reset()
		{
			_Presets.Clear();
		}

		public static PresetResult UsePreset(CardsGroupAttribute p_card)
		{
			Preset presetByName = GetPresetByName(p_card.ExecutePresetName);
			if (presetByName == null)
			{
				DebugUtils.Log("Preset: " + p_card.ExecutePresetName + " no in cards presets");
				return null;
			}
			StringBuffer.AddString("CardName", p_card.CardName);
			return presetByName.RunPreset();
		}

		private static Preset GetPresetByName(string p_name)
		{
			for (int i = 0; i < _Presets.Count; i++)
			{
				if (_Presets[i].Name == p_name)
				{
					return _Presets[i];
				}
			}
			return null;
		}
	}
}
