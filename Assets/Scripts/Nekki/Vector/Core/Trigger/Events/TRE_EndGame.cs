using System.Collections.Generic;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_EndGame : TriggerRunnerEvent
	{
		private static List<TriggerRunner> _WithThisEvent;

		public TRE_EndGame(TriggerRunner p_parent)
		{
			_Type = EventType.TRE_END_GAME;
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
			TRE_EndGame p_event = new TRE_EndGame(null);
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
