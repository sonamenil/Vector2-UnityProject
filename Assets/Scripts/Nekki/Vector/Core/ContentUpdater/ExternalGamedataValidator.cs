using System.IO;
using System.Xml;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.ContentUpdater
{
	public static class ExternalGamedataValidator
	{
		private static bool _IsExternalGamedataValid = true;

		public static void CheckExternalGamedata()
		{
			_IsExternalGamedataValid = IsExternalGamedataXmlsValid() && IsExternalGamedataYamlsValid();
			if (!_IsExternalGamedataValid)
			{
				ClearUserGamedataVersion();
				VectorPaths.ResetExternalGamedata();
			}
		}

		private static bool IsExternalGamedataXmlsValid()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(VectorPaths.ExternalCachedGameData);
			if (!directoryInfo.Exists)
			{
				return true;
			}
			FileInfo[] files = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				if (!XmlUtils.IsFileValid(fileInfo.FullName))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsExternalGamedataYamlsValid()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(VectorPaths.ExternalCachedGameData);
			if (!directoryInfo.Exists)
			{
				return true;
			}
			FileInfo[] files = directoryInfo.GetFiles("*.yaml", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				if (!YamlUtils.IsFileValid(fileInfo.FullName))
				{
					return false;
				}
			}
			return true;
		}

		private static void ClearUserGamedataVersion()
		{
			ClearUserGamedataVersionInFile(DataLocal.FilePath);
			ClearUserGamedataVersionInFile(DataLocal.LocalBackupPath);
		}

		private static void ClearUserGamedataVersionInFile(string p_file)
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(p_file, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			if (xmlDocument != null)
			{
				XmlElement xmlElement = xmlDocument["User"]["Build"];
				if (xmlElement != null)
				{
					xmlElement.SetAttribute("GamedataVersion", "0");
				}
				XmlUtils.SaveDocumentAndUpdateHash(xmlDocument, p_file);
			}
		}
	}
}
