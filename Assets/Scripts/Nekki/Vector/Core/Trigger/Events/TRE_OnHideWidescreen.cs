using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_OnHideWidescreen : TriggerRunnerEvent
	{
		public TRE_OnHideWidescreen()
		{
			_Type = EventType.TRE_ON_HIDE_WIDESCREEN;
		}

		public TRE_OnHideWidescreen(TriggerRunner p_parent)
		{
			TRE_OnHideWidescreen p_event = this;
			_Type = EventType.TRE_ON_HIDE_WIDESCREEN;
			if (p_parent != null)
			{
				p_parent.CreateCameraDetectorWidescreen();
				p_parent.DetectorWS.OnBecameInvisibleEvent = delegate
				{
					p_parent.CheckEvent(p_event, null);
				};
			}
		}
	}
}
