using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TQC_Equal : TriggerQuestCondition
	{
		private Variable _Value1;

		private Variable _Value2;

		public TQC_Equal(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_node, p_parent)
		{
			_Value1 = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value1"]), string.Empty);
			_Value2 = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value2"]), string.Empty);
		}

		public override bool Check()
		{
			return (!_IsNot) ? _Value2.IsEqual(_Value1) : (!_Value2.IsEqual(_Value1));
		}
	}
}
