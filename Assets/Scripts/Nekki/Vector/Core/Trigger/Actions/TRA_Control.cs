using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Control : TriggerRunnerAction
	{
		private Variable _ModelNameVar;

		private Variable _SwitchVar;

		private TRA_Control(TRA_Control p_copyAction)
			: base(p_copyAction)
		{
			_ModelNameVar = p_copyAction._ModelNameVar;
			_SwitchVar = p_copyAction._SwitchVar;
		}

		public TRA_Control(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["Model"].Value;
			string value2 = p_node.Attributes["Switch"].Value;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SwitchVar, value2);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelNameVar, value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			string valueString = _ModelNameVar.ValueString;
			ModelHuman model = GetModel(valueString);
			if (model != null)
			{
				model.ControllerControl.Enable = _SwitchVar.ValueString == "On";
				model.ControllerControl.ClearKey();
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Control(this);
		}

		public override string ToString()
		{
			return "Control: Model=" + _ModelNameVar.DebugStringValue + " Switch:" + _SwitchVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Control");
			VectorLog.Tab(1);
			VectorLog.RunLog("ModelName", _ModelNameVar);
			VectorLog.RunLog("Switch", _SwitchVar);
			VectorLog.Untab(1);
		}
	}
}
