using System.Collections;
using System.Text;
using UnityEngine;

public class SendMeLog : MonoBehaviour
{
	private static string _email;

	private static bool _active;

	private static GameObject _obj;

	private static readonly StringBuilder Log = new StringBuilder();

	public static void Init(string email, float seconds = 0f)
	{
		_email = email;
		if (!_obj)
		{
			Application.logMessageReceived += Application_logMessageReceived;
			_active = true;
			_obj = new GameObject("_sendMeLog", typeof(SendMeLog));
			Object.DontDestroyOnLoad(_obj);
		}
		if (seconds > 0.1f)
		{
			SendMeLog component = _obj.GetComponent<SendMeLog>();
			component.StartCoroutine(component.SendRoutene(seconds));
		}
	}

	private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (_active)
		{
			Log.Append(string.Format("[{0}] {1}{2}\n", type, condition, (!string.IsNullOrEmpty(stackTrace)) ? string.Format(" ({0})", stackTrace) : string.Empty));
		}
	}

	public static void Stop()
	{
		Send();
		_active = false;
	}

	private IEnumerator SendRoutene(float time)
	{
		yield return new WaitForSeconds(time);
		Stop();
	}

	private static void Send()
	{
		if (_active)
		{
			string text = WWW.EscapeURL(string.Format("Log from [{0}:{1}:{2}] {3}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier)).Replace("+", "%20");
			string text2 = WWW.EscapeURL(Log.ToString()).Replace("+", "%20");
			string url = "mailto:" + _email + "?subject=" + text + "&body=" + text2;
			Application.OpenURL(url);
		}
	}
}
