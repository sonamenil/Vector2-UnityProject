using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_RunModelEffect : TriggerRunnerAction
	{
		private Variable _EffectName;

		private Variable _Model;

		private TRA_RunModelEffect(TRA_RunModelEffect p_copyAction)
			: base(p_copyAction)
		{
			_EffectName = p_copyAction._EffectName;
			_Model = p_copyAction._Model;
		}

		public TRA_RunModelEffect(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["Name"].Value;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _EffectName, value);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Model, p_node.Attributes["Model"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			ModelHuman model = GetModel(_Model.ValueString);
			if (model != null)
			{
				model.ControllerModelEffect.RunEffect(_EffectName.ValueString, _ParentLoop.ParentTrigger);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_RunModelEffect(this);
		}

		public override string ToString()
		{
			return "RunModelEffect Name=" + _EffectName.ValueString;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: RunModelEffect");
			VectorLog.Tab(1);
			VectorLog.RunLog("Name", _EffectName);
			VectorLog.Untab(1);
		}
	}
}
