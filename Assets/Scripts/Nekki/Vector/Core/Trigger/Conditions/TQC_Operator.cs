using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TQC_Operator : TriggerQuestCondition
	{
		private bool _IsOr;

		public List<TriggerCondition> _Conditions = new List<TriggerCondition>();

		public TQC_Operator(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_node, p_parent)
		{
			_IsOr = XmlUtils.ParseString(p_node.Attributes["Type"], "Or") == "Or";
			TriggerQuestCondition.Parse(p_node, _Parent, _Conditions);
		}

		public override bool Check()
		{
			bool flag = ((!_IsOr) ? CheckAnd() : CheckOr());
			return (!_IsNot) ? flag : (!flag);
		}

		private bool CheckOr()
		{
			if (Settings.WriteRunLogs)
			{
				VectorLog.Tab(1);
				VectorLog.RunLog("ConditionType: " + TypeString());
				VectorLog.Tab(1);
				VectorLog.RunLog("Not: " + (_IsNot ? 1 : 0));
				VectorLog.RunLog("CheckConditions:");
			}
			foreach (TriggerCondition condition in _Conditions)
			{
				if (condition.Check())
				{
					if (Settings.WriteRunLogs)
					{
						condition.Log(false);
						VectorLog.RunLog("Result: " + !_IsNot);
						VectorLog.Untab(2);
					}
					return !_IsNot;
				}
				if (Settings.WriteRunLogs)
				{
					condition.Log(true);
				}
			}
			if (Settings.WriteRunLogs)
			{
				VectorLog.RunLog("Result: " + _IsNot);
				VectorLog.Untab(2);
			}
			return _IsNot;
		}

		private bool CheckAnd()
		{
			if (Settings.WriteRunLogs)
			{
				VectorLog.Tab(1);
				VectorLog.RunLog("ConditionType: " + TypeString());
				VectorLog.Tab(1);
				VectorLog.RunLog("Not: " + (_IsNot ? 1 : 0));
				VectorLog.RunLog("CheckConditions:");
			}
			foreach (TriggerCondition condition in _Conditions)
			{
				if (!condition.Check())
				{
					if (Settings.WriteRunLogs)
					{
						condition.Log(false);
						VectorLog.RunLog("Result: " + _IsNot);
						VectorLog.Untab(2);
					}
					return _IsNot;
				}
				if (Settings.WriteRunLogs)
				{
					condition.Log(true);
				}
			}
			if (Settings.WriteRunLogs)
			{
				VectorLog.RunLog("Result: " + !_IsNot);
				VectorLog.Untab(2);
			}
			return !_IsNot;
		}

		protected string TypeString()
		{
			return "Operator" + ((!_IsOr) ? "_And" : "_Or");
		}

		public override string ToString()
		{
			string text = ((!_IsOr) ? "And: " : "Or: ");
			foreach (TriggerCondition condition in _Conditions)
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
