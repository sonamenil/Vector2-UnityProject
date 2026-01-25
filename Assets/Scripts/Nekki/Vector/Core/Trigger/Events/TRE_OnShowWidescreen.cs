using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_OnShowWidescreen : TriggerRunnerEvent
	{
		public TRE_OnShowWidescreen()
		{
			_Type = EventType.TRE_ON_SHOW_WIDESCREEN;
		}

		public TRE_OnShowWidescreen(TriggerRunner p_parent)
		{
			TRE_OnShowWidescreen p_event = this;
			_Type = EventType.TRE_ON_SHOW_WIDESCREEN;
			if (p_parent != null)
			{
				p_parent.CreateCameraDetectorWidescreen();
				p_parent.DetectorWS.OnBecameVisibleEvent = delegate
				{
					p_parent.CheckEvent(p_event, null);
				};
			}
		}
	}
}
