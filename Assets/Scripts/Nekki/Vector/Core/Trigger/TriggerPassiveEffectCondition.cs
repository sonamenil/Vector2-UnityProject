using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger.Conditions;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerPassiveEffectCondition : TriggerCondition
	{
		protected TriggerPassiveEffectLoop _Parent;

		protected TriggerPassiveEffectCondition(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
		{
			_Parent = p_parent;
			_IsNot = XmlUtils.ParseBool(p_node.Attributes["Not"]);
		}

		public static TriggerPassiveEffectCondition Create(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
		{
			TriggerPassiveEffectCondition triggerPassiveEffectCondition = null;
			switch (p_node.Name)
			{
			case "Equal":
				return new TPEC_Equal(p_parent, p_node);
			case "Greater":
				return new TPEC_Greater(p_parent, p_node);
			case "GreaterEqual":
				return new TPEC_GreaterEqual(p_parent, p_node);
			case "Less":
				return new TPEC_Less(p_parent, p_node);
			case "LessEqual":
				return new TPEC_LessEqual(p_parent, p_node);
			case "Operator":
				return new TPEC_Operator(p_parent, p_node);
			default:
				if (triggerPassiveEffectCondition == null)
				{
					DebugUtils.Dialog("Unknown condition TPEC" + p_node.Name, true);
				}
				return triggerPassiveEffectCondition;
			}
		}

		public static void Parse(XmlNode p_node, TriggerPassiveEffectLoop p_parent, List<TriggerCondition> p_conditions)
		{
			if (p_node == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				TriggerPassiveEffectCondition triggerPassiveEffectCondition = Create(childNode, p_parent);
				if (triggerPassiveEffectCondition != null)
				{
					p_conditions.Add(triggerPassiveEffectCondition);
				}
			}
		}

		public override void Log(bool result)
		{
		}
	}
}
