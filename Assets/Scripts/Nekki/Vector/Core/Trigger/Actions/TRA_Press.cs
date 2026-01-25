using System.Xml;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Press : TriggerRunnerAction
	{
		private Variable _ModelNameVar;

		private Variable _KeyVar;

		private TRA_Press(TRA_Press p_copyAction)
			: base(p_copyAction)
		{
			_KeyVar = p_copyAction._KeyVar;
			_ModelNameVar = p_copyAction._ModelNameVar;
		}

		public TRA_Press(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _KeyVar, p_node.Attributes["Key"].Value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelNameVar, p_node.Attributes["Model"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			string valueString = _ModelNameVar.ValueString;
			ModelHuman model = GetModel(valueString);
			if (model != null)
			{
				model.ControllerControl.SetKeyVariable_force(new KeyVariables(_KeyVar.ValueString));
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Press(this);
		}

		public override string ToString()
		{
			return "Press Model=" + _ModelNameVar.DebugStringValue + " Key=" + _KeyVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Press");
			VectorLog.Tab(1);
			VectorLog.RunLog("ModelName", _ModelNameVar);
			VectorLog.RunLog("Key", _KeyVar);
			VectorLog.Untab(1);
		}
	}
}
