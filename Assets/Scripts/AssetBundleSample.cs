using System;
using System.Collections;
using BestHTTP;
using UnityEngine;

public sealed class AssetBundleSample : MonoBehaviour
{
	private const string URL = "http://besthttp.azurewebsites.net/Content/AssetBundle.html";

	private string status = "Waiting for user interaction";

	private AssetBundle cachedBundle;

	private Texture2D texture;

	private bool downloading;

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.Label("Status: " + status);
			if (texture != null)
			{
				GUILayout.Box(texture, GUILayout.MaxHeight(256f));
			}
			if (!downloading && GUILayout.Button("Start Download"))
			{
				UnloadBundle();
				StartCoroutine(DownloadAssetBundle());
			}
		});
	}

	private void OnDestroy()
	{
		UnloadBundle();
	}

	private IEnumerator DownloadAssetBundle()
	{
		downloading = true;
		HTTPRequest request = new HTTPRequest(new Uri("http://besthttp.azurewebsites.net/Content/AssetBundle.html")).Send();
		status = "Download started";
		while (request.State < HTTPRequestStates.Finished)
		{
			yield return new WaitForSeconds(0.1f);
			status += ".";
		}
		switch (request.State)
		{
		case HTTPRequestStates.Finished:
			if (request.Response.IsSuccess)
			{
				status = string.Format("AssetBundle downloaded! Loaded from local cache: {0}", request.Response.IsFromCache.ToString());
				AssetBundleCreateRequest async = AssetBundle.LoadFromMemoryAsync(request.Response.Data);
				yield return async;
				yield return StartCoroutine(ProcessAssetBundle(async.assetBundle));
			}
			else
			{
				status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", request.Response.StatusCode, request.Response.Message, request.Response.DataAsText);
				AdvLog.LogWarning(status);
			}
			break;
		case HTTPRequestStates.Error:
			status = "Request Finished with Error! " + ((request.Exception == null) ? "No Exception" : (request.Exception.Message + "\n" + request.Exception.StackTrace));
			AdvLog.LogError(status);
			break;
		case HTTPRequestStates.Aborted:
			status = "Request Aborted!";
			AdvLog.LogWarning(status);
			break;
		case HTTPRequestStates.ConnectionTimedOut:
			status = "Connection Timed Out!";
			AdvLog.LogError(status);
			break;
		case HTTPRequestStates.TimedOut:
			status = "Processing the request Timed Out!";
			AdvLog.LogError(status);
			break;
		}
		downloading = false;
	}

	private IEnumerator ProcessAssetBundle(AssetBundle bundle)
	{
		if (!(bundle == null))
		{
			cachedBundle = bundle;
			AssetBundleRequest asyncAsset = cachedBundle.LoadAssetAsync("9443182_orig", typeof(Texture2D));
			yield return asyncAsset;
			texture = asyncAsset.asset as Texture2D;
		}
	}

	private void UnloadBundle()
	{
		if (cachedBundle != null)
		{
			cachedBundle.Unload(true);
			cachedBundle = null;
		}
	}
}
