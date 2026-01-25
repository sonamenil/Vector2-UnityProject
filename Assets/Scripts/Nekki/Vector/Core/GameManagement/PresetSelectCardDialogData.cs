using System.Collections.Generic;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class PresetSelectCardDialogData : PresetDialogData
	{
		private Variable _GadgetName;

		private Variable _ActiveCard;

		public PresetSelectCardDialogData(Mapping p_node, Preset p_preset)
			: base(p_node, p_preset)
		{
			_GadgetName = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Gadget"), string.Empty), null);
			_ActiveCard = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("ActiveCard"), string.Empty), null);
		}

		public override void Show()
		{
			GadgetItem gadgetItem = GadgetItem.Create(DataLocal.Current.GetItemByName(_GadgetName.ValueString));
			if (gadgetItem != null)
			{
				CardsGroupAttribute p_card = CardsGroupAttribute.Create(_ActiveCard.ValueString);
				DialogNotificationManager.ShowSelectCardDialog(gadgetItem, p_card, Answer);
			}
		}

		public void Answer(bool p_result)
		{
			if (p_result)
			{
				OnPositive();
			}
			else
			{
				OnNegative();
			}
		}

		public void OnPositive()
		{
			List<Preset> nextRunPresets = GetNextRunPresets();
			if (nextRunPresets == null)
			{
				if (OnPresetEnd != null)
				{
					OnPresetEnd(null);
					OnPresetEnd = null;
				}
				return;
			}
			List<PresetResult> list = new List<PresetResult>();
			for (int i = 0; i < nextRunPresets.Count; i++)
			{
				PresetResult item = nextRunPresets[i].RunPreset();
				list.Add(item);
			}
			if (OnPresetEnd != null)
			{
				OnPresetEnd(list);
				OnPresetEnd = null;
			}
		}

		public void OnNegative()
		{
			if (OnPresetEnd != null)
			{
				OnPresetEnd(null);
				OnPresetEnd = null;
			}
		}
	}
}
