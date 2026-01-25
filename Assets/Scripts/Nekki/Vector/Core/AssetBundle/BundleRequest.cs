using System.Collections;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.AssetBundle
{
	public class BundleRequest
	{
		private const int _ContentLengthRequestAttemptLimit = 1;

		private const int _DownloadAttemptLimit = 1;

		private static bool _IsRequestsIsRunning;

		private string _BundleId;

		private int _Version;

		private bool _IsRequired;

		private long _ContentLength;

		private byte[] _Content;

		public static bool IsRequesrsIsRunning
		{
			get
			{
				return _IsRequestsIsRunning;
			}
		}

		public string BundleId
		{
			get
			{
				return _BundleId;
			}
		}

		public int Version
		{
			get
			{
				return _Version;
			}
		}

		public bool IsRequired
		{
			get
			{
				return _IsRequired;
			}
		}

		public long ContentLength
		{
			get
			{
				return _ContentLength;
			}
		}

		public byte[] Content
		{
			get
			{
				return _Content;
			}
		}

		public string ContentHash
		{
			get
			{
				return MD5Utils.MD5HashBytes(_Content, BundleManager.Salt);
			}
		}

		public string Url
		{
			get
			{
				return BundleUpdater.BundleURL(_BundleId, _Version);
			}
		}

		public static BundleRequest CreateInProgress(string p_bundleId, int p_version, bool p_isRequired)
		{
			BundleRequest bundleRequest = new BundleRequest();
			bundleRequest._BundleId = p_bundleId;
			bundleRequest._Version = p_version;
			bundleRequest._IsRequired = p_isRequired;
			bundleRequest.UpdateContentLength();
			return bundleRequest;
		}

		public static BundleRequest CreateInProgress(XmlNode p_node)
		{
			BundleRequest bundleRequest = new BundleRequest();
			bundleRequest._BundleId = XmlUtils.ParseString(p_node.Attributes["Id"], string.Empty);
			bundleRequest._Version = XmlUtils.ParseInt(p_node.Attributes["Version"]);
			bundleRequest._IsRequired = XmlUtils.ParseBool(p_node.Attributes["Required"]);
			bundleRequest._ContentLength = XmlUtils.ParseLong(p_node.Attributes["ContentLength"]);
			return bundleRequest;
		}

		public void Save(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Request");
			xmlElement.SetAttribute("Id", _BundleId);
			xmlElement.SetAttribute("Version", _Version.ToString());
			if (_IsRequired)
			{
				xmlElement.SetAttribute("Required", "1");
			}
			if (_ContentLength > 0)
			{
				xmlElement.SetAttribute("ContentLength", _ContentLength.ToString());
			}
			p_node.AppendChild(xmlElement);
		}

		public void Run()
		{
			ApplicationController.Current.StartCoroutine(DownloadFile(Url));
		}

		public bool UpdateContentLength()
		{
			if (_ContentLength > 0)
			{
				return false;
			}
			for (int i = 0; i < 1; i++)
			{
				_ContentLength = InternetUtils.GetContentLength(Url);
				if (_ContentLength > 0)
				{
					return true;
				}
			}
			return false;
		}

		private IEnumerator DownloadFile(string p_url)
		{
			_IsRequestsIsRunning = true;
			BundleManager.OnBundleDownloadStarted(_BundleId);
			DebugUtils.StartTimer("BundleRequest_" + _BundleId);
			InternetUtils.DownloadFileResult downloadResult = null;
			int attempt = 0;
			for (attempt = 0; attempt < 1; attempt++)
			{
				WWW www = new WWW(p_url);
				float lastProgress = 0f;
				while (!www.isDone && Application.internetReachability != 0)
				{
					if (lastProgress != www.progress)
					{
						lastProgress = www.progress;
						BundleManager.OnBundleDownloadProgress(_BundleId, lastProgress);
					}
					yield return null;
				}
				downloadResult = new InternetUtils.DownloadFileResult(www);
				if (www.isDone && ((!downloadResult.IsError && downloadResult.IsAssetBundle) || downloadResult.IsNotFound))
				{
					break;
				}
			}
			bool success = attempt < 1;
			if (success)
			{
				_Content = ((!downloadResult.IsNotFound) ? downloadResult.Data : null);
				BundleManager.CompleteRequest(this);
			}
			else
			{
				DebugUtils.LogFormat("[BundleRequest]: can't load bundle [{0}-{1}]! Try again later", _BundleId, _Version);
			}
			_IsRequestsIsRunning = false;
			DebugUtils.StopTimerWithMessage("BundleRequest.DownloadFile - " + _BundleId, "BundleRequest_" + _BundleId);
			BundleManager.OnBundleDownloadFinished(_BundleId, success);
		}
	}
}
