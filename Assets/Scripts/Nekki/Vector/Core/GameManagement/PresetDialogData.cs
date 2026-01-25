using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public abstract class PresetDialogData
	{
		public const string NodeName = "ShowDialog";

		public const string DialogResultStr = "DialogResult";

		protected Preset _Parent;

		protected Variable _Title;

		protected Variable _Text;

		protected Variable _PresetName;

		public Action<List<PresetResult>> OnPresetEnd;

		protected PresetDialogData(Mapping p_node, Preset p_preset)
		{
			_Parent = p_preset;
			_Title = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Title"), string.Empty), null);
			_Text = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Text"), string.Empty), null);
			_PresetName = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("RunNextPreset"), string.Empty), null);
		}

		public static PresetDialogData Create(Mapping p_node, Preset p_preset)
		{
			if (p_node == null)
			{
				return null;
			}
			switch (YamlUtils.GetStringValue(p_node.GetText("Type"), string.Empty))
			{
			case "Information":
				return new PresetContentDialogData(p_node, p_preset);
			case "SelectCard":
				return new PresetSelectCardDialogData(p_node, p_preset);
			default:
				return null;
			}
		}

		public abstract void Show();

		protected List<Preset> GetNextRunPresets()
		{
			if (_PresetName.ValueString == string.Empty)
			{
				return null;
			}
			if (_Parent == null)
			{
				return null;
			}
			List<Preset> presetBlock = _Parent.PresetBlock;
			if (presetBlock == null)
			{
				return null;
			}
			List<Preset> list = new List<Preset>();
			string[] array = _PresetName.ValueString.Split('|');
			for (int i = 0; i < presetBlock.Count; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (presetBlock[i].Name == array[j])
					{
						list.Add(presetBlock[i]);
					}
				}
			}
			if (list.Count == 0)
			{
				list = null;
			}
			return list;
		}
	}
}
