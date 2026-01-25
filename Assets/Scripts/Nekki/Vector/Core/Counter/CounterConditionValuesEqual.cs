using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterConditionValuesEqual : CounterCondition
	{
		private List<Variable> _Values;

		public CounterConditionValuesEqual(Mapping p_node)
			: base(p_node)
		{
			_Values = new List<Variable>();
			Sequence sequence = p_node.GetSequence("Values");
			foreach (Scalar item2 in sequence)
			{
				Variable item = Variable.CreateVariable(YamlUtils.GetStringValue(item2, string.Empty), string.Empty);
				_Values.Add(item);
			}
		}

		private CounterConditionValuesEqual(CounterConditionValuesEqual p_copy)
			: base(p_copy)
		{
			_Values = p_copy._Values;
		}

		public override bool Check()
		{
			for (int i = 1; i < _Values.Count; i++)
			{
				if (!_Values[0].IsEqual(_Values[i]))
				{
					return base.Not;
				}
			}
			return !base.Not;
		}

		public override CounterCondition Copy()
		{
			return new CounterConditionValuesEqual(this);
		}

		public override string ToString()
		{
			string text = "Condition ValuesEqual Name:" + base.Name + " NS:" + base.Namespace + " Values:";
			foreach (Variable value in _Values)
			{
				text = text + " " + value.ValueString;
			}
			return text;
		}
	}
}
