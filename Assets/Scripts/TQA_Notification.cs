using System.Xml;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.GUI.Dialogs;

public class TQA_Notification : TriggerQuestAction
{
	private string _Text;

	private string _Image;

	private string _Orientation;

	private string _HideBy;

	private string _Card;

	public TQA_Notification(XmlNode p_node, TriggerQuestLoop p_parent)
		: base(p_parent)
	{
		_Text = XmlUtils.ParseString(p_node.Attributes["Text"], string.Empty);
		_Image = XmlUtils.ParseString(p_node.Attributes["Image"], string.Empty);
		_Card = XmlUtils.ParseString(p_node.Attributes["Card"], string.Empty);
		_Orientation = XmlUtils.ParseString(p_node.Attributes["Orientation"], "Left");
		_HideBy = XmlUtils.ParseString(p_node.Attributes["HideBy"], "TimeBlockClicks");
	}

	public override void Activate(ref bool p_runNext)
	{
		Notification.Parameters parameters = new Notification.Parameters();
		parameters.Image = _Image;
		parameters.Card = _Card;
		parameters.Text = _Text;
		parameters.Orientation = _Orientation.OrientationFromName();
		parameters.HideBy = _HideBy.HideByFromName();
		parameters.QueueType = DialogQueueType.Notification;
		parameters.Callback = OkBtnCallBack;
		DialogNotificationManager.ShowSimpleNotification(parameters);
		p_runNext = false;
	}

	public void OkBtnCallBack()
	{
		_Parent.ActivateActions();
	}
}
