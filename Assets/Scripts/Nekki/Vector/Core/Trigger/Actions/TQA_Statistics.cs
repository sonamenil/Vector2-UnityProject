using System.Xml;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_Statistics : TriggerQuestAction
	{
		private Variable _SignalMessage;

		public TQA_Statistics(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_SignalMessage = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["SignalMessage"], string.Empty), string.Empty);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Trigger_signal, new ArgsDict { { "signal_message", _SignalMessage.ValueString } });
		}

		public override string ToString()
		{
			return "SetString SignalMessage:" + _SignalMessage.DebugStringValue;
		}
	}
}
