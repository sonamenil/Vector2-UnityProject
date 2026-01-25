using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ActivateNearPlayer : TriggerRunnerAction
	{
		private Variable _ActivationID;

		private TRA_ActivateNearPlayer(TRA_ActivateNearPlayer p_copyAction)
			: base(p_copyAction)
		{
			_ActivationID = p_copyAction._ActivationID;
		}

		public TRA_ActivateNearPlayer(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ActivationID, p_node.Attributes["ActionID"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			List<TriggerRunner> triggersInViewport = RunMainController.Location.TriggersInViewport;
			TRE_ActivateNearPlayer p_event = new TRE_ActivateNearPlayer(_ActivationID.ValueString);
			int i = 0;
			for (int count = triggersInViewport.Count; i < count; i++)
			{
				triggersInViewport[i].CheckEvent(p_event, null);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ActivateNearPlayer(this);
		}

		public override string ToString()
		{
			return "ActivateNearPlayer ID=" + _ActivationID.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ActivateNearPlayer");
			VectorLog.Tab(1);
			VectorLog.RunLog("ActivationID", _ActivationID);
			VectorLog.Untab(1);
		}
	}
}
