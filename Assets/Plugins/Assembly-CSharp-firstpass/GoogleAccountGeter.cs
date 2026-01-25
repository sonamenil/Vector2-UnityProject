using UnityEngine;

public static class GoogleAccountGeter
{
	public static string getUserEmail()
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("com/nekki/GoogleAccountGeter");
		return androidJavaObject.Call<string>("GetEmail", new object[0]);
	}
}
