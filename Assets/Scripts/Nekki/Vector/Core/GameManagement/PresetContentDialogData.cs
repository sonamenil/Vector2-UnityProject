using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class PresetContentDialogData : PresetDialogData
	{
		private Variable _ButtonPositiveText;

		private Variable _ButtonNegativeText;

		public PresetContentDialogData(Mapping p_node, Preset p_preset)
			: base(p_node, p_preset)
		{
			_ButtonPositiveText = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("ButtonPositive"), string.Empty), null);
			_ButtonNegativeText = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("ButtonNegative"), string.Empty), null);
		}

		public override void Show()
		{
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(OnPositiveBtnTap, _ButtonPositiveText.ValueString, ButtonUI.Type.Green));
			if (_ButtonNegativeText.ValueString != string.Empty)
			{
				list.Add(new DialogButtonData(OnNegativeBtnTap, _ButtonNegativeText.ValueString, ButtonUI.Type.Red));
			}
			DialogNotificationManager.ShowInfoDialog(list, _Title.ValueString, _Text.ValueString);
		}

		public void OnPositiveBtnTap(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
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
			StringBuffer.AddString("DialogResult", "Positive");
			List<PresetResult> list = new List<PresetResult>();
			for (int i = 0; i < nextRunPresets.Count; i++)
			{
				PresetResult item = nextRunPresets[i].RunPreset();
				list.Add(item);
			}
			StringBuffer.AddString("DialogResult", string.Empty);
			if (OnPresetEnd != null)
			{
				OnPresetEnd(list);
				OnPresetEnd = null;
			}
		}

		public void OnNegativeBtnTap(BaseDialog p_dialog)
		{
			p_dialog.Dismiss();
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
			StringBuffer.AddString("DialogResult", "Negative");
			List<PresetResult> list = new List<PresetResult>();
			for (int i = 0; i < nextRunPresets.Count; i++)
			{
				PresetResult item = nextRunPresets[i].RunPreset();
				list.Add(item);
			}
			StringBuffer.AddString("DialogResult", string.Empty);
			if (OnPresetEnd != null)
			{
				OnPresetEnd(list);
				OnPresetEnd = null;
			}
		}
	}
}
