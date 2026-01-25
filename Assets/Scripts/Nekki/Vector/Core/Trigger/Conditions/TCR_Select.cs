using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TCR_Select : TriggerRunnerCondition
	{
		private VariableItem _ObjectVar;

		private Variable _GroupNameVar;

		private Variable _SectionNameVar;

		private List<TriggerRunnerCondition> conditions;

		public TCR_Select(TriggerRunnerLoop p_parent, XmlNode p_node)
			: base(p_parent, p_node)
		{
			Parse(p_node);
		}

		public void Parse(XmlNode p_node)
		{
			Variable variable = GetVariable(p_node.Attributes["Object"].Value);
			if (variable.Type != VariableType.Item)
			{
				return;
			}
			_ObjectVar = variable as VariableItem;
			_GroupNameVar = GetOrCreateVar(p_node.Attributes["From"].Value);
			string text = ((p_node.Attributes["Section"] != null) ? p_node.Attributes["Section"].Value : null);
			if (text != null)
			{
				_SectionNameVar = GetOrCreateVar(text);
			}
			conditions = new List<TriggerRunnerCondition>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				conditions.Add(TriggerRunnerCondition.Create(_Parent, childNode, TriggerRunnerCondition.newNamesVars));
			}
		}

		public override bool Check()
		{
			VectorLog.Tab(1);
			VectorLog.RunLog("ConditionType: " + TypeString());
			VectorLog.Tab(1);
			VectorLog.RunLog("Not: " + (_IsNot ? 1 : 0));
			VectorLog.RunLog("Object", _ObjectVar);
			VectorLog.RunLog("GroupName", _GroupNameVar);
			VectorLog.RunLog("SectionName", _SectionNameVar);
			HashSet<UserItem> hashSet = null;
			if (_SectionNameVar == null)
			{
				hashSet = DataLocal.Current.AllItems;
			}
			else if (_SectionNameVar.ValueString == "Stash")
			{
				hashSet = DataLocal.Current.Stash;
			}
			else
			{
				if (!(_SectionNameVar.ValueString == "Equipped"))
				{
					VectorLog.RunLog("SelectResult: " + !_IsNot);
					VectorLog.Untab(2);
					return !_IsNot;
				}
				hashSet = DataLocal.Current.Equipped;
			}
			int num = 0;
			foreach (UserItem item in hashSet)
			{
				if (item.ContainsGroup(_GroupNameVar.ValueString))
				{
					if (num == 0)
					{
						VectorLog.RunLog("SelectConditions:");
						VectorLog.Tab(1);
					}
					num++;
					VectorLog.RunLog("Attempt: " + num);
					_ObjectVar.Item = item;
					if (CheckConditions())
					{
						VectorLog.Tab(1);
						VectorLog.RunLog("SelectResult: " + !_IsNot);
						VectorLog.Untab(4);
						return !_IsNot;
					}
				}
			}
			if (num == 0)
			{
				VectorLog.RunLog("SelectResult: " + _IsNot);
			}
			else
			{
				VectorLog.Tab(1);
				VectorLog.RunLog("SelectResult: " + _IsNot);
				VectorLog.Untab(2);
			}
			VectorLog.Untab(2);
			return _IsNot;
		}

		private bool CheckConditions()
		{
			int count = conditions.Count;
			for (int i = 0; i < count; i++)
			{
				if (!conditions[i].Check())
				{
					conditions[i].Log(false);
					return false;
				}
				conditions[i].Log(true);
			}
			return true;
		}

		protected override string TypeString()
		{
			return "Select";
		}

		public override string ToString()
		{
			string text = "Select From:" + _GroupNameVar.DebugStringValue + " Selection:" + ((_SectionNameVar != null) ? _SectionNameVar.DebugStringValue : "All") + " To:" + _ObjectVar.ToString();
			if (Check())
			{
				return text + "\n     Result:true Item:" + ((_ObjectVar.Item != null) ? _ObjectVar.Item.VisualName : "null");
			}
			return text + "\n     Result:false";
		}

		public override void Log(bool result)
		{
		}
	}
}
