using System.Xml;
using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public class ArrowData
	{
		public GameObject Prefab;

		public Vector3 Position;

		public Vector2 MoveDelta;

		public float Angle;

		public float Delay;

		public ArrowData()
		{
		}

		public ArrowData(XmlNode p_node)
		{
			if (p_node != null)
			{
				Prefab = TutorialStep.LoadAsetFromXML<GameObject>(p_node, "Prefab");
				Position.x = XmlUtils.ParseFloat(p_node["Position"].Attributes["X"]);
				Position.y = XmlUtils.ParseFloat(p_node["Position"].Attributes["Y"]);
				Position.z = XmlUtils.ParseFloat(p_node["Position"].Attributes["Z"]);
				MoveDelta.x = XmlUtils.ParseFloat(p_node["DeltaMove"].Attributes["X"]);
				MoveDelta.y = XmlUtils.ParseFloat(p_node["DeltaMove"].Attributes["Y"]);
				Angle = XmlUtils.ParseFloat(p_node["Angle"].Attributes["Value"]);
				XmlNode xmlNode = p_node["Delay"];
				if (xmlNode != null)
				{
					Delay = XmlUtils.ParseFloat(xmlNode.Attributes["Value"]);
				}
			}
		}

		public TutorialArrow CreateArrow(Transform p_parent)
		{
			GameObject gameObject = Object.Instantiate(Prefab);
			gameObject.transform.SetParent(Tutorial.Current._Scene.transform, false);
			TutorialArrow component = gameObject.GetComponent<TutorialArrow>();
			component.DefaultPosition = p_parent.position;
			component.Position = Position;
			component.Angle = Angle;
			component.Delta = MoveDelta;
			component.Delay = Delay;
			return component;
		}

		public void SaveToXML(XmlElement p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Arrow");
			p_node.AppendChild(xmlElement);
			TutorialStep.SaveAssetToXML(xmlElement, "Prefab", Prefab);
			XmlElement xmlElement2 = p_node.OwnerDocument.CreateElement("Position");
			xmlElement.AppendChild(xmlElement2);
			xmlElement2.SetAttribute("X", Position.x.ToString());
			xmlElement2.SetAttribute("Y", Position.y.ToString());
			xmlElement2.SetAttribute("Z", Position.z.ToString());
			XmlElement xmlElement3 = p_node.OwnerDocument.CreateElement("DeltaMove");
			xmlElement.AppendChild(xmlElement3);
			xmlElement3.SetAttribute("X", MoveDelta.x.ToString());
			xmlElement3.SetAttribute("Y", MoveDelta.y.ToString());
			XmlElement xmlElement4 = p_node.OwnerDocument.CreateElement("Angle");
			xmlElement.AppendChild(xmlElement4);
			xmlElement4.SetAttribute("Value", Angle.ToString());
			XmlElement xmlElement5 = p_node.OwnerDocument.CreateElement("Delay");
			xmlElement.AppendChild(xmlElement5);
			xmlElement5.SetAttribute("Value", Delay.ToString());
		}
	}
}
