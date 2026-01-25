using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BestHTTP;
using UnityEngine;

public class NekkiHelperWWW : MonoBehaviour
{
	public class Response : IDisposable
	{
		public string Url { get; private set; }

		public string text { get; private set; }

		public AssetBundle assetBundle { get; private set; }

		public byte[] bytes { get; private set; }

		public Response(HTTPRequest source)
		{
			Url = source.Uri.ToString();
			text = source.Response.DataAsText;
			bytes = source.Response.Data;
		}

		public IEnumerator YieldBundle()
		{
			if (bytes != null && bytes.Length != 0)
			{
				AssetBundleCreateRequest operation = AssetBundle.LoadFromMemoryAsync(bytes);
				yield return operation;
				assetBundle = operation.assetBundle;
			}
		}

		public void Dispose()
		{
			if ((bool)assetBundle)
			{
				assetBundle.Unload(true);
				assetBundle = null;
			}
			bytes = null;
			Url = null;
			text = null;
		}
	}

	protected static NekkiHelperWWW _instance = null;

	private static List<WWW> wwws = new List<WWW>();

	private static List<HTTPRequest> requests = new List<HTTPRequest>();

	public static float Progress { get; private set; }

	public static string Urls { get; private set; }

	public static NekkiHelperWWW Instance
	{
		get
		{
			if (_instance == null)
			{
				UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(NekkiHelperWWW));
				if (@object == null)
				{
					_instance = new GameObject(typeof(NekkiHelperWWW).ToString()).AddComponent<NekkiHelperWWW>();
				}
				else
				{
					_instance = @object as NekkiHelperWWW;
				}
				if (_instance == null)
				{
					AdvLog.LogError(string.Concat("An instance of ", typeof(NekkiHelperWWW), " is not found or cant created !!!"));
				}
			}
			return _instance;
		}
	}

	private void Update()
	{
		float num = 0f;
		for (int i = 0; i < wwws.Count; i++)
		{
			try
			{
				num = ((wwws[i] != null) ? (num + wwws[i].progress / (float)wwws.Count) : (num + 1f / (float)wwws.Count));
			}
			catch (Exception)
			{
				num += 1f / (float)wwws.Count;
			}
		}
		Progress = num * 100f;
		CompileUrls();
	}

	public static void Delete()
	{
		_instance = null;
	}

	public void ExecWWW(object url, Action<object, Response> action)
	{
		Time.timeScale = 1f;
		StartCoroutine(DownloadAndAct(url, action));
	}

	private IEnumerator DownloadAndAct(object urlObj, Action<object, Response> action)
	{
		BundleConfig.Line line = urlObj as BundleConfig.Line;
		string str = urlObj as string;
		string url = ((line != null) ? line.BinarryURL : str);
		if (url == null)
		{
			yield break;
		}
		HTTPRequest request = new HTTPRequest(new Uri(url)).Send();
		requests.Add(request);
		CompileUrls();
		AdvLog.Log(string.Format("NekkiHelperWWW.DownloadAndAct('{0}');", url));
		while (request.State < HTTPRequestStates.Finished)
		{
			yield return new WaitForSeconds(0.1f);
		}
		switch (request.State)
		{
		case HTTPRequestStates.Finished:
			if (request.Response.IsSuccess)
			{
				Response response = new Response(request);
				action(urlObj, response);
			}
			else
			{
				AdvLog.LogError(string.Format("got error from server after downloaded [{0}]", url));
				action(urlObj, null);
			}
			break;
		case HTTPRequestStates.Error:
		case HTTPRequestStates.Aborted:
		case HTTPRequestStates.ConnectionTimedOut:
		case HTTPRequestStates.TimedOut:
			AdvLog.LogError(string.Format("got error from server during downloading [{0}]", url));
			action(urlObj, null);
			break;
		}
		int index = requests.IndexOf(request);
		requests[index] = null;
		request.Abort();
		request = null;
		requests.Remove(null);
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}

	public void ExecWWW(object url, Action<object, WWW> action, bool cache = false)
	{
		if (cache)
		{
			StartCoroutine(DownloadAndCache(url, action));
		}
		else
		{
			StartCoroutine(DownloadAndAct(url, action));
		}
	}

	private IEnumerator DownloadAndAct(object url, Action<object, WWW> action)
	{
		BundleConfig.Line line = url as BundleConfig.Line;
		string str = url as string;
		if (line != null)
		{
			using (WWW www = new WWW(line.ConfigURL))
			{
				wwws.Add(www);
				CompileUrls();
				AdvLog.Log(string.Format("NekkiHelperWWW.DownloadAndAct Config('{0}');", line.ConfigURL));
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					AdvLog.LogError(www.error + www.url);
				}
				else
				{
					Directory.CreateDirectory(GlobalPaths.BundlesPath);
					string savePath = string.Format("{0}/{1}", GlobalPaths.BundlesPath, Path.GetFileName(line.ConfigURL.Split('?')[0]));
					string confPath = Path.ChangeExtension(savePath, "config");
					File.WriteAllText(confPath, www.text);
				}
				int i = wwws.IndexOf(www);
				if (i >= 0)
				{
					wwws[i] = null;
				}
				CompileUrls();
			}
		}
		using (WWW www2 = new WWW((line != null) ? line.BinarryURL : str))
		{
			wwws.Add(www2);
			CompileUrls();
			AdvLog.Log(string.Format("NekkiHelperWWW.DownloadAndAct('{0}');", (!(url is BundleConfig.Line)) ? url.ToString() : ((BundleConfig.Line)url).URL));
			yield return www2;
			if (www2.error != null)
			{
				AdvLog.LogWarning(string.Format("<ERROR>:{0}-{1}", url, www2.error));
				action(url, null);
			}
			else
			{
				action(url, www2);
			}
			int i2 = wwws.IndexOf(www2);
			if (i2 >= 0)
			{
				wwws[i2] = null;
			}
			CompileUrls();
		}
	}

	private IEnumerator DownloadAndCache(object url, Action<object, WWW> action)
	{
#if !UNITY_WEBGL

		while (!Caching.ready)
		{
			yield return null;
		}
#endif
		BundleConfig.Line line = url as BundleConfig.Line;
		string str = url as string;
		using (WWW www = WWW.LoadFromCacheOrDownload((line != null) ? line.BinarryURL : str, 0))
		{
			wwws.Add(www);
			CompileUrls();
			AdvLog.Log(string.Format("NekkiHelperWWW.DownloadAndCache('{0}');", url));
			yield return www;
			if (www.error != null)
			{
				AdvLog.LogWarning(string.Format("<ERROR>:{0}-{1}", url, www.error));
				action(url, null);
			}
			else
			{
				action(url, www);
			}
			int i = wwws.IndexOf(www);
			if (i >= 0)
			{
				wwws[i] = null;
			}
			CompileUrls();
		}
	}

	private static void CompileUrls()
	{
		Urls = string.Empty;
		foreach (WWW www in wwws)
		{
			if (www != null)
			{
				Urls += string.Format("<color=grey>[</color><color=yellow>{0}%</color><color=grey>]</color> {1}\n", (www.progress * 100f).ToString("0.00"), www.url);
			}
		}
	}
}
