using System.Xml;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_GiveMoney : TutorialStep
	{
		public const string ElementName = "GiveMoney";

		public string MoneyType = string.Empty;

		public string Value;

		public TS_GiveMoney()
		{
			_Type = Type.GiveMoney;
		}

		public TS_GiveMoney(XmlNode p_node)
		{
			_Type = Type.GiveMoney;
			MoneyType = XmlUtils.ParseString(p_node.Attributes["Type"]);
			Value = XmlUtils.ParseString(p_node.Attributes["Value"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			int valueInt = Variable.CreateVariable(Value, null).ValueInt;
			switch (MoneyType)
			{
			case "Money1":
			{
				DataLocal current3 = DataLocal.Current;
				current3.Money1 = (int)current3.Money1 + valueInt;
				break;
			}
			case "Money2":
			{
				DataLocal current2 = DataLocal.Current;
				current2.Money2 = (int)current2.Money2 + valueInt;
				break;
			}
			case "Money3":
			{
				DataLocal current = DataLocal.Current;
				current.Money3 = (int)current.Money3 + valueInt;
				break;
			}
			}
			DataLocal.Current.Save(true);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("GiveMoney");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Type", MoneyType);
			xmlElement.SetAttribute("Value", Value);
		}
	}
}
