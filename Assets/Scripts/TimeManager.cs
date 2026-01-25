using System;

public static class TimeManager
{
	private static long _TimeDelta = 1262304000000L;

	public static long UTCTime
	{
		get
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
		}
	}

	public static TimeZone CurrentTimeZone
	{
		get
		{
			return TimeZone.CurrentTimeZone;
		}
	}

	public static TimeSpan CurrentTimeZoneOffset
	{
		get
		{
			return CurrentTimeZone.GetUtcOffset(DateTime.Now);
		}
	}

	public static int ConvertMsToInt(long p_time)
	{
		return (int)((double)(p_time - _TimeDelta) * 0.001);
	}

	public static long ConvertIntToMs(int p_time)
	{
		return (long)p_time * 1000L + _TimeDelta;
	}
}
