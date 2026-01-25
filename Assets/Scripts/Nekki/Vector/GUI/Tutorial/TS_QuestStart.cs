using System.Xml;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_QuestStart : TutorialStep
	{
		public const string ElementName = "QuestStart";

		public string QuestName;

		public string Title;

		public string Text;

		public int Order;

		public Quest Quest
		{
			get
			{
				return QuestManager.Current.GetQuestByName(QuestName);
			}
		}

		public TS_QuestStart()
		{
			_Type = Type.QuestStart;
		}

		public TS_QuestStart(XmlNode p_node)
		{
			_Type = Type.QuestStart;
			QuestName = XmlUtils.ParseString(p_node.Attributes["QuestName"]);
			Title = XmlUtils.ParseString(p_node.Attributes["Title"]);
			Text = XmlUtils.ParseString(p_node.Attributes["Text"]);
			Order = XmlUtils.ParseInt(p_node.Attributes["Order"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			DialogNotificationManager.ShowQuestStartDialog(Title, Text, Quest, OkBtnCallBack, Order);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("QuestStart");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("QuestName", QuestName);
			xmlElement.SetAttribute("Title", Title);
			xmlElement.SetAttribute("Text", Text);
			xmlElement.SetAttribute("Order", Order.ToString());
		}

		public void OkBtnCallBack()
		{
			Tutorial.Current.StepOver();
		}
	}
}
