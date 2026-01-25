using System.Xml;
using Nekki.Vector.Core.PassiveEffects;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ActivatePassiveEffect : TriggerRunnerAction
	{
		private Variable _ActivationID;

		private TRA_ActivatePassiveEffect(TRA_ActivatePassiveEffect p_copyAction)
			: base(p_copyAction)
		{
			_ActivationID = p_copyAction._ActivationID;
		}

		public TRA_ActivatePassiveEffect(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["ActionID"].Value;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ActivationID, value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			ControllerPassiveEffects.EventActivate(_ActivationID.ValueString);
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ActivatePassiveEffect(this);
		}

		public override string ToString()
		{
			return "ActivatePassiveEffect ID=" + _ActivationID.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ActivatePassiveEffect");
			VectorLog.Tab(1);
			VectorLog.RunLog("ActivationID", _ActivationID);
			VectorLog.Untab(1);
		}
	}
}
