using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_ChangeCounter : TutorialStep
	{
		public const string ElementName = "ChangeCounter";

		public string Counter = string.Empty;

		public string Namespace = "Tutorial";

		public int Value;

		public TS_ChangeCounter()
		{
			_Type = Type.ChangeCounter;
		}

		public TS_ChangeCounter(XmlNode p_node)
		{
			_Type = Type.ChangeCounter;
			Counter = XmlUtils.ParseString(p_node.Attributes["Counter"]);
			Namespace = XmlUtils.ParseString(p_node.Attributes["Namespace"]);
			Value = XmlUtils.ParseInt(p_node.Attributes["Value"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			CounterController.Current.CreateCounterOrSetValue(Counter, Value, Namespace);
			DataLocal.Current.Save(true);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("ChangeCounter");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Counter", Counter);
			xmlElement.SetAttribute("Namespace", Namespace);
			xmlElement.SetAttribute("Value", Value.ToString());
		}
	}
}
