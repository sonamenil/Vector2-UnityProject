using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
	private static DialogManager _instance;

	private Dictionary<int, Action<bool>> _delegates;

	public static DialogManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("DialogManager").AddComponent<DialogManager>();
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	private void Awake()
	{
		_delegates = new Dictionary<int, Action<bool>>();
		SetLabel("YES", "NO", "CLOSE");
	}

	private void OnDestroy()
	{
		if (_delegates != null)
		{
			_delegates.Clear();
			_delegates = null;
		}
	}

	public int ShowSelectDialog(string msg, Action<bool> del)
	{
		int num = 0;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("unity.plugins.dialog.DialogManager"))
		{
			num = androidJavaClass.CallStatic<int>("ShowSelectDialog", new object[1] { msg });
			_delegates.Add(num, del);
			return num;
		}
	}

	public int ShowSelectDialog(string title, string msg, Action<bool> del)
	{
		int num = 0;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("unity.plugins.dialog.DialogManager"))
		{
			num = androidJavaClass.CallStatic<int>("ShowSelectTitleDialog", new object[2] { title, msg });
			_delegates.Add(num, del);
			return num;
		}
	}

	public int ShowSubmitDialog(string msg, Action<bool> del)
	{
		int num = 0;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("unity.plugins.dialog.DialogManager"))
		{
			num = androidJavaClass.CallStatic<int>("ShowSubmitDialog", new object[1] { msg });
			_delegates.Add(num, del);
			return num;
		}
	}

	public int ShowSubmitDialog(string title, string msg, Action<bool> del)
	{
		int num = 0;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("unity.plugins.dialog.DialogManager"))
		{
			num = androidJavaClass.CallStatic<int>("ShowSubmitTitleDialog", new object[2] { title, msg });
			_delegates.Add(num, del);
			return num;
		}
	}

	public void DissmissDialog(int id)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("unity.plugins.dialog.DialogManager"))
		{
			androidJavaClass.CallStatic("DissmissDialog", id);
		}
		if (_delegates.ContainsKey(id))
		{
			_delegates[id](false);
			_delegates.Remove(id);
		}
		else
		{
			Debug.LogWarning("undefined id:" + id);
		}
	}

	public void SetLabel(string decide, string cancel, string close)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("unity.plugins.dialog.DialogManager"))
		{
			androidJavaClass.CallStatic("SetLabel", decide, cancel, close);
		}
	}

	public void OnSubmit(string idStr)
	{
		int key = int.Parse(idStr);
		if (_delegates.ContainsKey(key))
		{
			_delegates[key](true);
			_delegates.Remove(key);
		}
		else
		{
			Debug.LogWarning("undefined id:" + idStr);
		}
	}

	public void OnCancel(string idStr)
	{
		int key = int.Parse(idStr);
		if (_delegates.ContainsKey(key))
		{
			_delegates[key](false);
			_delegates.Remove(key);
		}
		else
		{
			Debug.LogWarning("undefined id:" + idStr);
		}
	}
}
