using System.Xml;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_End : TutorialStep
	{
		public const string ElementName = "End";

		public TS_End()
		{
			_Type = Type.End;
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			Tutorial.Current.Stop();
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement newChild = p_node.OwnerDocument.CreateElement("End");
			p_node.AppendChild(newChild);
		}
	}
}
