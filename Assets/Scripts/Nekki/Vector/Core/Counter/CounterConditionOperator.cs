using System.Collections.Generic;
using System.Xml;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterConditionOperator : CounterCondition
	{
		private bool _IsTypeAnd;

		private List<CounterCondition> _Conditions;

		public CounterConditionOperator(XmlNode p_node, string p_namespace)
			: base(p_node)
		{
			_IsTypeAnd = p_node.Attributes["Type"].Value == "And";
			_Conditions = CounterCondition.CreateListConditions(p_node, p_namespace);
		}

		public CounterConditionOperator(Mapping p_node, string p_namespace)
			: base(p_node)
		{
			_IsTypeAnd = p_node.GetText("ConditionType").text == "And";
			_Conditions = CounterCondition.CreateListConditions(p_node.GetSequence("Content"), p_namespace);
		}

		private CounterConditionOperator(CounterConditionOperator p_copy)
			: base(p_copy)
		{
			_IsTypeAnd = p_copy._IsTypeAnd;
			_Conditions = p_copy._Conditions;
		}

		public override bool Check()
		{
			if (_IsTypeAnd)
			{
				foreach (CounterCondition condition in _Conditions)
				{
					if (!condition.Check())
					{
						return base.Not;
					}
				}
				return !base.Not;
			}
			foreach (CounterCondition condition2 in _Conditions)
			{
				if (condition2.Check())
				{
					return !base.Not;
				}
			}
			return base.Not;
		}

		public override CounterCondition Copy()
		{
			return new CounterConditionOperator(this);
		}

		public override bool Check(int p_value)
		{
			return false;
		}
	}
}
