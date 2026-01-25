using UnityEngine;

public class WaitForTime : CustomYieldInstruction
{
	public float Timeout { get; private set; }

	public override bool keepWaiting
	{
		get
		{
			if (!CoroutineManager.Current.IsPaused)
			{
				Timeout -= Time.deltaTime;
			}
			return Timeout >= 1E-07f;
		}
	}

	public WaitForTime(float p_timeout)
	{
		Timeout = p_timeout;
	}
}
