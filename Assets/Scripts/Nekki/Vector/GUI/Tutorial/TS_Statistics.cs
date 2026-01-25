using System.Xml;
using Nekki.Vector.Core.Statistics;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Statistics : TutorialStep
	{
		public const string ElementName = "Statistics";

		public string StepSignal;

		public TS_Statistics()
		{
			_Type = Type.Statistics;
		}

		public TS_Statistics(XmlNode p_node)
		{
			_Type = Type.Statistics;
			StepSignal = XmlUtils.ParseString(p_node.Attributes["StepSignal"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			StatisticsCollector.SetEvent(StatisticsEvent.EventType.Tutor_step, new ArgsDict { { "signal_message", StepSignal } });
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Statistics");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("StepSignal", StepSignal);
		}
	}
}
