using System.Collections.Generic;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class StarterItemsManager : ZoneResource<StarterItemsManager>
	{
		private Dictionary<string, Preset> _PresetsByName = new Dictionary<string, Preset>();

		protected override string ResourceId
		{
			get
			{
				return "StarterItems";
			}
		}

		protected override void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorData, base.FilePath);
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				Preset preset = Preset.Create(item);
				if (preset == null)
				{
					DebugUtils.Dialog(string.Format("Can't create start items preset from - {0}", item.key), false);
				}
				else if (_PresetsByName.ContainsKey(item.key))
				{
					DebugUtils.Dialog(string.Format("Start items preset with name '{0}' already exists", item.key), false);
				}
				else
				{
					_PresetsByName.Add(item.key, preset);
				}
			}
		}

		public Preset GetStartItemPreset(string p_name)
		{
			Preset value = null;
			_PresetsByName.TryGetValue(p_name, out value);
			return value;
		}
	}
}
