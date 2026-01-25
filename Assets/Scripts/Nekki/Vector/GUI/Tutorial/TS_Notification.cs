using System.Xml;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Notification : TutorialStep
	{
		public const string ElementName = "Notification";

		public string Text;

		public string Portrait;

		public Notification.Orientation Orientation;

		public Notification.HideBy HideBy;

		public string Card;

		public TS_Notification()
		{
			_Type = Type.Notification;
		}

		public TS_Notification(XmlNode p_node)
		{
			_Type = Type.Notification;
			Text = XmlUtils.ParseString(p_node.Attributes["Text"], string.Empty);
			Card = XmlUtils.ParseString(p_node.Attributes["Card"]);
			Portrait = XmlUtils.ParseString(p_node.Attributes["Portrait"], string.Empty);
			Orientation = XmlUtils.ParseString(p_node.Attributes["Orientation"], "Left").OrientationFromName();
			HideBy = XmlUtils.ParseString(p_node.Attributes["HideBy"], "TimeBlockClicks").HideByFromName();
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			Notification.Parameters parameters = new Notification.Parameters();
			parameters.Image = Portrait;
			parameters.Text = Text;
			parameters.Card = Card;
			parameters.Orientation = Orientation;
			parameters.HideBy = HideBy;
			parameters.QueueType = DialogQueueType.Dialog;
			parameters.Callback = StepDoneCallback;
			DialogNotificationManager.ShowSimpleNotification(parameters);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Notification");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Text", Text);
			xmlElement.SetAttribute("Card", Card);
			xmlElement.SetAttribute("Portrait", Portrait);
			xmlElement.SetAttribute("Orientation", Orientation.GetName());
			xmlElement.SetAttribute("HideBy", HideBy.GetName());
		}

		public void StepDoneCallback()
		{
			Tutorial.Current.StepOver();
		}
	}
}
