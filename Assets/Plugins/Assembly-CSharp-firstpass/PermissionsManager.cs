using System;
using UnityEngine;

public class PermissionsManager : MonoBehaviour
{
	public Action<string> OnUserSkipCallBack;

	public Action<string> OnGrantedCallBack;

	public Action<string> OnDeniedCallBack;

	private static PermissionsManager _Current;

	private static string ClassName
	{
		get
		{
			return null;
		}
	}

	public static PermissionsManager Current
	{
		get
		{
			if (_Current == null)
			{
				_Current = new GameObject("PermissionsManager").AddComponent<PermissionsManager>();
				UnityEngine.Object.DontDestroyOnLoad(_Current.gameObject);
			}
			return _Current;
		}
	}

	public bool CheckPermission(string p_permission)
	{
		return true;
	}

	public bool IsShouldShowRequestPermissionRationale(string p_permission)
	{
		return false;
	}

	public void RequestPermission(string p_permission, string p_title, string p_text, string p_ok, string p_cancel, string p_settings)
	{

	}

	public void ShowExplanationWithOpenSettings(string p_permission, string p_title, string p_text, string p_settings)
	{

	}

	public void OnUserSkip(string p_permission)
	{

	}

	public void OnGranted(string p_permission)
	{

	}

	public void OnDenied(string p_permission)
	{

	}
}
