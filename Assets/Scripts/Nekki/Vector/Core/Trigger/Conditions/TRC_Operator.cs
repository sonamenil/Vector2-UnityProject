using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TRC_Operator : TriggerRunnerCondition
	{
		private bool _IsOr;

		public List<TriggerCondition> _Conditions = new List<TriggerCondition>();

		public TRC_Operator(TriggerRunnerLoop p_parent, XmlNode p_node)
			: base(p_parent, p_node)
		{
			_IsOr = p_node.Attributes["Type"].Value.Equals("Or");
			TriggerRunnerCondition.Parse(p_node, _Parent, _Conditions);
		}

		public override bool Check()
		{
			return (!_IsOr) ? CheckAnd() : CheckOr();
		}

		private bool CheckOr()
		{
			VectorLog.Tab(1);
			VectorLog.RunLog("ConditionType: " + TypeString());
			VectorLog.Tab(1);
			VectorLog.RunLog("Not: " + (_IsNot ? 1 : 0));
			VectorLog.RunLog("CheckConditions:");
			foreach (TriggerRunnerCondition condition in _Conditions)
			{
				if (condition.Check())
				{
					condition.Log(false);
					VectorLog.RunLog("Result: " + !_IsNot);
					VectorLog.Untab(2);
					return !_IsNot;
				}
				condition.Log(true);
			}
			VectorLog.RunLog("Result: " + _IsNot);
			VectorLog.Untab(2);
			return _IsNot;
		}

		private bool CheckAnd()
		{
			VectorLog.Tab(1);
			VectorLog.RunLog("ConditionType: " + TypeString());
			VectorLog.Tab(1);
			VectorLog.RunLog("Not: " + (_IsNot ? 1 : 0));
			VectorLog.RunLog("CheckConditions:");
			foreach (TriggerRunnerCondition condition in _Conditions)
			{
				if (!condition.Check())
				{
					condition.Log(false);
					VectorLog.RunLog("Result: " + _IsNot);
					VectorLog.Untab(2);
					return _IsNot;
				}
				condition.Log(true);
			}
			VectorLog.RunLog("Result: " + !_IsNot);
			VectorLog.Untab(2);
			return !_IsNot;
		}

		protected override string TypeString()
		{
			return "Operator" + ((!_IsOr) ? "_And" : "_Or");
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
