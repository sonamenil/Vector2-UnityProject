using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_SetCounter : TriggerQuestAction
	{
		private string _NameCounter;

		private string _NamespaceCounter;

		private Variable _Value;

		public TQA_SetCounter(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_NameCounter = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_NamespaceCounter = XmlUtils.ParseString(p_node.Attributes["Namespace"], _Parent.QuestNamespaceName);
			_Value = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value"]), string.Empty);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			CounterController.Current.CreateCounterOrSetValue(_NameCounter, _Value.ValueInt, _NamespaceCounter);
		}
	}
}
