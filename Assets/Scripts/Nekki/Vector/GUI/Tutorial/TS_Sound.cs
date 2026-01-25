using System.Xml;
using Nekki.Vector.Core.Audio;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Sound : TutorialStep
	{
		public const string ElementName = "Sound";

		public string SoundPath;

		public TS_Sound()
		{
			_Type = Type.Sound;
		}

		public TS_Sound(XmlNode p_node)
		{
			_Type = Type.Sound;
			SoundPath = XmlUtils.ParseString(p_node.Attributes["Sound"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			AudioManager.PlaySound(SoundPath);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Sound");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Sound", SoundPath);
		}
	}
}
