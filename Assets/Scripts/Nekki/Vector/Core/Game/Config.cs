using System.IO;
using System.Xml;
using Nekki.Vector.Core.Advertising;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.Payment;
using UnityEngine;

namespace Nekki.Vector.Core.Game
{
	public static class Config
	{
		private const string _Filename = "config.xml";

		private const string _OldFilename = "сonfig.xml";

		private static readonly byte[] _Key = new byte[32]
		{
			147, 14, 105, 144, 49, 134, 56, 233, 21, 23,
			180, 181, 251, 48, 85, 137, 150, 5, 215, 109,
			114, 46, 228, 2, 141, 143, 252, 82, 123, 174,
			177, 196
		};

		private static readonly byte[] _IV = new byte[16]
		{
			154, 142, 224, 8, 5, 43, 199, 146, 24, 235,
			141, 141, 172, 68, 15, 75
		};

		private static string URL
		{
			get
			{
				return URLCreator.Make("config.bin");
			}
		}

		public static string Path
		{
			get
			{
				return VectorPaths.ConfigExternal + "/config.xml";
			}
		}

		public static string OldPath
		{
			get
			{
				return VectorPaths.ConfigExternal + "/сonfig.xml";
			}
		}

		public static void Init(bool p_downloadUpdate = true)
		{
			if (p_downloadUpdate)
			{
				ServerProvider.Instance.DownloadFile(URL, OnDownloadComplete);
			}
			if (File.Exists(OldPath))
			{
				File.Delete(OldPath);
			}
			if (!File.Exists(Path))
			{
				Debug.LogWarning("[Config]: config file don't exists!");
				return;
			}
			byte[] p_bytes = AESUtils.DecryptFileToBytes(_Key, _IV, Path, true);
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
			if (xmlDocument == null)
			{
				Debug.LogWarning("[Config]: failed to load config!");
				File.Delete(Path);
			}
			else
			{
				LoadConfig(xmlDocument["Config"]);
			}
		}

		private static void LoadConfig(XmlNode p_node)
		{
			XmlNode xmlNode = p_node["InAppsVerificationEnabled"];
			PaymentVerificationManager.SetActive(XmlUtils.ParseBool(xmlNode.Attributes["iOS"], true), XmlUtils.ParseBool(xmlNode.Attributes["Android"], true));
			xmlNode = p_node["DataValidationEnabled"];
			ValidatorSettings.SetEnabled(XmlUtils.ParseBool(xmlNode.Attributes["iOS"], true), XmlUtils.ParseBool(xmlNode.Attributes["Android"], true));
			ADProbabilityManager.Init(p_node["Advertising"]);
		}

		private static void OnDownloadComplete(byte[] p_bytes, string p_error)
		{
			if (!string.IsNullOrEmpty(p_error))
			{
				Debug.LogWarning("[Config]: failed to download config because " + p_error);
				return;
			}
			File.WriteAllBytes(Path, p_bytes);
			Init(false);
		}
	}
}
