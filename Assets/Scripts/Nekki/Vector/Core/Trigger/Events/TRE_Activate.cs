namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_Activate : TriggerRunnerEvent
	{
		public string ActionID;

		public TRE_Activate(string p_ActionID)
		{
			ActionID = p_ActionID;
			_Type = EventType.TRE_ACTIVATE;
		}
	}
}
