using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class RunPresetData
	{
		public const string NodeName = "RunPreset";

		public static Action<List<PresetResult>> OnPresetEnd = delegate
		{
		};

		private Preset _Parent;

		private List<Variable> _Presets;

		private RunPresetData(Sequence p_node, Preset p_parent)
		{
			_Parent = p_parent;
			_Presets = CreatePresets(p_node);
		}

		public static RunPresetData Create(Sequence p_node, Preset p_parent)
		{
			if (p_node == null)
			{
				return null;
			}
			return new RunPresetData(p_node, p_parent);
		}

		private List<Variable> CreatePresets(Sequence p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			List<Variable> list = new List<Variable>();
			foreach (Scalar item in p_node)
			{
				Variable variable = Variable.CreateVariable(YamlUtils.GetStringValue(item, string.Empty), string.Empty);
				if (variable != null)
				{
					list.Add(variable);
				}
			}
			return (list.Count <= 0) ? null : list;
		}

		public void Activate()
		{
			List<Preset> presetsToRun = GetPresetsToRun();
			if (presetsToRun == null)
			{
				OnPresetEnd(null);
				return;
			}
			List<PresetResult> list = new List<PresetResult>();
			int i = 0;
			for (int count = presetsToRun.Count; i < count; i++)
			{
				PresetResult item = presetsToRun[i].RunPreset();
				list.Add(item);
			}
			OnPresetEnd(list);
		}

		private List<Preset> GetPresetsToRun()
		{
			if (_Parent == null || _Presets == null)
			{
				return null;
			}
			List<Preset> presetBlock = _Parent.PresetBlock;
			if (presetBlock == null)
			{
				return null;
			}
			List<Preset> list = new List<Preset>();
			int i = 0;
			string presetName;
			for (int count = _Presets.Count; i < count; i++)
			{
				presetName = _Presets[i].ValueString;
				Preset preset = presetBlock.Find((Preset p_preset) => p_preset.Name == presetName);
				if (preset != null)
				{
					list.Add(preset);
				}
			}
			return (list.Count <= 0) ? null : list;
		}
	}
}
