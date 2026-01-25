using System.Xml;
using Nekki.Vector.Core.Counter;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_ClearCounter : TriggerQuestAction
	{
		private string _NameCounter;

		private string _NamespaceCounter;

		public TQA_ClearCounter(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_NameCounter = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_NamespaceCounter = XmlUtils.ParseString(p_node.Attributes["Namespace"], _Parent.QuestNamespaceName);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			if (_NameCounter == null)
			{
				CounterController.Current.ClearCounterNamespace(_NamespaceCounter);
			}
			else
			{
				CounterController.Current.RemoveUserCounter(_NameCounter, _NamespaceCounter);
			}
		}
	}
}
