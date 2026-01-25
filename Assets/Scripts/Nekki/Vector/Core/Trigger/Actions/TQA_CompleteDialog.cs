using System.Xml;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_CompleteDialog : TriggerQuestAction
	{
		private string _Title;

		private int _Order;

		public TQA_CompleteDialog(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_Title = XmlUtils.ParseString(p_node.Attributes["Title"]);
			_Order = XmlUtils.ParseInt(p_node.Attributes["Order"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			Nekki.Vector.Core.Quest.Quest parent = _Parent.Parent.Parent;
			DialogNotificationManager.ShowQuestCompleteDialog(_Title, parent, OkBtnCallBack, _Order);
		}

		public void OkBtnCallBack()
		{
			_Parent.ActivateActions();
		}
	}
}
