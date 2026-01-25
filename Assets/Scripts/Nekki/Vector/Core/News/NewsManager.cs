using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.DataValidation;

namespace Nekki.Vector.Core.News
{
	public static class NewsManager
	{
		private const int _MaxDownloadAttempt = 2;

		private const string _UrlFolder = "news/";

		private const string _ConfigFilename = "news_config.bin";

		private const string _ContentFolder = "/content";

		private const string _ContentFile = "content.xml";

		public static readonly byte[] Key = new byte[32]
		{
			131, 3, 24, 117, 252, 110, 88, 165, 194, 19,
			177, 116, 161, 152, 50, 103, 49, 72, 84, 248,
			6, 137, 157, 206, 170, 136, 58, 48, 231, 107,
			37, 232
		};

		public static readonly byte[] IV = new byte[16]
		{
			84, 114, 172, 125, 79, 103, 38, 202, 223, 249,
			54, 12, 51, 234, 158, 253
		};

		private static string _LocalFile;

		private static string _LocalHash;

		private static int _NewsShowCount = 0;

		private static string _CustomNewsFolder = null;

		private static string URL_ConfigFile
		{
			get
			{
				return URLCreator.Make("news/news_config.bin");
			}
		}

		private static string ConfigFilename
		{
			get
			{
				return VectorPaths.NewsExternal + "/news_config.bin";
			}
		}

		public static string ContentFolder
		{
			get
			{
				return VectorPaths.CurrentResources + "/visionnews";
			}
		}

		private static string ContentFile
		{
			get
			{
				return ContentFolder + "/content.xml";
			}
		}

		private static string URL_Content(string p_file)
		{
			return URLCreator.Make("news/" + p_file);
		}

		public static void ActivateCustomNews(string p_path)
		{
			_CustomNewsFolder = p_path;
		}

		public static void ResetCustomNews()
		{
			_CustomNewsFolder = null;
		}

		public static void Init()
		{
		}

		private static void TryLoadConfigFile(int p_attempt = 0)
		{
			if (p_attempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(URL_ConfigFile);
			if (downloadFileResult.IsError)
			{
				if (downloadFileResult.IsNotFound)
				{
					ClearNewsContent();
					return;
				}
				DebugUtils.LogFormat("[NewsManager]: load url - {0}, error - {1}", downloadFileResult.Url, downloadFileResult.ErrorMsg);
				TryLoadConfigFile(p_attempt + 1);
			}
			else
			{
				byte[] p_bytes = AESUtils.DecryptBytes(downloadFileResult.Data, Key, IV);
				XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
				if (xmlDocument == null)
				{
					TryLoadConfigFile(p_attempt + 1);
				}
				else
				{
					ParseRemoteConfigFile(xmlDocument);
				}
			}
		}

		private static void OnLoadFail()
		{
		}

		private static bool ParseLocalConfigFile()
		{
			if (!File.Exists(ConfigFilename))
			{
				return false;
			}
			byte[] p_bytes = AESUtils.DecryptFileToBytes(Key, IV, ConfigFilename, true);
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
			if (xmlDocument == null)
			{
				return false;
			}
			return ParseConfigFile(xmlDocument, ref _LocalFile, ref _LocalHash);
		}

		private static void ParseRemoteConfigFile(XmlDocument p_doc)
		{
			string p_file = string.Empty;
			string p_hash = string.Empty;
			if (ParseConfigFile(p_doc, ref p_file, ref p_hash))
			{
				bool flag = _LocalHash != p_hash;
				if (flag)
				{
					_LocalFile = p_file;
					_LocalHash = p_hash;
					AESUtils.EncryptBytesToFile(Encoding.UTF8.GetBytes(p_doc.OuterXml), Key, IV, ConfigFilename);
				}
				if (flag || !Directory.Exists(ContentFolder))
				{
					DebugUtils.Log("[NewsManager]: try to download content");
					TryToLoadContent(_LocalFile, _LocalHash);
				}
			}
			else
			{
				ClearNewsContent();
			}
		}

		private static bool ParseConfigFile(XmlDocument p_doc, ref string p_file, ref string p_hash)
		{
			XmlNode xmlNode = p_doc["News"];
			if (xmlNode == null)
			{
				return false;
			}
			XmlNode xmlNode2 = xmlNode["N"];
			if (xmlNode2 == null)
			{
				return false;
			}
			p_file = XmlUtils.ParseString(xmlNode2.Attributes["File"], string.Empty);
			p_hash = XmlUtils.ParseString(xmlNode2.Attributes["Hash"], string.Empty);
			return true;
		}

		private static void TryToLoadContent(string p_file, string p_hash, int p_downloadAttempt = 0)
		{
			if (p_downloadAttempt >= 2)
			{
				OnLoadFail();
				return;
			}
			InternetUtils.DownloadFileResult downloadFileResult = InternetUtils.DownloadFile(URL_Content(p_file));
			if (downloadFileResult.IsError)
			{
				if (downloadFileResult.IsNotFound)
				{
					ClearNewsContent();
				}
				else
				{
					TryToLoadContent(p_file, p_hash, p_downloadAttempt + 1);
				}
			}
			else if (MD5Utils.CheckBytesHash(downloadFileResult.Data, p_hash, ResourcesValidator.Salt))
			{
				if (!ExtractNewsContent(downloadFileResult.Data))
				{
					TryToLoadContent(p_file, p_hash, p_downloadAttempt + 1);
				}
			}
			else
			{
				TryToLoadContent(p_file, p_hash, p_downloadAttempt + 1);
			}
		}

		private static void ClearNewsContent()
		{
			if (Directory.Exists(ContentFolder))
			{
				Directory.Delete(ContentFolder, true);
			}
		}

		private static bool ExtractNewsContent(byte[] p_data)
		{
			ClearNewsContent();
			Directory.CreateDirectory(ContentFolder);
			if (!CompressUtils.DecompressToDirectory(p_data, ContentFolder))
			{
				return false;
			}
			return true;
		}

		public static void ShowNews()
		{
			if (_NewsShowCount <= 0 && (int)CounterController.Current.CounterNewsBlock == 0)
			{
				XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(ContentFile, string.Empty);
				if (xmlDocument == null)
				{
					ClearNewsContent();
					DebugUtils.Log("[NewsManager]: news xml is broken!");
				}
				else
				{
					DebugUtils.Log("[NewsManager]: start news show!");
					_NewsShowCount++;
					NewsParser.ShowNewsDialog(xmlDocument["Dialog"]);
				}
			}
		}
	}
}
