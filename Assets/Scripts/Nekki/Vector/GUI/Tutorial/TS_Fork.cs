using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Fork : TutorialStep
	{
		public class TutorialWay
		{
			public TS_Fork Parent;

			public TS_ClickButton Step;

			public bool CurrentSequence;

			public TextAsset Sequence;

			public int StepIndex;
		}

		public const string ElementName = "Fork";

		public List<TutorialWay> Ways = new List<TutorialWay>();

		public TS_Fork()
		{
			_Type = Type.Fork;
		}

		public TS_Fork(XmlNode p_node)
		{
			_Type = Type.Fork;
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				ParseWay(childNode);
			}
		}

		private void ParseWay(XmlNode p_node)
		{
			TutorialWay tutorialWay = new TutorialWay();
			tutorialWay.Parent = this;
			if (p_node["ClickButton"] != null)
			{
				tutorialWay.Step = new TS_ClickButton(p_node["ClickButton"]);
			}
			tutorialWay.StepIndex = XmlUtils.ParseInt(p_node.Attributes["StepIndex"]);
			tutorialWay.CurrentSequence = XmlUtils.ParseBool(p_node.Attributes["CurrentSequence"]);
			tutorialWay.Sequence = TutorialStep.LoadAsetFromXML<TextAsset>(p_node, "CurrentSequence");
			Ways.Add(tutorialWay);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			for (int i = 0; i < Ways.Count; i++)
			{
				if (Ways[i].Step != null)
				{
					Ways[i].Step.Activate(Ways[i]);
				}
			}
		}

		public void RemovePrefabAndDelegate()
		{
			for (int i = 0; i < Ways.Count; i++)
			{
				if (Ways[i].Step != null)
				{
					Ways[i].Step.RemoveDelegate();
					Ways[i].Step.RemoveArrow();
				}
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Fork");
			p_node.AppendChild(xmlElement);
			for (int i = 0; i < Ways.Count; i++)
			{
				XmlElement xmlElement2 = p_node.OwnerDocument.CreateElement("Way");
				xmlElement.AppendChild(xmlElement2);
				if (Ways[i].Step != null)
				{
					Ways[i].Step.SaveToXML(xmlElement2);
				}
				xmlElement2.SetAttribute("StepIndex", Ways[i].StepIndex.ToString());
				xmlElement2.SetAttribute("CurrentSequence", (!Ways[i].CurrentSequence) ? "0" : "1");
				TutorialStep.SaveAssetToXML(xmlElement2, "TextAsset", Ways[i].Sequence);
			}
		}
	}
}
