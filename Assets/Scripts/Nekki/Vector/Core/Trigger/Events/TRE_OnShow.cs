using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_OnShow : TriggerRunnerEvent
	{
		public TRE_OnShow()
		{
			_Type = EventType.TRE_ON_SHOW;
		}

		public TRE_OnShow(TriggerRunner p_parent)
		{
			TRE_OnShow p_event = this;
			_Type = EventType.TRE_ON_SHOW;
			if (p_parent != null)
			{
				p_parent.Controller.OnBecameVisibleEvent = delegate
				{
					p_parent.CheckEvent(p_event, null);
				};
			}
		}
	}
}
