using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Game;
using Nekki.Vector.GUI.Common;
using UnityEngine;

public static class DebugUtils
{
	private static Dictionary<string, double> _TimeStart = new Dictionary<string, double>();

	private static double _SceneLoadTime;

	private static long _Memory;

	public static double SceneLoadTime
	{
		get
		{
			return _SceneLoadTime;
		}
	}

	public static double GetMS()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		return (DateTime.UtcNow - dateTime).TotalMilliseconds;
	}

	public static void StartTimer(string p_id = "")
	{
		if (_TimeStart.ContainsKey(p_id))
		{
			_TimeStart[p_id] = GetMS();
		}
		else
		{
			_TimeStart.Add(p_id, GetMS());
		}
	}

	public static double TimerElapsed(string p_id = "")
	{
		double value;
		_TimeStart.TryGetValue(p_id, out value);
		return GetMS() - value;
	}

	public static double StopTimer(string p_id = "")
	{
		double num = TimerElapsed(p_id);
		Debug.Log("Time: " + num);
		return num;
	}

	public static void StopAndStartTimer(string p_msg, string p_id = "")
	{
		StopTimerWithMessage(p_msg, p_id);
		StartTimer(p_id);
	}

	public static double StopTimerWithMessage(string p_msg, string p_id = "")
	{
		double num = TimerElapsed(p_id);
		Debug.Log("Time(" + p_msg + "): " + num);
		return num;
	}

	public static void SceneLoadStartTimer()
	{
		StartTimer("SceneLoadTime");
	}

	public static void SceneLoadStopTimer()
	{
		_SceneLoadTime = TimerElapsed("SceneLoadTime");
	}

	public static void StartMem()
	{
		GC.Collect();
		_Memory = GC.GetTotalMemory(true);
	}

	public static void StopMem(string p_mess = null)
	{
		GC.Collect();
		Debug.Log("Mem" + ((p_mess == null) ? " :" : ("(" + p_mess + ") :")) + (float)(GC.GetTotalMemory(true) - _Memory) / 1048576f);
	}

	public static void Log(string p_value)
	{
		Debug.Log(p_value);
	}

	public static void LogFormat(string p_format, params object[] p_args)
	{
		Debug.LogFormat(p_format, p_args);
	}

	public static void LogError(string p_value)
	{
		Debug.LogError(p_value);
	}

	public static void LogErrorFormat(string p_value, params object[] p_args)
	{
		Debug.LogErrorFormat(p_value, p_args);
	}

	public static void Dialog(string p_message, bool p_isClose)
	{
		if (p_isClose)
		{
			Debug.LogError("DIALOG(FATAL): " + p_message);
		}
		else
		{
			Debug.Log("DIALOG: " + p_message);
		}
		if (!Settings.IsReleaseBuild)
		{
			SysDlg.Show(p_message, p_isClose);
		}
	}

	public static void LogToConsole(string p_value, bool p_logToUnity = false)
	{
		if (!Settings.IsReleaseBuild)
		{
			ConsoleUI.Log(p_value);
			if (p_logToUnity)
			{
				Debug.Log(p_value);
			}
		}
	}
}
