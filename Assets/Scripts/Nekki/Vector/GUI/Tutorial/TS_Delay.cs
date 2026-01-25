using System.Collections;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Delay : TutorialStep
	{
		public const string ElementName = "Delay";

		public float _DelayTime;

		public TS_Delay()
		{
			_Type = Type.Delay;
		}

		public TS_Delay(XmlNode p_node)
		{
			_Type = Type.Delay;
			_DelayTime = XmlUtils.ParseFloat(p_node.Attributes["TimeInSeconds"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			if (_DelayTime == 0f)
			{
				p_runNext = true;
				return;
			}
			p_runNext = false;
			Tutorial.Current.StartCoroutine(LaunchDelay());
		}

		private IEnumerator LaunchDelay()
		{
			yield return new WaitForSeconds(_DelayTime);
			Tutorial.Current.StepOver();
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Delay");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("TimeInSeconds", _DelayTime.ToString());
		}
	}
}
