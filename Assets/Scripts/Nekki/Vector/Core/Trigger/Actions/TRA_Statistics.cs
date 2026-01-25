using System.Xml;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Statistics : TriggerRunnerAction
	{
		private Variable _SignalMessage;

		public TRA_Statistics(TRA_Statistics p_copyAction)
			: base(p_copyAction)
		{
			_SignalMessage = p_copyAction._SignalMessage;
		}

		public TRA_Statistics(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SignalMessage, p_node.Attributes["SignalMessage"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Trigger_signal, new ArgsDict { { "signal_message", _SignalMessage.ValueString } });
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Statistics(this);
		}

		public override string ToString()
		{
			return "TRA_Statistics _SignalMessage=" + _SignalMessage.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Statistics");
			VectorLog.Tab(1);
			VectorLog.RunLog("SignalMessage", _SignalMessage);
			VectorLog.Untab(1);
		}
	}
}
