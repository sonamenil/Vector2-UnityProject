using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace Nekki.Vector.Core
{
	public static class InternetUtils
	{
		private enum Internet
		{
			UNDEFINED = 0,
			AVAILABLE = 1,
			REDIRECTED = 2,
			NOT_AVAILABLE = 3
		}

		public class DownloadFileResult
		{
			private WWW _WWW;

			public string Url
			{
				get
				{
					return _WWW.url;
				}
			}

			public byte[] Data
			{
				get
				{
					return _WWW.bytes;
				}
			}

			public string ErrorMsg
			{
				get
				{
					return _WWW.error;
				}
			}

			public bool IsError
			{
				get
				{
					return !string.IsNullOrEmpty(ErrorMsg) || Data == null;
				}
			}

			public bool IsNotFound
			{
				get
				{
					if (!IsError)
					{
						return false;
					}
					string text = ErrorMsg.ToLower();
					return text.Contains("404") || text.Contains("not found");
				}
			}

			public bool IsAssetBundle
			{
				get
				{
					bool result = false;
					try
					{
						UnityEngine.AssetBundle assetBundle = _WWW.assetBundle;
						if (assetBundle != null)
						{
							result = true;
							assetBundle.Unload(true);
						}
					}
					catch
					{
					}
					return result;
				}
			}

			public DownloadFileResult(WWW p_www)
			{
				_WWW = p_www;
			}
		}

		private const int _RequestTimeout = 5000;

		private static Internet _InternetState;

		public static bool IsInternetAvailable
		{
			get
			{
				if (_InternetState != 0)
				{
					return _InternetState == Internet.AVAILABLE;
				}
				return CheckInternetConnection();
			}
		}

		public static bool CheckInternetConnection()
		{
			string text = string.Empty;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://google.com");
			httpWebRequest.Timeout = 5000;
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					if (httpWebResponse.StatusCode < (HttpStatusCode)299 && httpWebResponse.StatusCode >= HttpStatusCode.OK)
					{
						using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
						{
							char[] array = new char[80];
							streamReader.Read(array, 0, array.Length);
							char[] array2 = array;
							foreach (char c in array2)
							{
								text += c;
							}
						}
					}
				}
			}
			catch
			{
				text = string.Empty;
			}
			if (string.IsNullOrEmpty(text))
			{
				_InternetState = Internet.NOT_AVAILABLE;
				Debug.Log("[ServerProvider]: internet.NOT_AVAILABLE");
			}
			else if (!text.Contains("schema.org/WebPage"))
			{
				_InternetState = Internet.REDIRECTED;
				Debug.Log("[ServerProvider]: internet.REDIRECTED");
			}
			else
			{
				_InternetState = Internet.AVAILABLE;
				Debug.Log("[ServerProvider]: internet.AVAILABLE");
			}
			return _InternetState == Internet.AVAILABLE;
		}

		public static DownloadFileResult DownloadFile(string p_url)
		{
			WWW wWW = new WWW(p_url);
			while (!wWW.isDone && Application.internetReachability != 0)
			{
			}
			return new DownloadFileResult(wWW);
		}

		public static long GetContentLength(string p_url)
		{
			UnityWebRequest unityWebRequest = UnityWebRequest.Head(p_url);
			unityWebRequest.Send();
			while (!unityWebRequest.isDone && Application.internetReachability != 0)
			{
			}
			try
			{
				Dictionary<string, string> responseHeaders = unityWebRequest.GetResponseHeaders();
				return long.Parse(responseHeaders["Content-Length"]);
			}
			catch (Exception ex)
			{
				DebugUtils.Log("[InternetUtils]: GetContentLength error - " + ex.Message);
				return 0L;
			}
		}
	}
}
