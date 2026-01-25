using UnityEngine.Events;

public static class UnityEventBaseExtension
{
	public static void SetPersistentAllListenersState(this UnityEventBase p_event, UnityEventCallState p_state)
	{
		int persistentEventCount = p_event.GetPersistentEventCount();
		if (persistentEventCount > 0)
		{
			for (int i = 0; i < persistentEventCount; i++)
			{
				p_event.SetPersistentListenerState(i, p_state);
			}
		}
	}
}
