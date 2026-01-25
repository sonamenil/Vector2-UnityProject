using UnityEngine;

public static class AndroidLocalNotification
{
	public enum NotificationExecuteMode
	{
		Inexact = 0,
		Exact = 1,
		ExactAndAllowWhileIdle = 2
	}

	private static string AndroidClassName
	{
		get
		{
			return Application.identifier + ".LocalNotificationManager";
		}
	}

	private static string AndroidMainActivityClassName
	{
		get
		{
			return Application.identifier + ".VectorActivity";
		}
	}

	public static void CreateNotification(int p_id, string p_title, string p_message, long p_delayInSeconds)
	{
		SendNotification(p_id, p_delayInSeconds, p_title, p_message, Color.black, true, true, true, string.Empty);
	}

	private static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(AndroidClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetNotification", id, delay * 1000, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, "app_icon", "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, AndroidMainActivityClassName);
		}
	}

	public static void CancelNotification(int id)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(AndroidClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("CancelNotification", id);
		}
	}
}
