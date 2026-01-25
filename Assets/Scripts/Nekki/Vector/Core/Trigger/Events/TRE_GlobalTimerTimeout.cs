using System.Collections.Generic;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_GlobalTimerTimeout : TriggerRunnerEvent
	{
		private static List<TriggerRunner> _WithThisEvent;

		public TRE_GlobalTimerTimeout(TriggerRunner p_parent)
		{
			_Type = EventType.TRE_GLOBAL_TIMEOUT;
			if (p_parent != null)
			{
				AddToWithThisEvent(p_parent);
			}
		}

		private static void AddToWithThisEvent(TriggerRunner p_parent)
		{
			if (_WithThisEvent == null)
			{
				_WithThisEvent = new List<TriggerRunner>();
			}
			if (!_WithThisEvent.Contains(p_parent))
			{
				_WithThisEvent.Add(p_parent);
			}
		}

		public static void ActivateThisEvent()
		{
			if (_WithThisEvent == null)
			{
				return;
			}
			TRE_GlobalTimerTimeout p_event = new TRE_GlobalTimerTimeout(null);
			foreach (TriggerRunner item in _WithThisEvent)
			{
				item.CheckEvent(p_event, null);
			}
		}

		public static void Reset()
		{
			_WithThisEvent = null;
		}
	}
}
