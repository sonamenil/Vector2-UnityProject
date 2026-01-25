using System.Xml;
using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_SaveBackup : TutorialStep
	{
		public const string ElementName = "SaveBackup";

		public TS_SaveBackup()
		{
			_Type = Type.SaveBackup;
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			GameRestorer.SaveBackup();
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement newChild = p_node.OwnerDocument.CreateElement("SaveBackup");
			p_node.AppendChild(newChild);
		}
	}
}
