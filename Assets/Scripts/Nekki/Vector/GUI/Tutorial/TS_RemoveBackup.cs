using System.Xml;
using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_RemoveBackup : TutorialStep
	{
		public const string ElementName = "RemoveBackup";

		public TS_RemoveBackup()
		{
			_Type = Type.RemoveBackup;
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			GameRestorer.RemoveBackup();
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement newChild = p_node.OwnerDocument.CreateElement("RemoveBackup");
			p_node.AppendChild(newChild);
		}
	}
}
