using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_AddItem : TriggerRunnerAction
	{
		private Variable _Name;

		private Variable _Preset;

		private Variable _Result;

		private TRA_AddItem(TRA_AddItem p_copy)
			: base(p_copy)
		{
			_Name = p_copy._Name;
			_Preset = p_copy._Preset;
			_Result = p_copy._Result;
		}

		public TRA_AddItem(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			if (p_node.Attributes["Name"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Name, p_node.Attributes["Name"].Value);
			}
			if (p_node.Attributes["Result"] != null)
			{
				_Result = p_parent.GetParentVar(p_node.Attributes["Result"].Value);
				if (_Result == null)
				{
					DebugUtils.Dialog("Action AddItem In trigger " + p_parent.ParentTrigger.Name + "Not Result var by name" + p_node.Attributes["Result"].Value, true);
				}
				if (_Result.Type != VariableType.Item)
				{
					DebugUtils.Dialog("Result var in Action AddItem In trigger " + p_parent.ParentTrigger.Name + "wrong type", true);
				}
			}
			try
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Preset, p_node.Attributes["Preset"].Value);
			}
			catch
			{
				DebugUtils.Dialog("Error: Parse AddItem action.", true);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			Preset presetByName = PresetsManager.GetPresetByName(_Preset.ValueString);
			if (presetByName == null)
			{
				DebugUtils.Dialog(string.Concat("Error: Preset \"", _Preset, "\" not founded!"), true);
				return;
			}
			for (int i = 0; i < presetByName.ItemsCount.ValueInt; i++)
			{
				PresetResult presetResult = presetByName.RunPreset();
				if (presetResult.Result)
				{
					if (_Result != null)
					{
						(_Result as VariableItem).Item = presetResult.Item;
					}
					if (_Name != null)
					{
						presetResult.Item.Name = _Name.ValueString;
					}
				}
			}
			DataLocal.Current.Save(false);
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_AddItem(this);
		}

		public override string ToString()
		{
			return string.Format("[AddItem]");
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: AddItem");
			VectorLog.Tab(1);
			VectorLog.RunLog("Name", _Name);
			VectorLog.RunLog("Preset", _Preset);
			VectorLog.RunLog("Result", _Result);
			VectorLog.Untab(1);
		}
	}
}
