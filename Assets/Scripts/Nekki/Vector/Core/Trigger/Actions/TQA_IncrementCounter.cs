using System.Xml;
using Nekki.Vector.Core.Counter;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_IncrementCounter : TriggerQuestAction
	{
		private string _NameCounter;

		private string _NamespaceCounter;

		private int _Value;

		public TQA_IncrementCounter(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_NameCounter = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_NamespaceCounter = XmlUtils.ParseString(p_node.Attributes["Namespace"], _Parent.QuestNamespaceName);
			_Value = XmlUtils.ParseInt(p_node.Attributes["Value"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			CounterController.Current.IncrementUserCounter(_NameCounter, _Value, _NamespaceCounter);
		}
	}
}
