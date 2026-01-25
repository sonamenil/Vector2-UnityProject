using System.Xml;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_RemoveItem : TutorialStep
	{
		public const string ElementName = "RemoveItem";

		public string ItemName;

		public TS_RemoveItem()
		{
			_Type = Type.RemoveItem;
		}

		public TS_RemoveItem(XmlNode p_node)
		{
			_Type = Type.RemoveItem;
			ItemName = XmlUtils.ParseString(p_node.Attributes["Name"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			UserItem itemByName = DataLocal.Current.GetItemByName(ItemName);
			if (itemByName != null)
			{
				DataLocal.Current.Remove(itemByName);
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("RemoveItem");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Name", ItemName);
		}
	}
}
