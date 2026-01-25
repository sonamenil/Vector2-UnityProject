using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.DataValidation;

namespace Nekki.Vector.Core.ContentUpdater
{
	public static class ContentUpdater
	{
		private const int _MaxDownloadAttempt = 2;

		public static readonly byte[] Key = new byte[32]
		{
			186, 105, 191, 252, 192, 169, 253, 179, 235, 196,
			45, 183, 152, 234, 132, 194, 247, 58, 67, 166,
			213, 251, 56, 7, 103, 154, 119, 242, 174, 247,
			50, 179
		};

		public static readonly byte[] IV = new byte[16]
		{
			94, 205, 45, 37, 160, 207, 211, 149, 51, 233,
			100, 176, 128, 99, 179, 203
		};

		private static SortedDictionary<int, string> _VersionAndHash;

		private static string URL_ConfigFile
		{
			get
			{
				return URLCreator.Make("updates/update_config.bin");
			}
		}

		public static bool IsUpdateAvalible
		{
			get
			{
				return _VersionAndHash != null;
			}
		}

		private static string URL_UpdateContent(int p_version)
		{
			return URLCreator.Make(string.Format("updates/update_{0}.bin", p_version));
		}

		public static void LoadConfigFile(int p_attempt = 0)
		{
			if (p_attempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(URL_ConfigFile);
			if (downloadFileResult.IsError)
			{
				LoadConfigFile(p_attempt + 1);
				return;
			}
			byte[] p_bytes = AESUtils.DecryptBytes(downloadFileResult.Data, Key, IV);
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
			if (xmlDocument == null)
			{
				LoadConfigFile(p_attempt + 1);
			}
			else
			{
				ParseConfigFile(xmlDocument);
			}
		}

		private static void OnLoadFail()
		{
			Preloader.ServerResponse(false, false, true, false);
		}

		private static void ParseConfigFile(XmlDocument p_doc)
		{
			XmlNode xmlNode = p_doc["Updates"];
			if (xmlNode == null)
			{
				OnLoadFail();
				return;
			}
			_VersionAndHash = new SortedDictionary<int, string>();
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				int num = XmlUtils.ParseInt(childNode.Attributes["V"], -1);
				string text = XmlUtils.ParseString(childNode.Attributes["Hash"]);
				if (num != -1 && text != null && num > ResourcesValidator.GamedataVersion)
				{
					_VersionAndHash.Add(num, text);
				}
			}
			if (_VersionAndHash.Count == 0)
			{
				_VersionAndHash = null;
			}
			Preloader.ServerResponse(false, false, true, false);
		}

		public static void RunUpdate()
		{
			if (_VersionAndHash == null)
			{
				return;
			}
			foreach (KeyValuePair<int, string> item in _VersionAndHash)
			{
				LoadUpdateContent(item.Key, item.Value);
			}
		}

		private static void LoadUpdateContent(int p_version, string p_hash, int p_downloadAttempt = 0)
		{
			if (p_downloadAttempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(URL_UpdateContent(p_version));
			if (downloadFileResult.IsError)
			{
				LoadUpdateContent(p_version, p_hash, p_downloadAttempt + 1);
			}
			else if (MD5Utils.CheckBytesHash(downloadFileResult.Data, p_hash, ResourcesValidator.Salt))
			{
				if (!UpdateUtils.ProcessUpdateFromPackage(downloadFileResult.Data, p_version))
				{
					LoadUpdateContent(p_version, p_hash, p_downloadAttempt + 1);
				}
			}
			else
			{
				LoadUpdateContent(p_version, p_hash, p_downloadAttempt + 1);
			}
		}
	}
}
