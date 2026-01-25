using System.Collections.Generic;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TPEE_Activate : TriggerPassiveEffectEvent
	{
		public const string NodeName = "Activate";

		public static List<TriggerPassiveEffectLoop> LoopsWithEvent;

		public TPEE_Activate(TriggerPassiveEffectLoop p_parent)
		{
			_Type = EventType.TPEE_ACTIVATE;
			if (p_parent != null)
			{
				if (LoopsWithEvent == null)
				{
					LoopsWithEvent = new List<TriggerPassiveEffectLoop>();
				}
				if (!LoopsWithEvent.Contains(p_parent))
				{
					LoopsWithEvent.Add(p_parent);
				}
			}
		}

		public override bool IsEqual(TriggerEvent p_value)
		{
			return base.IsEqual(p_value);
		}

		public static void CheckEvent(List<List<TriggerPassiveEffectAction>> p_delayAction)
		{
			TriggerPassiveEffectEvent.CheckEvent(new TPEE_Activate(null), LoopsWithEvent, p_delayAction);
		}

		public static void Free()
		{
			if (LoopsWithEvent != null)
			{
				LoopsWithEvent.Clear();
			}
			LoopsWithEvent = null;
		}
	}
}
