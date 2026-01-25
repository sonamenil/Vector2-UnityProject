using System.Xml;
using Nekki.Vector.Core.DataValidation;

namespace Nekki.Vector.Core.ABTest
{
	public static class ABTestManager
	{
		private const string _Filename = "abgroup.xml";

		private static string _NewGroup;

		private static string _NewHash;

		private static string _UserABGroup = string.Empty;

		public static string FilePath
		{
			get
			{
				return VectorPaths.ABTestExternal + "/abgroup.xml";
			}
		}

		public static bool IsUpdateAvalible
		{
			get
			{
				return _NewGroup != null;
			}
		}

		public static string UserABGroup
		{
			get
			{
				return _UserABGroup;
			}
		}

		private static void LoadFromFile()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocumentAndCheckHash(FilePath, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			if (xmlDocument != null && UserDataValidator.IsValid)
			{
				XmlElement xmlElement = xmlDocument["ABGroup"];
				_UserABGroup = xmlElement.Attributes["User"].Value;
			}
		}

		private static void SaveToFile()
		{
			if (!string.IsNullOrEmpty(_UserABGroup))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
				XmlElement xmlElement = xmlDocument.CreateElement("ABGroup");
				xmlDocument.AppendChild(xmlElement);
				xmlElement.SetAttribute("User", _UserABGroup);
				XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, FilePath);
			}
			else if (FileUtils.FileExists(FilePath))
			{
				FileUtils.DeleteFileAndHash(FilePath);
			}
		}

		public static void SetABGroup(string newGroup, string hash)
		{
			LoadFromFile();
			if (_UserABGroup == newGroup)
			{
				_NewGroup = null;
				return;
			}
			if (string.IsNullOrEmpty(newGroup))
			{
				_NewGroup = string.Empty;
				return;
			}
			_NewGroup = newGroup;
			_NewHash = hash;
		}

		public static void RunUpdate()
		{
			if (_NewGroup != null)
			{
				if (_NewGroup != string.Empty)
				{
					ABTestUpdater.LoadResources(_NewGroup, _NewHash);
					ApplyGameDataChanges();
				}
				else
				{
					ApplyGameDataChanges();
					ABTestUpdater.ClearResources(_UserABGroup);
				}
			}
		}

		private static void ApplyGameDataChanges()
		{
			if (ABTestUpdater.ApplyGroupChanges(UserABGroup, _NewGroup))
			{
				if (string.IsNullOrEmpty(_NewGroup))
				{
					_UserABGroup = string.Empty;
				}
				else
				{
					_UserABGroup = _NewGroup;
				}
			}
			SaveToFile();
		}

		public static void Clear()
		{
			_UserABGroup = string.Empty;
			if (FileUtils.FileExists(FilePath))
			{
				FileUtils.DeleteFileAndHash(FilePath);
			}
		}
	}
}
