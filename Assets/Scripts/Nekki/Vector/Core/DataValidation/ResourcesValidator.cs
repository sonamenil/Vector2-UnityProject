using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.DataValidation
{
	public static class ResourcesValidator
	{
		private const string _Filename = "validation.bytes";

		private const string _DefaultFilename = "validation_default.bytes";

		private const string _UpdateFilename = "validation_update.bytes";

		public static readonly string Salt = "FaE=LYvpGHdSwqxwl618wqO+Qchj|r*QXg7o_KNmKaD^QI2vyPwt-h3H8*uJ";

		public static readonly byte[] Key = new byte[32]
		{
			183, 85, 139, 66, 211, 16, 24, 96, 215, 154,
			166, 55, 129, 189, 141, 32, 233, 192, 151, 254,
			82, 120, 96, 21, 11, 52, 51, 143, 89, 19,
			213, 40
		};

		public static readonly byte[] IV = new byte[16]
		{
			166, 213, 37, 50, 73, 198, 222, 73, 115, 187,
			192, 168, 219, 23, 201, 114
		};

		private static ValidationResult _ValidationResult = ValidationResult.Success;

		private static int _GamedataVersion;

		private static bool _IsValidationStarted = false;

		private static XmlDocument _Document = null;

		private static Dictionary<string, XmlNode> _FileHashNodes = new Dictionary<string, XmlNode>();

		public static string Filename
		{
			get
			{
				return VectorPaths.ExternalCachedGameData + "/validation.bytes";
			}
		}

		public static string DefaultFilename
		{
			get
			{
				return VectorPaths.Settings + "/validation_default.bytes";
			}
		}

		public static string UpdateFilename
		{
			get
			{
				return VectorPaths.ExternalCachedGameData + "/validation_update.bytes";
			}
		}

		public static bool IsValid
		{
			get
			{
				return _ValidationResult == ValidationResult.Success;
			}
		}

		public static int GamedataVersion
		{
			get
			{
				return _GamedataVersion;
			}
		}

		public static void RemoveOldFileValidation()
		{
			string path = VectorPaths.StorageUserData + "/validation.bytes";
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static void PrepareValidation()
		{
			if (!ValidatorSettings.IsEnabled)
			{
				ClearTempData();
				return;
			}
			bool flag = false;
			if (!FileUtils.FileExists(Filename))
			{
				CopyValidationFileFromResources();
				flag = true;
			}
			_Document = OpenValidationFile(Filename);
			if (_Document == null)
			{
				if (FileUtils.FileExists(Filename))
				{
					FileUtils.DeleteFile(Filename);
				}
				_ValidationResult = ValidationResult.Failed;
				DebugUtils.LogError("[ResourcesValidator]: falied to load validation.bytes!");
			}
			else
			{
				if (!flag)
				{
					TryUpdateValidationFile(ref _Document);
				}
				_GamedataVersion = XmlUtils.ParseInt(_Document["Hashes"].Attributes["GamedataVersion"]);
				BuildFileHashNodesDictionary();
			}
		}

		private static void BuildFileHashNodesDictionary()
		{
			_FileHashNodes.Clear();
			foreach (XmlNode childNode in _Document["Hashes"].ChildNodes)
			{
				_FileHashNodes.Add(XmlUtils.ParseString(childNode.Attributes["Name"], string.Empty), childNode);
			}
		}

		public static void RunValidation()
		{
			if (!ValidatorSettings.IsEnabled || _IsValidationStarted)
			{
				ClearTempData();
				return;
			}
			if (_GamedataVersion < DataLocal.Current.GamedataVersion)
			{
				_ValidationResult = ValidationResult.GamedataOutdated;
				return;
			}
			if (_GamedataVersion > DataLocal.Current.GamedataVersion)
			{
				DataLocal.Current.GamedataVersion = _GamedataVersion;
			}
			_IsValidationStarted = true;
			DebugUtils.Log("[ResourcesValidator]: start resources validation!");
			bool flag = true;
			foreach (XmlNode childNode in _Document["Hashes"].ChildNodes)
			{
				flag = flag && CheckIsFileValid(XmlUtils.ParseString(childNode.Attributes["Name"], string.Empty), XmlUtils.ParseString(childNode.Attributes["Value"], string.Empty));
				if (!flag)
				{
					break;
				}
			}
			_ValidationResult = (flag ? ValidationResult.Success : ValidationResult.Failed);
			ClearTempData();
			DebugUtils.Log("[ResourcesValidator]: finish resources validation!");
		}

		private static void TryUpdateValidationFile(ref XmlDocument p_currentDoc)
		{
			XmlAttribute xmlAttribute = p_currentDoc["Hashes"].Attributes["GamedataVersion"];
			XmlAttribute xmlAttribute2 = p_currentDoc["Hashes"].Attributes["BuildVersion"];
			if (xmlAttribute == null && xmlAttribute2 == null)
			{
				CopyValidationFileFromResources();
				p_currentDoc = OpenValidationFile(Filename);
				return;
			}
			byte[] p_bytes = AESUtils.DecryptFileToBytes(Key, IV, DefaultFilename);
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
			if (xmlDocument != null)
			{
				XmlAttribute xmlAttribute3 = xmlDocument["Hashes"].Attributes["BuildVersion"];
				if (xmlAttribute2.Value != xmlAttribute3.Value)
				{
					VectorPaths.BackupBundles();
					VectorPaths.ResetExternalGamedata();
					VectorPaths.RestoreBundles();
					VectorPaths.TEMP_ResetExternalGamedataAndroid();
					CopyValidationFileFromResources();
					p_currentDoc = OpenValidationFile(Filename);
				}
			}
		}

		private static void CopyValidationFileFromResources()
		{
			FileUtils.CopyFileFromResourcesToExternal(DefaultFilename, Filename);
		}

		private static XmlDocument OpenValidationFile(string p_filename)
		{
			byte[] p_bytes = AESUtils.DecryptFileToBytes(Key, IV, p_filename, true);
			return XmlUtils.OpenXMLDocumentFromBytes(p_bytes);
		}

		public static void UpdateValidationHashes(Dictionary<string, string> p_hashesUpdate, int p_newVersion = -1)
		{
			if (!ValidatorSettings.IsEnabled)
			{
				return;
			}
			XmlNode xmlNode = _Document["Hashes"];
			foreach (KeyValuePair<string, string> item in p_hashesUpdate)
			{
				string key = item.Key;
				string value = item.Value;
				if (_FileHashNodes.ContainsKey(key))
				{
					_FileHashNodes[key].Attributes["Value"].Value = value;
					continue;
				}
				XmlElement xmlElement = _Document.CreateElement("File");
				xmlElement.SetAttribute("Name", key);
				xmlElement.SetAttribute("Value", value);
				xmlNode.AppendChild(xmlElement);
				_FileHashNodes[key] = xmlElement;
			}
			if (p_newVersion != -1)
			{
				_GamedataVersion = p_newVersion;
				xmlNode.Attributes["GamedataVersion"].Value = _GamedataVersion.ToString();
			}
			AESUtils.EncryptBytesToFile(Encoding.UTF8.GetBytes(_Document.OuterXml), Key, IV, Filename);
			DebugUtils.Log("[ResourcesValidator]: validation.bytes successfully updated!");
		}

		private static bool CheckIsFileValid(string p_name, string p_hash)
		{
			if (CheckFileInPath(VectorPaths.ExternalCachedGameData + "/" + p_name, p_hash))
			{
				return true;
			}
			bool flag = CheckFileHash(VectorPaths.GameData + "/" + p_name, p_hash);
			if (!flag)
			{
				DebugUtils.LogErrorFormat("[ResourcesValidator]: file is missing or has incorrect file hash - {0} [{1} | {2}] !", p_name, p_hash, GetFileHash(VectorPaths.GameData + "/" + p_name));
			}
			return flag;
		}

		private static bool CheckFileHash(string p_path, string p_hash)
		{
			return (!VectorPaths.UsingResources) ? CheckFileInPath(p_path, p_hash) : CheckFileInResources(p_path, p_hash);
		}

		private static bool CheckFileInPath(string p_path, string p_hash)
		{
			if (!File.Exists(p_path))
			{
				return false;
			}
			return MD5Utils.CheckFileHash(p_path, p_hash, Salt);
		}

		private static bool CheckFileInResources(string p_name, string p_hash)
		{
			string text = ResourceManager.GetText(p_name);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			return MD5Utils.CheckStringHash(text, p_hash, Salt);
		}

		private static string GetFileHash(string p_path)
		{
			return (!VectorPaths.UsingResources) ? GetHashFileInPath(p_path) : GetHashInResources(p_path);
		}

		private static string GetHashFileInPath(string p_path)
		{
			if (!File.Exists(p_path))
			{
				return "null";
			}
			return MD5Utils.MD5HashFile(p_path, Salt);
		}

		private static string GetHashInResources(string p_name)
		{
			string text = ResourceManager.GetText(p_name);
			if (string.IsNullOrEmpty(text))
			{
				return "null";
			}
			return MD5Utils.MD5HashString(text, Salt);
		}

		public static bool ApplyValidationResult()
		{
			if (!ValidatorSettings.IsEnabled)
			{
				return true;
			}
			switch (_ValidationResult)
			{
			case ValidationResult.GamedataOutdated:
				VectorPaths.ResetExternalGamedata();
				ShowDialog("[ResourcesValidator]: application resources is outdated!", "OutdatedData", "^GUI.Labels.DialogWindow.Text.ResourcesValidationOutdated^");
				return false;
			default:
				return true;
			}
		}

		private static void ShowDialog(string p_log, string p_statisticsMsg, string p_dialogContent)
		{
			DebugUtils.LogError(p_log);
			GoogleAnalyticsV4.getInstance().LogEvent("Game", "Validation", p_statisticsMsg, 1L);
			DialogNotificationManager.ShowValidationFailDialog(p_dialogContent);
		}

		private static void ClearTempData()
		{
			_Document = null;
			_FileHashNodes.Clear();
		}

		public static bool IsFileSupportValidation(string p_filename)
		{
			return _FileHashNodes.ContainsKey(p_filename);
		}
	}
}
