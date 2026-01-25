using System.Xml;
using Nekki.Vector.Core.Counter;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TQC_CounterRange : TriggerQuestCondition
	{
		private CounterConditionRange _Condition;

		public TQC_CounterRange(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_node, p_parent)
		{
			_Condition = new CounterConditionRange(p_node, _Parent.QuestNamespaceName);
		}

		public override bool Check()
		{
			return _Condition.Check();
		}
	}
}
