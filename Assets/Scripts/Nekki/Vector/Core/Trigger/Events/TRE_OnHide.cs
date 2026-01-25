using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_OnHide : TriggerRunnerEvent
	{
		public TRE_OnHide()
		{
			_Type = EventType.TRE_ON_HIDE;
		}

		public TRE_OnHide(TriggerRunner p_parent)
		{
			TRE_OnHide p_event = this;
			_Type = EventType.TRE_ON_HIDE;
			if (p_parent != null)
			{
                p_parent.Controller.OnBecameInvisibleEvent = delegate
                {
                    p_parent.CheckEvent(p_event, null);
                };
            }
		}
	}
}
