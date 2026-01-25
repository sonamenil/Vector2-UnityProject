using System.Xml;
using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_StartSequence : TutorialStep
	{
		public const string ElementName = "StartSequence";

		public TextAsset Sequence;

		public TS_StartSequence()
		{
			_Type = Type.StartSequence;
		}

		public TS_StartSequence(XmlNode p_node)
		{
			_Type = Type.StartSequence;
			Sequence = TutorialStep.LoadAsetFromXML<TextAsset>(p_node, "Sequence");
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			Tutorial.Current.Play(Sequence);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("StartSequence");
			p_node.AppendChild(xmlElement);
			TutorialStep.SaveAssetToXML(xmlElement, "Sequence", Sequence);
		}
	}
}
