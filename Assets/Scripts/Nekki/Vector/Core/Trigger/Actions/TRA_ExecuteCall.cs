using System.Xml;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_ExecuteCall : TriggerRunnerAction
	{
		private Variable _Message;

		private TRA_ExecuteCall(TRA_ExecuteCall p_copyAction)
			: base(p_copyAction)
		{
			_Message = p_copyAction._Message;
		}

		public TRA_ExecuteCall(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			if (p_node.Attributes["Message"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Message, p_node.Attributes["Message"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			QuestManager.Current.CheckEvent(TQE_OnCall.CalledByTriggerEvent(_Message));
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_ExecuteCall(this);
		}

		public override string ToString()
		{
			return "ExecuteCall";
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: ExecuteCall");
		}
	}
}
