using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerPassiveEffectEvent : TriggerEvent
	{
		public static TriggerPassiveEffectEvent Create(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
		{
			switch (p_node.Name)
			{
			case "FloorStart":
				return new TPEE_FloorStart(p_parent);
			case "FloorEnd":
				return new TPEE_FloorEnd(p_parent);
			case "Activate":
				return new TPEE_Activate(p_parent);
			case "AnimationStart":
				return new TPEE_AnimationStart(p_parent);
			default:
				return null;
			}
		}

		public static void Parse(XmlNode p_node, TriggerPassiveEffectLoop p_parent, List<TriggerEvent> p_result)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				TriggerPassiveEffectEvent triggerPassiveEffectEvent = Create(childNode, p_parent);
				if (triggerPassiveEffectEvent != null)
				{
					p_result.Add(triggerPassiveEffectEvent);
				}
			}
		}

		protected static void CheckEvent(TriggerEvent p_event, List<TriggerPassiveEffectLoop> p_loops, List<List<TriggerPassiveEffectAction>> p_delayAction)
		{
			if (p_loops == null)
			{
				return;
			}
			for (int i = 0; i < p_loops.Count; i++)
			{
				if (p_loops[i].CheckEvent(p_event))
				{
					p_loops[i].ActivateActions(p_delayAction);
				}
			}
		}

		public static void AllFree()
		{
			TPEE_Activate.Free();
			TPEE_AnimationStart.Free();
			TPEE_FloorStart.Free();
			TPEE_FloorEnd.Free();
		}
	}
}
