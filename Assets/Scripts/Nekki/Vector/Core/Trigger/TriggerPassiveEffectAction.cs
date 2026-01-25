using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.PassiveEffects;
using Nekki.Vector.Core.Trigger.Actions;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerPassiveEffectAction
	{
		protected TriggerPassiveEffectLoop _Parent;

		public virtual int Frames
		{
			get
			{
				return 0;
			}
		}

		protected TriggerPassiveEffectAction(TriggerPassiveEffectLoop p_parent)
		{
			_Parent = p_parent;
		}

		protected TriggerPassiveEffectAction(TriggerPassiveEffectAction p_copy)
		{
			_Parent = p_copy._Parent;
		}

		public static void Parse(XmlNode p_node, TriggerPassiveEffectLoop p_parent, List<TriggerPassiveEffectAction> p_result)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				TriggerPassiveEffectAction triggerPassiveEffectAction = Create(childNode, p_parent);
				if (triggerPassiveEffectAction != null)
				{
					p_result.Add(triggerPassiveEffectAction);
				}
			}
		}

		private static TriggerPassiveEffectAction Create(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
		{
			switch (p_node.Name)
			{
			case "AddCharge":
				return new TPEA_AddCharge(p_node, p_parent);
			case "SetCharge":
				return new TPEA_SetCharge(p_node, p_parent);
			case "KillPassiveEffect":
				return new TPEA_KillPassiveEffect(p_node, p_parent);
			case "SetVariable":
				return new TPEA_SetVariable(p_node, p_parent);
			case "ShowPassiveEffect":
				return new TPEA_ShowPassiveEffect(p_node, p_parent);
			case "Wait":
				return new TPEA_Wait(p_node, p_parent);
			case "Log":
				return new TPEA_Log(p_node, p_parent);
			default:
				DebugUtils.Dialog("Unknown action: " + p_node.Name, true);
				return null;
			}
		}

		public abstract void Activate(ref bool p_runNext);

		public abstract TriggerPassiveEffectAction Copy();

		protected static void InitActionVar(TriggerPassiveEffect p_parent, ref Variable p_var, string p_nameOrValue)
		{
			if (p_nameOrValue.Length > 1 && p_nameOrValue[0] == '_')
			{
				p_var = p_parent.GetOrCreateVar(p_nameOrValue);
			}
			else
			{
				p_var = Variable.CreateVariable(p_nameOrValue, string.Empty, p_parent);
			}
		}
	}
}
