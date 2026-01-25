using System.Xml;

namespace Nekki.Vector.Core.User
{
	public static class DataLocalWiper
	{
		public static void TryWipe()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(DataLocal.FilePath, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			if (xmlDocument == null)
			{
				return;
			}
			int num = XmlUtils.ParseInt(xmlDocument["User"].Attributes["Version"], -1);
			if (num >= 9)
			{
				return;
			}
			XmlNode xmlNode = null;
			XmlNode xmlNode2 = xmlDocument["User"]["UserCounters"];
			foreach (XmlNode childNode in xmlNode2.ChildNodes)
			{
				if (childNode.Attributes["Name"].Value == "ST_ProgressMarkers")
				{
					xmlNode = childNode;
					break;
				}
			}
			if (xmlNode == null)
			{
				return;
			}
			bool flag = false;
			foreach (XmlNode childNode2 in xmlNode.ChildNodes)
			{
				if (childNode2.Attributes["Name"].Value == "TutorialInProgress" && childNode2.Attributes["Value"].Value == "1")
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				DataLocal.Reset();
			}
		}
	}
}
