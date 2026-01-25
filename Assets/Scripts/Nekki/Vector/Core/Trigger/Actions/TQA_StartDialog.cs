using System.Xml;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_StartDialog : TriggerQuestAction
	{
		private string _Text;

		private string _Title;

		private int _Order;

		private bool _NoDetails;

		public TQA_StartDialog(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_Text = XmlUtils.ParseString(p_node.Attributes["Text"], string.Empty);
			_Title = XmlUtils.ParseString(p_node.Attributes["Title"], string.Empty);
			_Order = XmlUtils.ParseInt(p_node.Attributes["Order"]);
			_NoDetails = XmlUtils.ParseBool(p_node.Attributes["NoDetails"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			DialogNotificationManager.ShowQuestStartDialog(_Title, _Text, _Parent.Parent.Parent, RunNext, _Order, _NoDetails);
		}

		private void RunNext()
		{
			_Parent.ActivateActions();
		}
	}
}
