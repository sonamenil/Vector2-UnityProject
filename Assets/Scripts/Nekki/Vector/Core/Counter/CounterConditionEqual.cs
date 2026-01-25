using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterConditionEqual : CounterCondition
	{
		private Variable _Value;

		public CounterConditionEqual(XmlNode p_node, string p_namespace)
			: base(p_node, p_namespace)
		{
			_Value = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value"]), null);
		}

		public CounterConditionEqual(Mapping p_node, string p_namespace)
			: base(p_node, p_namespace)
		{
			_Value = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Value"), "0"), null);
		}

		private CounterConditionEqual(CounterConditionEqual p_copy)
			: base(p_copy)
		{
			_Value = p_copy._Value;
		}

		public override bool Check()
		{
			return Check(CounterController.Current.GetUserCounter(base.Name, base.Namespace));
		}

		public override bool Check(int p_value)
		{
			return (!base.Not) ? (p_value == _Value.ValueInt) : (p_value != _Value.ValueInt);
		}

		public override bool CheckRange(Point p_range)
		{
			return p_range.X <= (float)_Value.ValueInt && (float)_Value.ValueInt <= p_range.Y;
		}

		public override CounterCondition Copy()
		{
			return new CounterConditionEqual(this);
		}

		public override string ToString()
		{
			return "Condition Equal Name:" + base.Name + " NS:" + base.Namespace + " Value:" + _Value.ValueInt + " Not:" + base.Not;
		}
	}
}
