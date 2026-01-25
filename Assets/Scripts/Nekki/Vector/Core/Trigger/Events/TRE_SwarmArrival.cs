using System.Collections.Generic;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_SwarmArrival : TriggerRunnerEvent
	{
		private static List<TriggerRunner> _WithThisEvent;

		public string WayPointKey;

		public TRE_SwarmArrival(TriggerRunner p_parent, string p_wayPointKey)
		{
			_Type = EventType.TRE_SWARM_ARRIVAL;
			WayPointKey = p_wayPointKey;
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

		public static void ActivateThisEvent(string p_wayPointKey)
		{
			if (_WithThisEvent == null)
			{
				return;
			}
			TRE_SwarmArrival p_event = new TRE_SwarmArrival(null, p_wayPointKey);
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
