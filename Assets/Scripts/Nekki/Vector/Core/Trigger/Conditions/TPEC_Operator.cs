using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TPEC_Operator : TriggerPassiveEffectCondition
	{
		public const string NodeName = "Operator";

		private bool _IsOr;

		public List<TriggerCondition> _Conditions = new List<TriggerCondition>();

		public TPEC_Operator(TriggerPassiveEffectLoop p_parent, XmlNode p_node)
			: base(p_node, p_parent)
		{
			_IsOr = p_node.Attributes["Type"].Value.Equals("Or");
			TriggerPassiveEffectCondition.Parse(p_node, _Parent, _Conditions);
		}

		public override bool Check()
		{
			return (!_IsOr) ? CheckAnd() : CheckOr();
		}

		private bool CheckOr()
		{
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (_Conditions[i].Check())
				{
					return !_IsNot;
				}
			}
			return _IsNot;
		}

		private bool CheckAnd()
		{
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (!_Conditions[i].Check())
				{
					return _IsNot;
				}
			}
			return !_IsNot;
		}

		public override string ToString()
		{
			string text = "And: ";
			foreach (TriggerRunnerCondition condition in _Conditions)
			{
				text = text + "\n    " + condition.ToString();
			}
			return text;
		}

		public override void Log(bool result)
		{
		}
	}
}
