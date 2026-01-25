using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger.Conditions;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerQuestCondition : TriggerCondition
	{
		protected TriggerQuestLoop _Parent;

		protected TriggerQuestCondition(XmlNode p_node, TriggerQuestLoop p_parent)
		{
			_Parent = p_parent;
			_IsNot = XmlUtils.ParseBool(p_node.Attributes["Not"]);
		}

		public static TriggerQuestCondition Create(XmlNode p_node, TriggerQuestLoop p_parent)
		{
			TriggerQuestCondition triggerQuestCondition = null;
			switch (p_node.Name)
			{
			case "CounterRange":
				return new TQC_CounterRange(p_node, p_parent);
			case "GroupExist":
				return new TQC_GroupExist(p_node, p_parent);
			case "Equal":
				return new TQC_Equal(p_node, p_parent);
			case "Operator":
				return new TQC_Operator(p_node, p_parent);
			case "CheckScreenType":
				return new TQC_CheckScreenType(p_node, p_parent);
			default:
				if (triggerQuestCondition == null)
				{
					DebugUtils.Dialog("Unknown condition " + p_node.Name, true);
				}
				return triggerQuestCondition;
			}
		}

		public static void Parse(XmlNode p_node, TriggerQuestLoop p_parent, List<TriggerCondition> p_conditions)
		{
			if (p_node == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				TriggerQuestCondition triggerQuestCondition = Create(childNode, p_parent);
				if (triggerQuestCondition != null)
				{
					p_conditions.Add(triggerQuestCondition);
				}
			}
		}

		public override void Log(bool result)
		{
		}
	}
}
