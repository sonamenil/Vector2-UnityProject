using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class ProvisionOnDroid : MonoBehaviour
{
	private static string _andId;

	private static bool _idReceived;

	private Action _onValid;

	private Action _onFail;

	public static string AndroidID
	{
		get
		{
			if (!_idReceived)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
				_andId = androidJavaClass2.CallStatic<string>("getString", new object[2] { androidJavaObject, "android_id" });
				_idReceived = true;
			}
			return _andId;
		}
	}

	public static void Check(Action onValid, Action onFail)
	{
		ProvisionOnDroid provisionOnDroid = new GameObject("_androidIdValidator").AddComponent<ProvisionOnDroid>();
		provisionOnDroid._onValid = onValid;
		provisionOnDroid._onFail = onFail;
	}

	internal void Start()
	{
		StartCoroutine(CheckAndroid());
	}

	private IEnumerator CheckAndroid()
	{
		yield return new WaitForEndOfFrame();
		WWW www = new WWW(NekkiAssetDownloader.Timestamp(GlobalPaths.GetPath("AndroidIDs").Raw));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(www.error);
			_onFail();
			yield break;
		}
		try
		{
			string str = Crypto.DecryptStringAES(Encoding.UTF8.GetString(www.bytes), "MntEr453Sd'xzz$#2xxccAs';l]Qesxbv782as$3xazaD;puvcvcDQaszSDzfE4f");
			string id = AndroidID;
			if (string.IsNullOrEmpty(id))
			{
				_onFail();
			}
			else if (str.Contains(AndroidID))
			{
				_onValid();
			}
			else
			{
				_onFail();
			}
		}
		catch (Exception ex)
		{
			_onFail();
			Debug.Log(ex.Message);
		}
	}
}
