using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterConditionBalanceElementExists : CounterCondition
	{
		private const int _SearchDepth = 1;

		private List<Variable> _Path;

		public CounterConditionBalanceElementExists(Mapping p_node)
			: base(p_node)
		{
			_Path = new List<Variable>();
			Sequence sequence = p_node.GetSequence("Path");
			foreach (Scalar item2 in sequence)
			{
				Variable item = Variable.CreateVariable(YamlUtils.GetStringValue(item2, string.Empty), string.Empty);
				_Path.Add(item);
			}
			_Path.Add(Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Element"), string.Empty), string.Empty));
		}

		private CounterConditionBalanceElementExists(CounterConditionBalanceElementExists p_copy)
			: base(p_copy)
		{
			_Path = p_copy._Path;
		}

		private List<Variable> GetSearchRequest()
		{
			return _Path.GetRange(0, Math.Min(1, _Path.Count - 1));
		}

		private bool HasBalanceElement()
		{
			if (ZoneResource<ZoneBalanceManager>.Current.HasBalanceElement(GetSearchRequest(), false))
			{
				return ZoneResource<ZoneBalanceManager>.Current.HasBalanceElement(_Path);
			}
			return BalanceManager.Current.HasBalanceElement(_Path);
		}

		public override bool Check()
		{
			bool flag = HasBalanceElement();
			return (!base.Not) ? flag : (!flag);
		}

		public override CounterCondition Copy()
		{
			return new CounterConditionBalanceElementExists(this);
		}

		public override string ToString()
		{
			string text = "Condition BalanceElementExists Path:";
			int i = 0;
			for (int num = _Path.Count - 1; i < num; i++)
			{
				text = text + " " + _Path[i].ValueString;
			}
			string text2 = text;
			return text2 + " Element:" + _Path[_Path.Count - 1].ValueString + " Not:" + base.Not;
		}
	}
}
