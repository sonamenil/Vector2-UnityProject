using System;
using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.AssetBundle
{
	public static class BundleUpdater
	{
		public class BundleData
		{
			public string Id;

			public int Version;

			public BundleData(XmlNode p_node)
			{
				Id = XmlUtils.ParseString(p_node.Attributes["Id"], string.Empty);
				Version = XmlUtils.ParseInt(p_node.Attributes["Version"]);
			}
		}

		private const int _MaxDownloadAttempt = 2;

		private static Dictionary<string, BundleData> _Bundles = new Dictionary<string, BundleData>();

		private static bool _IsInited;

		public static string BundlesListUrl
		{
			get
			{
				return URLCreator.Make("bundles/bundles_list.bin");
			}
		}

		public static bool IsInited
		{
			get
			{
				return _IsInited;
			}
		}

		public static string BundleURL(string p_bundleId, int p_version)
		{
			return URLCreator.Make(string.Format("bundles/{0}_{1}.bin", p_bundleId, p_version));
		}

		public static void LoadBundlesListFile(int p_attempt = 0)
		{
			if (p_attempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(BundlesListUrl);
			if (downloadFileResult.IsError)
			{
				LoadBundlesListFile(p_attempt + 1);
				return;
			}
			byte[] p_bytes = AESUtils.DecryptBytes(downloadFileResult.Data, BundleManager.Key, BundleManager.IV);
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
			if (xmlDocument == null)
			{
				LoadBundlesListFile(p_attempt + 1);
			}
			else
			{
				ParseBundlesListFile(xmlDocument);
			}
		}

		private static void OnLoadFail()
		{
			DebugUtils.Log("[BundleUpdater]: OnLoadFail");
			Preloader.ServerResponse(false, false, false, true);
		}

		private static void ParseBundlesListFile(XmlDocument p_doc)
		{
			XmlNode xmlNode = p_doc["Bundles"];
			if (xmlNode == null)
			{
				OnLoadFail();
				return;
			}
			_Bundles.Clear();
			_IsInited = true;
			foreach (XmlNode item in xmlNode)
			{
				BundleData bundleData = new BundleData(item);
				_Bundles.Add(bundleData.Id, bundleData);
			}
			Preloader.ServerResponse(false, false, false, true);
		}

		private static BundleData GetBundleData(string p_bundleId)
		{
			BundleData value = null;
			_Bundles.TryGetValue(p_bundleId, out value);
			return value;
		}

		public static int CheckBundleUpdate(BundleRef p_bundle)
		{
			BundleData bundleData = GetBundleData(p_bundle.BundleId);
			if (bundleData == null)
			{
				return -1;
			}
			if (p_bundle.Version < bundleData.Version)
			{
				return bundleData.Version;
			}
			return 0;
		}

		public static int GetActualBundleVersion(string p_bundleId, int p_version)
		{
			if (!_IsInited)
			{
				return p_version;
			}
			BundleData bundleData = GetBundleData(p_bundleId);
			if (bundleData == null)
			{
				return -1;
			}
			return Math.Max(p_version, bundleData.Version);
		}
	}
}
