using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_ActivateNearPlayer : TriggerRunnerEvent
	{
		public string ActionID;

		public TRE_ActivateNearPlayer(string p_ActionID)
		{
			ActionID = p_ActionID;
			_Type = EventType.TRE_ACTIVATE_NEAR_PLAYER;
		}

		public TRE_ActivateNearPlayer(string p_ActionID, TriggerRunner p_parent)
		{
			ActionID = p_ActionID;
			_Type = EventType.TRE_ACTIVATE_NEAR_PLAYER;
			if (p_parent != null)
			{
				//p_parent.CreateCameraDetector();
			}
		}
	}
}
