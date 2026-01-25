namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_Key : TriggerRunnerEvent
	{
		public string Key;

		public TRE_Key(string p_value = "")
		{
			Key = p_value;
			_Type = EventType.TRE_KEY;
		}
	}
}
